// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLWebRTCAppDefinedVideoSourceNativeBindings.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Runtime.InteropServices;
#if UNITY_MAGICLEAP || UNITY_ANDROID
    using UnityEngine.XR.MagicLeap.Native;
#endif

    /// <summary>
    /// MLWebRTC class contains the API to interface with the
    /// WebRTC C API.
    /// </summary>
    public partial class MLWebRTC
    {
        /// <summary>
        /// Class that represents a source used by the MLWebRTC API.
        /// </summary>
        public partial class AppDefinedSource
        {
            /// <summary>
            /// Native bindings for the MLWebRTC.AppDefinedVideoSource class. 
            /// </summary>
            internal class NativeBindings
            {
                /// <summary>
                /// A delegate that describes the requirements of the OnSetEnabled callback.
                /// </summary>
                /// <param name="enabled">True if the source was enabled.</param>
                /// <param name="context">Pointer to a context object.</param>
                public delegate void OnSetEnabledDelegate([MarshalAs(UnmanagedType.I1)] bool enabled, IntPtr context);

                /// <summary>
                /// A delegate that describes the requirements of the OnDestroyed callback.
                /// </summary>
                /// <param name="context">Pointer to a context object.</param>
                public delegate void OnDestroyedDelegate(IntPtr context);

                /// <summary>
                /// The native representation of the MLWebRTC data channel callback events.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct MLWebRTCAppDefinedSourceEventCallbacks
                {
                    /// <summary>
                    /// Version of the struct.
                    /// </summary>
                    public uint Version;

                    /// <summary>
                    /// Version of the struct.
                    /// </summary>
                    public IntPtr Context;

                    /// <summary>
                    /// OnSetEnabled event.
                    /// </summary>
                    public OnSetEnabledDelegate OnSetEnabled;

                    /// <summary>
                    /// OnDestroyed event.
                    /// </summary>
                    public OnDestroyedDelegate OnDestroyed;

                    /// <summary>
                    /// Factory method used to create a new MLWebRTCAppDefinedVideoSourceEventCallbacks object.
                    /// </summary>
                    /// <param name="context">Pointer to the context object to use for the callbacks.</param>
                    /// <returns>An MLWebRTCAppDefinedVideoSourceEventCallbacks object with the given handle.</returns>
                    public static MLWebRTCAppDefinedSourceEventCallbacks Create(IntPtr context, OnSetEnabledDelegate onSetEnabled, OnDestroyedDelegate onDestroyed)
                    {
                        MLWebRTCAppDefinedSourceEventCallbacks callbacks = new MLWebRTCAppDefinedSourceEventCallbacks
                        {
                            Version = 1,
#if UNITY_MAGICLEAP || UNITY_ANDROID
                            OnSetEnabled = onSetEnabled,
                            OnDestroyed = onDestroyed,
#endif
                            Context = context
                        };
                        return callbacks;
                    }
                }
            }
        }
    }
}
