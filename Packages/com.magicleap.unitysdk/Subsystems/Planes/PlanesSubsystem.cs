// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="PlanesSubsystem.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2021 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.Lumin;
using UnityEngine.Scripting;
using UnityEngine.XR.MagicLeap.Native;

namespace UnityEngine.XR.MagicLeap
{
    /// <summary>
    /// The Magic Leap implementation of the <c>XRPlaneSubsystem</c>. Do not create this directly.
    /// Use <c>PlanesSubsystemDescriptor.Create()</c> instead.
    /// </summary>
    [Preserve]
    [UsesLuminPrivilege("SpatialMapping")]
    public sealed partial class PlanesSubsystem : XRPlaneSubsystem
    {
        public static partial class Extensions
        {
            public static Extensions.PlanesQuery Query
            {
                get => query;
                set
                {
                    QuerySet = true;
                    query = value;
                }
            }

            internal static bool QuerySet
            {
                get;
                private set;
            }

            private static Extensions.PlanesQuery query = default;

            public struct PlanesQuery
            {
                /// <summary>
                /// The flags to apply to this query.
                /// </summary>
                public PlanesSubsystem.Extensions.MLPlanesQueryFlags Flags;

                /// <summary>
                /// The center of the bounding box which defines where planes extraction should occur.
                /// </summary>
                public Vector3 BoundsCenter;

                /// <summary>
                /// The rotation of the bounding box where planes extraction will occur.
                /// </summary>
                public Quaternion BoundsRotation;

                /// <summary>
                /// The size of the bounding box where planes extraction will occur.
                /// </summary>
                public Vector3 BoundsExtents;

                /// <summary>
                /// The maximum number of results that should be returned.
                /// </summary>
                public uint MaxResults;

                /// <summary>
                /// If MLPlanesQueryFlags.IgnoreHoles is not set, holes with a perimeter
                /// (in meters) smaller than this value will be ignored, and can be part of
                /// the plane. This value cannot be lower than 0 (lower values will be
                /// capped to this minimum).
                /// </summary>
                public float MinHoleLength;

                /// <summary>
                /// The minimum area (in squared meters) of planes to be returned. This value
                /// cannot be lower than 0.04 (lower values will be capped to this minimum).
                /// </summary>
                public float MinPlaneArea;
            }
        }

#if UNITY_2020_2_OR_NEWER
        MagicLeapProvider magicLeapProvider => (MagicLeapProvider)provider;
#else
        MagicLeapProvider magicLeapProvider;

        protected override Provider CreateProvider()
        {
            magicLeapProvider = new MagicLeapProvider();
            return magicLeapProvider;
        }
#endif

        internal static TrackableId GetTrackableId(ulong planeId)
        {
            const ulong planeTrackableIdSalt = 0xf52b75076e45ad88;
            return new TrackableId(planeId, planeTrackableIdSalt);
        }

        class MagicLeapProvider : Provider
        {
            internal Extensions.PlanesQuery defaultPlanesQuery
            {
                get
                {
                    if (Extensions.QuerySet)
                        return Extensions.Query;
                    else
                    {
                        return new Extensions.PlanesQuery
                        {
                            Flags = m_defaultQueryFlags,
                            BoundsCenter = Vector3.zero,
                            BoundsRotation = Quaternion.identity,
                            BoundsExtents = Vector3.one * 20f,
                            MaxResults = m_MaxResults,
                            MinHoleLength = 0.5f,
                            MinPlaneArea = 0.25f
                        };
                    }
                }
            }

            ulong m_PlanesTracker = Native.MagicLeapNativeBindings.InvalidHandle;
            ulong m_QueryHandle = Native.MagicLeapNativeBindings.InvalidHandle;

            uint m_MaxResults = 4;
            uint m_LastNumResults;
            uint m_PreviousLastNumResults = 0;

            Extensions.MLPlanesQueryFlags m_defaultQueryFlags = Extensions.MLPlanesQueryFlags.None;

