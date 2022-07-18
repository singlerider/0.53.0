// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLMediaPlayerTrack.cs" company="Magic Leap, Inc">
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
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
#if UNITY_MAGICLEAP || UNITY_ANDROID
    using UnityEngine.XR.MagicLeap.Native;
#endif

    /// <summary>
    /// MLMedia APIs.
    /// </summary>
    public partial class MLMedia
    {
        /// <summary>
        /// Media player script that allows playback of a streaming video (either from file or web URL)
        /// This script will update the main texture parameter of the Renderer attached as a sibling
        /// with the video frame from playback. Audio is also handled through this class and will
        /// playback audio from the file.
        /// </summary>
        public partial class Player
        {
            /// <summary>
            /// Track from the prepared source that can be selected by the media player.
            /// </summary>
            public partial class Track
            {
#if UNITY_MAGICLEAP || UNITY_ANDROID
                internal class NativeBindings : MagicLeapNativeBindings
                {
                    public const uint MAX_KEY_STRING_SIZE = 64;

                    /// <summary>
                    /// Internal DLL used to free strings.
                    /// </summary>
                    private const string CUtilsDLL = "ml_c_utils";
                    
                    public static string GetTrackLanguage(ulong mediaPlayerHandle, uint trackIndex)
                    {
                        string language = string.Empty;
                        IntPtr stringPtr = IntPtr.Zero;
                        MLResult.Code resultCode = MLMediaPlayerGetTrackLanguage(mediaPlayerHandle, trackIndex, ref stringPtr);
                        MLResult.DidNativeCallSucceed(resultCode, "MLMediaPlayerGetTrackLanguage");

                        if (stringPtr != IntPtr.Zero)
                        {
                            language = Marshal.PtrToStringAnsi(stringPtr);
                            FreeUnmanagedMemory(stringPtr);
                        }

                        return language;
                    }
                    
                    /// <summary>
                    /// Get the language of a track.
                    /// </summary>
                    [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                    public static extern MLResult.Code MLMediaPlayerGetTrackLanguage(ulong mediaPlayerHandle, uint trackIndex, ref IntPtr OutTrackLanguage);

                    /// <summary>
                    /// Get the type of a track.
                    /// </summary>
                    [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                    public static extern MLResult.Code MLMediaPlayerGetTrackType(ulong mediaPlayerHandle, uint trackIndex, out Type trackType);

                    /// <summary>
                    /// Get the Media Format of a track.
                    /// </summary>
                    [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                    public static extern MLResult.Code MLMediaPlayerGetTrackMediaFormat(ulong mediaPlayerHandle, uint trackIndex,  out ulong formatHandle);

                    /// <summary>
                    /// Get the Media Format of a track.
                    /// </summary>
                    [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                    public static extern MLResult.Code MLMediaFormatGetKeyString(ulong formatHandle, [MarshalAs(UnmanagedType.LPStr)] string formatKey, IntPtr stringPtr);

                    /// <summary>
                    /// Get the Media Format of a track.
                    /// </summary>
                    [DllImport(CUtilsDLL, CallingConvention = CallingConvention.Cdecl)]
                    public static extern MLResult.Code FreeUnmanagedMemory(IntPtr mediaStringPtr);

                }
#endif
            }
        }
    }
}