// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
//
// Copyright (c) 2019-present, Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Developer Agreement, located
// here: https://auth.magicleap.com/terms/developer
//
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;

    public partial class InputSubsystem
    {
        public static partial class Extensions
        {
            public static class MLHandTracking
            {
                /// <summary>
                /// The max number of key points to track.
                /// </summary>
                public const int MaxKeyPoints = 24;

                /// <summary>
                /// Represents if a hand is the right or left hand.
                /// </summary>
                public enum HandType
                {
                    /// <summary>
                    /// Left hand.
                    /// </summary>
                    Left,

                    /// <summary>
                    /// Right hand.
                    /// </summary>
                    Right
                }

                public static void StartTracking()
                {
#if UNITY_ANDROID
                    LuminXrProviderNativeBindings.StartHandTracking();
#endif
                }

                /// <summary>
                /// By default the keypoints data is updated twice. To turn this off
                /// set enable to false to potentially improve performance. This is not
                /// recommended if keypoints are visual in the app as it will
                /// significantly decrease the smoothness of visuals.
                /// </summary>
                public static void SetPreRenderHandUpdate(bool enable = true)
                {
#if UNITY_MAGICLEAP || UNITY_ANDROID
                    NativeBindings.SetPreRenderPoseUpdate(enable);
#endif
                }

                public static bool TryGetKeyPointsMask(InputDevice handDevice, out bool[] keyPointsMask) => NativeBindings.TryGetKeyPointsMask(handDevice, out keyPointsMask);

                internal static class NativeBindings
                {
                    private static byte[] allocatedKeyPointsMaskData = new byte[Marshal.SizeOf<NativeBindings.KeyPointsMask>()];

                    public static bool TryGetKeyPointsMask(InputDevice handDevice, out bool[] keyPointsMask)
                    {
                        if (!handDevice.TryGetFeatureValue(InputSubsystem.Extensions.DeviceFeatureUsages.Hand.KeyPointsMask, allocatedKeyPointsMaskData))
                            goto Failure;

                        try
                        {
                            IntPtr ptr = Marshal.AllocHGlobal(allocatedKeyPointsMaskData.Length);
                            Marshal.Copy(allocatedKeyPointsMaskData, 0, ptr, allocatedKeyPointsMaskData.Length);
                            var nativeStruct = Marshal.PtrToStructure<NativeBindings.KeyPointsMask>(ptr);
                            Marshal.FreeHGlobal(ptr);
                            keyPointsMask = nativeStruct.Mask;
                            return true;
                        }

                        catch (Exception e)
                        {
                            Debug.LogError("TryGetKeyPointsMask failed with the exception: " + e);
                            goto Failure;
                        }

                    Failure:
                        keyPointsMask = new NativeBindings.KeyPointsMask().Mask;
                        return false;
                    }

#if UNITY_MAGICLEAP || UNITY_ANDROID
                    /// <summary>
                    /// Native call for pre render Keypoints update.
                    /// </summary>
                    /// <param name="enable">bool to determine if pre render pose update should happen.</param>
                    [DllImport(LuminXrProviderNativeBindings.LuminXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
                    public static extern void SetPreRenderPoseUpdate(bool enable);
#endif

                    public readonly struct KeyPointsMask
                    {
                        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.I1, SizeConst = (int)MaxKeyPoints)]
                        public readonly bool[] Mask;
                    }
                }
            }
        }
    }
}