            Dictionary<TrackableId, BoundedPlane> m_Planes = new Dictionary<TrackableId, BoundedPlane>();

            Extensions.MLPlaneBoundariesList m_BoundariesList;

            Extensions.MLPlanesQueryFlags m_RequestedPlaneDetectionMode;
            Extensions.MLPlanesQueryFlags m_CurrentPlaneDetectionMode;

            // todo: 2019-05-22: Unity.Collections.NativeHashMap would be better
            // but introduces another package dependency. Probably not worth it
            // for just this one thing, but if it becomes a dependency, we should
            // switch to using the NativeHashMap (or NativeHashSet if it exists).
            static HashSet<TrackableId> s_CurrentSet = new HashSet<TrackableId>();

            public MagicLeapProvider() { }

            public override PlaneDetectionMode requestedPlaneDetectionMode
            {
                get => m_RequestedPlaneDetectionMode.ToPlaneDetectionMode();
                set => m_RequestedPlaneDetectionMode = value.ToMLQueryFlags();
            }

            public override PlaneDetectionMode currentPlaneDetectionMode => m_CurrentPlaneDetectionMode.ToPlaneDetectionMode();

            private bool CreateClient()
            {
                if (m_PlanesTracker != Native.MagicLeapNativeBindings.InvalidHandle)
                {
                    // client already created
                    return true;
                }

                if (!MLPermissions.CheckPermission(MLPermission.SpatialMapping).IsOk)
                {
                    // permission denied
                    return false;
                }

                var result = NativeBindings.MLPlanesCreate(out m_PlanesTracker);
                if (!MLResult.IsOK(result))
                {
                    Debug.LogError($"Failed to start planes subsystem, reason: {result}");
                    return false;
                }

                if (m_BoundariesList.valid)
                {
                    Debug.LogError($"Restarting the plane subsystem with an existing boundaries list.");
                }
                m_BoundariesList = Extensions.MLPlaneBoundariesList.Create();

                return true;
            }

            public override void Start()
            {
                // Don't create client right away as permission request will not be approved yet

                SubsystemFeatures.SetFeatureRequested(Feature.PlaneTracking, true);
            }

            public override void Stop()
            {
#if UNITY_MAGICLEAP || UNITY_ANDROID
                if (m_PlanesTracker != Native.MagicLeapNativeBindings.InvalidHandle)
                {
                    if (m_BoundariesList.valid)
                    {
                        NativeBindings.MLPlanesReleaseBoundariesList(m_PlanesTracker, ref m_BoundariesList);
                        m_BoundariesList = Extensions.MLPlaneBoundariesList.Create();
                    }

                    NativeBindings.MLPlanesDestroy(m_PlanesTracker);
                    m_PlanesTracker = Native.MagicLeapNativeBindings.InvalidHandle;
                }

                SubsystemFeatures.SetFeatureRequested(Feature.PlaneTracking, false);
                m_QueryHandle = Native.MagicLeapNativeBindings.InvalidHandle;
#endif
            }

            public override void Destroy() { }

            public unsafe PlaneBoundaryCollection GetAllBoundariesForPlane(TrackableId trackableId)
            {
                if (!m_Planes.TryGetValue(trackableId, out BoundedPlane plane))
                    return default;

                // MLPlaneBoundaries is an array of boundaries, so planeBoundariesArray represents an array of MLPlaneBoundaries
                // which is itself an array of boundaries.
                var planeBoundariesArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Extensions.MLPlaneBoundaries>(
                    m_BoundariesList.plane_boundaries,
                    (int)m_BoundariesList.plane_boundaries_count,
                    Allocator.None);

#if UNITY_EDITOR
                var safetyHandle = AtomicSafetyHandle.Create();
                NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref planeBoundariesArray, safetyHandle);
#endif

                // Find the plane boundaries with the given trackable id
                foreach (var planeBoundaries in planeBoundariesArray)
                {
                    if (GetTrackableId(planeBoundaries.id) == trackableId)
                    {
                        return new PlaneBoundaryCollection(planeBoundaries, plane.pose);
                    }
                }

