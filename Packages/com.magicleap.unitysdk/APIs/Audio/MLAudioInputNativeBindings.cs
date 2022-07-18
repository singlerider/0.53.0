// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file = "MLAudioInputNativeBindings.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

#if UNITY_MAGICLEAP || UNITY_ANDROID

// Disable warnings about missing documentation for native interop.
#pragma warning disable 1591
namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Manages Audio.
    /// </summary>
    public sealed partial class MLAudioInput
    {
        /// <summary>
        /// See ml_audio.h for additional comments.
        /// </summary>
        private class NativeBindings : Native.MagicLeapNativeBindings
        {
            /// <summary>
            /// The callback that occurs when the mute state changes for the microphone.
            /// </summary>
            /// <param name="muted">The mute state of the microphone.</param>
            /// <param name="callback">A pointer to the callback.</param>
            public delegate void MLAudioMicMuteCallback([MarshalAs(UnmanagedType.I1)] bool muted, IntPtr callback);

            /// <summary>
            /// Sets the mute state of the microphone.
            /// </summary>
            /// <param name="muted">The mute state of the microphone.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
            /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if <c>AudioCaptureMic</c> permission is denied.
            /// MLResult.Result will be <c>MLResult.Code.NotImplemented</c>.
            /// </returns>
            [DllImport(AudioPlayerDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLAudioSetMicMute([MarshalAs(UnmanagedType.I1)] bool muted);

            /// <summary>
            /// Gets the mute state of the microphone.
            /// </summary>
            /// <param name="isMuted">The mute state of the microphone.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if input parameter is invalid.
            /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if <c>AudioCaptureMic</c> permission is denied.
            /// MLResult.Result will be <c>MLResult.Code.AllocFailed</c> if failed due to internal error.
            /// MLResult.Result will be <c>MLResult.Code.NotImplemented</c>.
            /// </returns>
            [DllImport(AudioPlayerDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLAudioIsMicMuted([MarshalAs(UnmanagedType.I1)] out bool isMuted);

            /// <summary>
            /// Register a callback for when the mute state changes for the microphone.
            /// </summary>
            /// <param name="callback">A pointer to the callback.</param>
            /// <param name="data">A generic data pointer passed back to the callback.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
            /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if <c>AudioCaptureMic</c> permission is denied.
            /// MLResult.Result will be <c>MLResult.Code.AllocFailed</c> if failed due to internal error.
            /// MLResult.Result will be <c>MLResult.Code.NotImplemented</c>.
            /// </returns>
            [DllImport(AudioPlayerDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLAudioSetMicMuteCallback(MLAudioMicMuteCallback callback, IntPtr data);

            /// <summary>
            /// Gets the result string for a MLResult.Code.
            /// </summary>
            /// <param name="result">The MLResult.Code to be requested.</param>
            /// <returns>A pointer to the result string.</returns>
            [DllImport(AudioPlayerDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLAudioGetResultString(MLResult.Code result);
        }
    }
}

#endif
