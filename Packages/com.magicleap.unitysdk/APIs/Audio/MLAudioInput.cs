// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file = "MLAudioInput.cs" company="Magic Leap, Inc">
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
    /// Manages Audio.
    /// </summary>
    public sealed partial class MLAudioInput : MLAutoAPISingleton<MLAudioInput>
    {
#if UNITY_MAGICLEAP || UNITY_ANDROID
        /// <summary>
        /// The mute state of the microphone.
        /// </summary>
        private bool isMicrophoneMuted;

        /// <summary>
        /// The delegate for the microphone mute changed event.
        /// </summary>
        /// <param name="muted">The new mute state of the microphone.</param>
        public delegate void OnMicrophoneMuteChangedDelegate(bool muted);

        /// <summary>
        /// Raised whenever the global microphone mute gets changed.
        /// </summary>
        public static event OnMicrophoneMuteChangedDelegate OnMicrophoneMuteChanged = delegate { };

        /// <summary>
        /// Gets or sets a value indicating whether the microphone is muted.
        /// </summary>
        public static bool MicrophoneMuted
        {
            get
            {
                return Instance.isMicrophoneMuted;
            }

            set
            {
                Instance.InternalSetMicMute(value);
            }
        }


        /// <summary>
        /// Gets the result string for a MLResult.Code.
        /// </summary>
        /// <param name="result">The MLResult.Code to be requested.</param>
        /// <returns>A pointer to the result string.</returns>
        internal static IntPtr GetResultString(MLResult.Code result)
        {
            nativeGetResultStringPerfMarker.Begin();
            IntPtr ptr = NativeBindings.MLAudioGetResultString(result);
            nativeGetResultStringPerfMarker.End();
            return ptr;
        }

#if !DOXYGEN_SHOULD_SKIP_THIS
        /// <summary>
        /// Called by MLAutoAPISingleton to start the API
        /// </summary>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if a parameter is invalid.
        /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if <c>AudioCaptureMic</c> permission is denied.
        /// MLResult.Result will be <c>MLResult.Code.AudioNotImplemented</c> if the function is not implemented.
        /// </returns>
        protected override MLResult.Code StartAPI()
        {
            startAPIPerfMarker.Begin();
            MLResult.Code resultCode;

            // Microphone Muted Callback
            resultCode = this.RegisterOnMicrophoneMuteCallback();
            if (resultCode != MLResult.Code.Ok)
            {
                startAPIPerfMarker.End();
                return resultCode;
            }

            // Get the initial IsMicrophoneMuted value.
            resultCode = Instance.GetMicrophoneMuted(out this.isMicrophoneMuted);
            if (resultCode != MLResult.Code.Ok)
            {
                startAPIPerfMarker.End();
                return resultCode;
            }

            startAPIPerfMarker.End();
            return resultCode;
        }
#endif // DOXYGEN_SHOULD_SKIP_THIS

        /// <summary>
        /// Called by MLAutoAPISingleton on destruction
        /// </summary>
        protected override MLResult.Code StopAPI() => this.UnregisterOnMicrophoneMuteCallback();

        /// <summary>
        /// Called every device frame
        /// </summary>
        protected override void Update()
        {
        }

        /// <summary>
        /// Handles the callback for <c>MLAudioSetMicMute</c>.
        /// </summary>
        /// <param name="isMuted">The mute state of the microphone.</param>
        /// <param name="callback">A pointer to the callback.</param>
        [AOT.MonoPInvokeCallback(typeof(NativeBindings.MLAudioMicMuteCallback))]
        private static void HandleOnMLAudioSetMicMuteCallback([MarshalAs(UnmanagedType.I1)] bool isMuted, IntPtr callback)
        {
            Instance.isMicrophoneMuted = isMuted;

            MLThreadDispatch.Call(isMuted, OnMicrophoneMuteChanged);
        }


        /// <summary>
        /// Returns the mute state of the microphone.
        /// </summary>
        /// <param name="isMuted">The mute state of the microphone.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if a parameter is invalid.
        /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if <c>AudioCaptureMic</c> permission is denied.
        /// MLResult.Result will be <c>MLResult.Code.AudioNotImplemented</c> if the function is not implemented.
        /// </returns>
        private MLResult.Code GetMicrophoneMuted(out bool isMuted)
        {
            MLResult.Code result;

            try
            {
                nativeIsMicMutedPerfMarker.Begin();
                result = NativeBindings.MLAudioIsMicMuted(out isMuted);
                nativeIsMicMutedPerfMarker.End();

                if (result != MLResult.Code.Ok)
                {
                    MLPluginLog.ErrorFormat("MLAudioInput.GetMicrophoneMuted failed to get the value. Reason: {0}", result);
                }
            }
            catch (System.DllNotFoundException)
            {
                // Exception is caught in the Singleton BaseStart().
                throw;
            }

            return result;
        }

        /// <summary>
        /// Registers a callback for the device microphone mute change event.
        /// </summary>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if a parameter is invalid.
        /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if <c>AudioCaptureMic</c> permission is denied.
        /// MLResult.Result will be <c>MLResult.Code.AudioNotImplemented</c> if the function is not implemented.
        /// </returns>
        private MLResult.Code RegisterOnMicrophoneMuteCallback()
        {
            MLResult.Code result;

            try
            {
                nativeSetMicMuteCallbackPerfMarker.Begin();
                // Attempt to register the native callback for the volume change event.
                result = NativeBindings.MLAudioSetMicMuteCallback(HandleOnMLAudioSetMicMuteCallback, IntPtr.Zero);
                nativeSetMicMuteCallbackPerfMarker.End();

                if (result != MLResult.Code.Ok)
                {
                    MLPluginLog.ErrorFormat("MLAudioInput.RegisterOnAudioSetMicMuteCallback failed to register callback. Reason: {0}", result);
                }
            }
            catch (System.DllNotFoundException)
            {
                // Exception is caught in the Singleton BaseStart().
                throw;
            }

            return result;
        }

        /// <summary>
        /// Unregisters a previously registered callback for the device microphone mute change event.
        /// </summary>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if a parameter is invalid.
        /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if <c>AudioCaptureMic</c> permission is denied.
        /// MLResult.Result will be <c>MLResult.Code.AudioNotImplemented</c> if the function is not implemented.
        /// </returns>
        private MLResult.Code UnregisterOnMicrophoneMuteCallback()
        {
            MLResult.Code result;

            try
            {
                nativeSetMicMuteCallbackPerfMarker.Begin();
                // Unregister the native callback for the microphone mute change event.
                result = NativeBindings.MLAudioSetMicMuteCallback(null, IntPtr.Zero);
                nativeSetMicMuteCallbackPerfMarker.End();

                if (result != MLResult.Code.Ok)
                {
                    MLPluginLog.ErrorFormat("MLAudioInput.UnregisterOnMicrophoneMuteCallback failed to register callback. Reason: {0}", result);
                }
            }
            catch (System.DllNotFoundException)
            {
                MLPluginLog.Error(MLResult.Code.APIDLLNotFound);
                throw;
            }

            return result;
        }

        /// <summary>
        /// Sets the mute state of the microphone.
        /// </summary>
        /// <param name="mute">The microphone mute state.</param>
        private void InternalSetMicMute(bool mute)
        {
            try
            {
                nativeSetMicMutePerfMarker.Begin();
                MLResult.Code result = NativeBindings.MLAudioSetMicMute(mute);
                nativeSetMicMutePerfMarker.End();

                if (result != MLResult.Code.Ok)
                {
                    MLPluginLog.ErrorFormat("MLAudioInput.InternalSetMicMute failed to set the value. Reason: {0}", result);
                }

                this.isMicrophoneMuted = mute;
            }
            catch (System.DllNotFoundException)
            {
                MLPluginLog.Error(MLResult.Code.APIDLLNotFound);
                throw;
            }
        }
#endif
    }
}