                return default;
            }

            public unsafe override void GetBoundary(
                TrackableId trackableId,
                Allocator allocator,
                ref NativeArray<Vector2> convexHullOut)
            {
                var boundaries = GetAllBoundariesForPlane(trackableId);
                if (boundaries.count > 0)
                {
                    // TODO 2019-05-21: handle multiple boundaries?
                    using (var polygon = boundaries[0].GetPolygon(Allocator.TempJob))
                    {
                        ConvexHullGenerator.Giftwrap(polygon, allocator, ref convexHullOut);
                        return;
                    }
                }
                else
                {
                    if (m_Planes.TryGetValue(trackableId, out BoundedPlane plane))
                    {
                        float halfHeight = plane.height * 0.5f;
                        float halfWidth = plane.width * 0.5f;

                        var calculatedBoundaries = new NativeArray<Vector2>(4, Allocator.Temp);
                        calculatedBoundaries[0] = new Vector2(halfHeight, halfWidth);
                        calculatedBoundaries[1] = new Vector2(-halfHeight, halfWidth);
                        calculatedBoundaries[2] = new Vector2(-halfHeight, -halfWidth);
                        calculatedBoundaries[3] = new Vector2(halfHeight, -halfWidth);

                        ConvexHullGenerator.Giftwrap(calculatedBoundaries, allocator, ref convexHullOut);
                        return;
                    }
                }

                CreateOrResizeNativeArrayIfNecessary<Vector2>(0, allocator, ref convexHullOut);
            }


            public unsafe override TrackableChanges<BoundedPlane> GetChanges(BoundedPlane defaultPlane, Allocator allocator)
            {
                if (m_PlanesTracker == Native.MagicLeapNativeBindings.InvalidHandle)
                {
                    if (!CreateClient())
                    {
                        return default;
                    }
                }

                if (m_QueryHandle == Native.MagicLeapNativeBindings.InvalidHandle)
                {
                    m_QueryHandle = BeginNewQuery();
                    return default;
                }
                else
                {
                    // Get results
                    var mlPlanes = new NativeArray<Extensions.MLPlane>((int)m_MaxResults, Allocator.TempJob);
                    if (m_BoundariesList.valid)
                    {
                        NativeBindings.MLPlanesReleaseBoundariesList(m_PlanesTracker, ref m_BoundariesList);
                    }
                    m_BoundariesList = Extensions.MLPlaneBoundariesList.Create();

                    try
                    {
                        var result = NativeBindings.MLPlanesQueryGetResultsWithBoundaries(
                            m_PlanesTracker, m_QueryHandle,
                            (Extensions.MLPlane*)mlPlanes.GetUnsafePtr(), out uint numResults, ref m_BoundariesList);

                        if (m_defaultQueryFlags == Extensions.MLPlanesQueryFlags.None)
                        {
                            m_defaultQueryFlags = m_RequestedPlaneDetectionMode | Extensions.MLPlanesQueryFlags.Polygons | Extensions.MLPlanesQueryFlags.Semantic_All;
                        }

                        switch (result)
                        {
                            case MLResult.Code.Ok:
                                {
                                    m_PreviousLastNumResults = m_LastNumResults;
                                    m_LastNumResults = numResults;
                                    m_QueryHandle = BeginNewQuery();

                                    using (var uPlanes = new NativeArray<BoundedPlane>((int)numResults, Allocator.TempJob))
                                    {
                                        new CopyPlaneResultsJob
                                        {
                                            planesIn = mlPlanes,
                                            planesOut = uPlanes
                                        }.Schedule((int)numResults, 1).Complete();

                                        var added = new NativeFixedList<BoundedPlane>((int)numResults, Allocator.Temp);
                                        var updated = new NativeFixedList<BoundedPlane>((int)numResults, Allocator.Temp);
                                        var removed = new NativeFixedList<TrackableId>((int)m_PreviousLastNumResults, Allocator.Temp);

                                        s_CurrentSet.Clear();
                                        for (int i = 0; i < numResults; ++i)
                                        {
                                            var uPlane = uPlanes[i];
                                            var trackableId = uPlane.trackableId;
                                            s_CurrentSet.Add(trackableId);

                                            if (m_Planes.ContainsKey(trackableId))
                                            {
                                                updated.Add(uPlane);
                                            }
                                            else
                                            {
                                                added.Add(uPlane);
                                            }

                                            m_Planes[trackableId] = uPlane;
                                        }

                                        // Look for removed planes
                                        foreach (var kvp in m_Planes)
                                        {
                                            var trackableId = kvp.Key;
                                            if (!s_CurrentSet.Contains(trackableId))
                                            {
                                                removed.Add(trackableId);
                                            }
                                        }

                                        foreach (var trackableId in removed)
                                        {
                                            m_Planes.Remove(trackableId);
                                        }

                                        using (added)
                                        using (updated)
                                        using (removed)
                                        {
                                            var changes = new TrackableChanges<BoundedPlane>(
                                                added.Length,
                                                updated.Length,
                                                removed.Length,
                                                allocator);

                                            added.CopyTo(changes.added);
                                            updated.CopyTo(changes.updated);
                                            removed.CopyTo(changes.removed);

                                            return changes;
                                        }
                                    }
                                }
                            case MLResult.Code.Pending:
                                {
                                    return default;
                                }
                            default:
                                {
                                    m_QueryHandle = BeginNewQuery();
                                    return default;
                                }
                        }
                    }
                    finally
                    {
                        mlPlanes.Dispose();
                    }
                }
            }

