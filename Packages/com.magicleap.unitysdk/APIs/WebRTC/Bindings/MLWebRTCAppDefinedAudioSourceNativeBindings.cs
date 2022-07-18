// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLWebRTCappDefinedAudioSourceNativeBindings.cs" company="Magic Leap, Inc">
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
        public partial class AppDefinedAudioSource
        {
            /// <summary>
            /// Native bindings for the MLWebRTC.AppDefinedAudioSource class. 
            /// </summary>
            internal new class NativeBindings
#if UNITY_MAGICLEAP || UNITY_ANDROID
                : MagicLeapNativeBindings
#endif
            {
                /// <summary>
                /// Initialized a given AppDefinedAudioSource object and sets it's callbacks.
                /// </summary>
                /// <param name="appDefinedAudioSource">The AppDefinedAudioSource object to initialize.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the connection was successfully created.
                /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if necessary permission is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                public static MLResult.Code InitializeAppDefinedAudioSource(MLWebRTC.AppDefinedAudioSource appDefinedAudioSource)
                {
#if UNITY_MAGICLEAP || UNITY_ANDROID
                    appDefinedAudioSource.TrackType = Type.Audio;
                    appDefinedAudioSource.IsLocal = true;
                    appDefinedAudioSource.gcHandle = GCHandle.Alloc(appDefinedAudioSource);
                    IntPtr gcHandlePtr = GCHandle.ToIntPtr(appDefinedAudioSource.gcHandle);

                    AppDefinedSource.NativeBindings.MLWebRTCAppDefinedSourceEventCallbacks callbacks = AppDefinedSource.NativeBindings.MLWebRTCAppDefinedSourceEventCallbacks.Create(gcHandlePtr, OnSetEnabled, OnDestroyed);

                    ulong appDefinedAudioSourceHandle = MagicLeapNativeBindings.InvalidHandle;
                    var sourceParams = Source.NativeBindings.MLWebRTCAppDefinedSourceParams.Create(appDefinedAudioSource.Id, callbacks);
                    MLResult.Code resultCode = Source.NativeBindings.MLWebRTCSourceCreateAppDefinedAudioSourceEx(ref sourceParams, out appDefinedAudioSourceHandle);
                    appDefinedAudioSource.Handle = appDefinedAudioSourceHandle;
                    if (!MLResult.IsOK(resultCode))
                    {
                        appDefinedAudioSource.gcHandle.Free();
                        Marshal.FreeHGlobal(sourceParams.Callbacks);
                    }

                    return resultCode;
#else
                    appDefinedAudioSource = null;
                    return MLResult.Code.Ok;
#endif
                }

                public static MLResult.Code PushData(ulong sourceHandle, byte[] audioBuffer, in MLAudioOutput.BufferFormat audioBufferFormat)
                {
#if UNITY_MAGICLEAP || UNITY_ANDROID
                    MLAudioOutput.NativeBindings.MLAudioBuffer audioBufferNative = new MLAudioOutput.NativeBindings.MLAudioBuffer();
                    audioBufferNative.Size = (uint)audioBuffer.Length;
                    // TODO : pool this memory since audio buffers should be expected to be submitted every frame?
                    audioBufferNative.Ptr = Marshal.AllocHGlobal(audioBuffer.Length);
                    Marshal.Copy(audioBuffer, 0, audioBufferNative.Ptr, audioBuffer.Length);

                    MLAudioOutput.NativeBindings.MLAudioBufferFormat audioBufferFormatNative = new MLAudioOutput.NativeBindings.MLAudioBufferFormat(audioBufferFormat);

                    MLResult.Code resultCode = MLWebRTCSourceAppDefinedAudioSourcePushData(sourceHandle, in audioBufferNative, in audioBufferFormatNative);

                    Marshal.FreeHGlobal(audioBufferNative.Ptr);

                    return resultCode;
#else
                    return MLResult.Code.Ok;
#endif
                }

                public static MLResult.Code PushData(ulong sourceHandle, float[] audioBuffer, in MLAudioOutput.BufferFormat audioBufferFormat)
                {
#if UNITY_MAGICLEAP || UNITY_ANDROID
                    MLAudioOutput.NativeBindings.MLAudioBuffer audioBufferNative = new MLAudioOutput.NativeBindings.MLAudioBuffer();
                    int numBytes = (audioBuffer.Length * sizeof(float));
                    audioBufferNative.Size = (uint)numBytes;
                    // TODO : pool this memory since audio buffers should be expected to be submitted every frame?
                    audioBufferNative.Ptr = Marshal.AllocHGlobal(numBytes);
                    Marshal.Copy(audioBuffer, 0, audioBufferNative.Ptr, audioBuffer.Length);

                    MLAudioOutput.NativeBindings.MLAudioBufferFormat audioBufferFormatNative = new MLAudioOutput.NativeBindings.MLAudioBufferFormat(audioBufferFormat);

                    MLResult.Code resultCode = MLWebRTCSourceAppDefinedAudioSourcePushData(sourceHandle, in audioBufferNative, in audioBufferFormatNative);

                    Marshal.FreeHGlobal(audioBufferNative.Ptr);

                    return resultCode;
#else
                    return MLResult.Code.Ok;
#endif
                }

#if UNITY_MAGICLEAP || UNITY_ANDROID
                /// <summary>
                /// Callback that is invoked when the source has been enabled or disabled. This callback will be called on the main thread.
                /// </summary>
                /// <param name="enabled">True if the source was enabled.</param>
                /// <param name="context">Pointer to a context object.</param>
                [AOT.MonoPInvokeCallback(typeof(AppDefinedSource.NativeBindings.OnSetEnabledDelegate))]
                private static void OnSetEnabled(bool enabled, IntPtr context)
                {
                    GCHandle gcHandle = GCHandle.FromIntPtr(context);
                    AppDefinedAudioSource audioSource = gcHandle.Target as AppDefinedAudioSource;
                    audioSource.isEnabled = enabled;
                    audioSource?.OnSourceSetEnabled(enabled);
                }

                /// <summary>
                /// Callback that is invoked when the source has been destroyed. This callback will be called on the main thread.
                /// </summary>
                /// <param name="context">Pointer to a context object.</param>
                [AOT.MonoPInvokeCallback(typeof(AppDefinedSource.NativeBindings.OnDestroyedDelegate))]
                private static void OnDestroyed(IntPtr context)
                {
                    GCHandle gcHandle = GCHandle.FromIntPtr(context);
                    AppDefinedAudioSource audioSource = gcHandle.Target as AppDefinedAudioSource;
                    audioSource?.OnSourceDestroy();
                }

                /// <summary>
                /// Creates the local source that links to the user's camera and mic.
                /// </summary>
                /// <param name="sourceHandle">The handle to the local source to return to the caller.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the local source was successfully created.
                /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if necessary permission is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(MLWebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCSourceCreateAppDefinedAudioSource(in AppDefinedSource.NativeBindings.MLWebRTCAppDefinedSourceEventCallbacks callbacks, out ulong sourceHandle);

                /// <summary>
                /// Creates the local source that links to the user's camera and mic.
                /// </summary>
                /// <param name="sourceHandle">The handle to the local source to return to the caller.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the local source was successfully created.
                /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if necessary permission is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(MLWebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCSourceAppDefinedAudioSourcePushData(ulong sourceHandle, in MLAudioOutput.NativeBindings.MLAudioBuffer audioBuffer, in MLAudioOutput.NativeBindings.MLAudioBufferFormat audioBufferFormat);
#endif
            }
        }
    }
}