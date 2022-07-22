// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="SessionSubsystem.cs" company="Magic Leap, Inc">
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
using Unity.Collections;
using UnityEngine.Scripting;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.MagicLeap.Internal;
using UnityEngine.XR.Management;

namespace UnityEngine.XR.MagicLeap
{
    /// <summary>
    /// The Magic Leap implementation of the <c>XRSessionSubsystem</c>. Do not create this directly.
    /// Use <c>SessionSubsystemDescriptor.Create()</c> instead.
    /// </summary>
    [Preserve]
    public sealed class SessionSubsystem : XRSessionSubsystem
    {
#if !UNITY_2020_2_OR_NEWER
        protected override Provider CreateProvider() => new MagicLeapProvider();
#endif

        class MagicLeapProvider : Provider
        {
            public override Promise<SessionAvailability> GetAvailabilityAsync()
            {
                var availability =
#if UNITY_MAGICLEAP || UNITY_ANDROID
                SessionAvailability.Installed | SessionAvailability.Supported;
#else
                SessionAvailability.None;
#endif
                return Promise<SessionAvailability>.CreateResolvedPromise(availability);
            }

            public override TrackingState trackingState
            {
                get
                {
                    var device = InputDevices.GetDeviceAtXRNode(XRNode.CenterEye);
                    if (device.TryGetFeatureValue(CommonUsages.trackingState, out InputTrackingState inputTrackingState))
                    {
                        if (inputTrackingState == InputTrackingState.None)
                            return TrackingState.None;
                        else if (inputTrackingState == (InputTrackingState.Position | InputTrackingState.Rotation))
                            return TrackingState.Tracking;
                        else
                            return TrackingState.Limited;
                    }
                    else
                    {
                        return TrackingState.None;
                    }
                }
            }

            public override Feature requestedFeatures => SubsystemFeatures.requestedFeatures;

            public override NativeArray<ConfigurationDescriptor> GetConfigurationDescriptors(Allocator allocator)
                => SubsystemFeatures.AcquireConfigurationDescriptors(allocator);

            public override Feature requestedTrackingMode
            {
                get => SubsystemFeatures.requestedFeatures.Intersection(Feature.AnyTrackingMode);
                set
                {
                    SubsystemFeatures.SetFeatureRequested(Feature.AnyTrackingMode, false);
                    SubsystemFeatures.SetFeatureRequested(value, true);
                }
            }

            /// <summary>
            /// The current tracking mode feature flag.
            /// </summary>
            /// <remarks>
            /// Magic Leap will always try to use 6DoF tracking but will automatically switch to
            /// 3DoF if it doesn't have a sufficient tracking environment. This will report which
            /// of the two modes is currently active and
            /// <c>UnityEngine.XR.ARSubsystems.Feature.None</c> otherwise.
            /// </remarks>
            public override Feature currentTrackingMode
            {
                get
                {
                    switch (trackingState)
                    {
                        case TrackingState.Tracking:
                            return Feature.PositionAndRotation;
                        case TrackingState.Limited:
                            return Feature.RotationOnly;
                        default:
                            return Feature.None;
                    }
                }
            }

            public override void Update(XRSessionUpdateParams updateParams, Configuration configuration)
            {
                // Magic Leap supports almost everything working at the same time except Point Clouds and Meshing
                if (configuration.features.HasFlag(Feature.Meshing | Feature.PointCloud))
                {
                    /// TODO (5/26/2020): Move MLSpatialMapper specific features to shared XRMeshSubsystem extensions
                    // Currently, the MeshingSubsystemComponent is required to do PointClouds on magic leap.  So
                    // if meshing is detected at all then simply request a start to the subsystem because it will be
                    // handled either by ARMeshManager or the MeshingSubsystemComponent.
                    var loader = (MagicLeapLoader)XRGeneralSettings.Instance.Manager.activeLoader;
                    if (loader.meshSubsystem != null && !loader.meshSubsystem.running && configuration.features.HasFlag(Feature.Meshing))
                    {
                        loader.StartMeshSubsystem();
                        SubsystemFeatures.SetCurrentFeatureEnabled(Feature.Meshing, true);
                    }
                }

                SubsystemFeatures.currentFeatures = configuration.features;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void RegisterDescriptor()
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            XRSessionSubsystemDescriptor.RegisterDescriptor(new XRSessionSubsystemDescriptor.Cinfo
            {
                id = LuminXrProvider.SessionSubsystemId,
#if UNITY_2020_2_OR_NEWER
                providerType = typeof(SessionSubsystem.MagicLeapProvider),
                subsystemTypeOverride = typeof(SessionSubsystem),
#else
                subsystemImplementationType = typeof(SessionSubsystem),
#endif
                supportsInstall = false
            });
#endif
        }
    }
}