            ulong BeginNewQuery()
            {
                // We hit the max, so increase for next time
                if (!Extensions.QuerySet && m_MaxResults == m_LastNumResults)
                    m_MaxResults = m_MaxResults * 3 / 2;

                var query = new Extensions.MLPlanesQuery(defaultPlanesQuery);
                if (Extensions.QuerySet)
                    m_MaxResults = query.max_results;

                ulong queryHandle;
                var result = NativeBindings.MLPlanesQueryBegin(m_PlanesTracker, in query, out queryHandle);

                if (!MLResult.IsOK(result))
                {
                    return Native.MagicLeapNativeBindings.InvalidHandle;
                }

                m_CurrentPlaneDetectionMode = m_RequestedPlaneDetectionMode;

                return queryHandle;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void RegisterDescriptor()
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            XRPlaneSubsystemDescriptor.Create(new XRPlaneSubsystemDescriptor.Cinfo
            {
                id = LuminXrProvider.PlanesSubsystemId,
#if UNITY_2020_2_OR_NEWER
                providerType = typeof(PlanesSubsystem.MagicLeapProvider),
                subsystemTypeOverride = typeof(PlanesSubsystem),
#else
                subsystemImplementationType = typeof(PlanesSubsystem),
#endif
                supportsVerticalPlaneDetection = true,
                supportsArbitraryPlaneDetection = true,
                supportsBoundaryVertices = true,
                supportsClassification = true
            });
#endif
        }

        internal class NativeBindings : MagicLeapNativeBindings
        {
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLPlanesCreate(out ulong planes_tracker);

            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLPlanesDestroy(ulong planes_tracker);

            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLPlanesQueryBegin(ulong planes_tracker, in Extensions.MLPlanesQuery query, out ulong request_handle);

            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern unsafe MLResult.Code MLPlanesQueryGetResultsWithBoundaries(ulong planes_tracker, ulong planes_query, Extensions.MLPlane* out_results, out uint out_num_results, ref Extensions.MLPlaneBoundariesList out_boundaries);

            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLPlanesReleaseBoundariesList(ulong planes_tracker, ref Extensions.MLPlaneBoundariesList plane_boundaries);
        }
    }
}
