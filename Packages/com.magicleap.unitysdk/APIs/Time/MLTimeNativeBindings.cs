// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLTimeNativeBindings.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.Runtime.InteropServices;
using UnityEngine.XR.MagicLeap.Native;

namespace UnityEngine.XR.MagicLeap
{
    /// <summary>
    /// MLTime description goes here.
    /// </summary>
    public partial class MLTime
    {
        /// <summary>
        /// See ml_time.h for additional comments.
        /// </summary>
        internal class NativeBindings : MagicLeapNativeBindings
        {
            /// <summary>
            /// Mirrors time.h's timespec struct, which is generated by clock_gettime()
            /// Used to store and convert the input/output of the NativeBindings methods
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct TimeSpec
            {
                public long Seconds;

                public long Nanoseconds;
            }

            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLTimeConvertSystemTimeToMLTime(IntPtr timeSpec, out long mlTime);

            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLTimeConvertMLTimeToSystemTime(long mlTime, IntPtr timeSpec);
        }
    }
}
