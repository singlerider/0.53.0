// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLWebRTCFrameNativeBindings.cs" company="Magic Leap, Inc">
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
    /// MLWebRTC class contains the API to interface with the
    /// WebRTC C API.
    /// </summary>
    public partial class MLWebRTC
    {
        /// <summary>
        /// Class that represents a source used by the MLWebRTC API.
        /// </summary>
        public partial class VideoSink
        {
            /// <summary>
            /// Struct representing a captured camera frame.
            /// </summary>
            public partial struct Frame
            {
                /// <summary>
                /// Native bindings for the MLWebRTC.Frame struct. 
                /// </summary>
                internal class NativeBindings
#if UNITY_MAGICLEAP || UNITY_ANDROID
                    : MagicLeapNativeBindings
#endif
                {
#if UNITY_MAGICLEAP || UNITY_ANDROID
                    /// <summary>
                    /// Gets frame data.
                    /// </summary>
                    /// <param name="frameHandle">The handle to the frame to query.</param>
                    /// <param name="frame">Pointer to the frame data.</param>
                    /// <returns>
                    /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the dimensions were successfully obtained.
                    /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if necessary permission is missing.
                    /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                    /// </returns>
                    [DllImport(MLWebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                    public static extern MLResult.Code MLWebRTCFrameGetData(ulong frameHandle, ref MLWebRTCFrame frame);
#endif

                    /// <summary>
                    /// Representation of the native frame structure.
                    /// </summary>
                    [StructLayout(LayoutKind.Sequential)]
                    public struct MLWebRTCFrame
                    {
                        /// <summary>
                        /// Version of this structure.
                        /// </summary>
                        public uint Version;

                        /// <summary>
                        /// Frame data, can be CPU based image planes or
                        /// gpu based native surface. Check MLWebRTCFrame.Format
                        /// to confirm how this memory should be read.
                        /// </summary>
                        public MLWebRTCFrameUnion FrameData;

                        /// <summary>
                        /// Timestamp of the frame.
                        /// </summary>
                        public ulong TimeStamp;

                        /// <summary>
                        /// Output format that the image planes will be in.
                        /// </summary>
                        public MLWebRTC.VideoSink.Frame.OutputFormat Format;

                        public static MLWebRTCFrame Create(OutputFormat format)
                        {
                            MLWebRTCFrame frameNative = new MLWebRTCFrame();
                            frameNative.Version = 1;
                            frameNative.Format = format;
                            frameNative.FrameData = new MLWebRTCFrameUnion();
                            switch (format)
                            {
                                case OutputFormat.YUV_420_888:
                                    {
                                        frameNative.FrameData.ImageFrameInfo = new MLWebRTCImageFrameInfo();
                                        frameNative.FrameData.ImageFrameInfo.PlaneCount = (byte)NativeImagePlanesLength[format];
                                        // TODO : should we assign frameNative.FrameData.ImageFrameInfo.ImagePlanes from the circular buffer here?
                                        break;
                                    }
                                case OutputFormat.RGBA_8888:
                                    {
                                        frameNative.FrameData.ImageFrameInfo = new MLWebRTCImageFrameInfo();
                                        frameNative.FrameData.ImageFrameInfo.PlaneCount = (byte)NativeImagePlanesLength[format];
                                        // TODO : should we assign frameNative.FrameData.ImageFrameInfo.ImagePlanes from the circular buffer here?
                                        break;
                                    }
                                case OutputFormat.NativeBuffer:
                                    {
                                        frameNative.FrameData.NativeFrameInfo = new MLWebRTCNativeFrameInfo();
                                        if (frameNative.FrameData.NativeFrameInfo.Transform == null)
                                        {
                                            frameNative.FrameData.NativeFrameInfo.Transform = new float[16];
                                        }
                                        break;
                                    }
                            }
                            return frameNative;
                        }

                        /// <summary>
                        /// Creates and returns an initialized version of this struct from a MLWebRTC.VideoSink.Frame object.
                        /// </summary>
                        /// <param name="frame">The frame object to use for initializing.</param>
                        /// <returns>An initialized version of this struct.</returns>
                        public static MLWebRTCFrame Create(MLWebRTC.VideoSink.Frame frame)
                        {
                            MLWebRTCFrame frameNative = Create(frame.Format);
                            frameNative.TimeStamp = frame.TimeStampUs;
                            if (frame.Format == OutputFormat.NativeBuffer)
                            {
                                frameNative.FrameData.NativeFrameInfo.Width = frame.NativeFrame.Width;
                                frameNative.FrameData.NativeFrameInfo.Height = frame.NativeFrame.Height;
                                frameNative.FrameData.NativeFrameInfo.SurfaceHandle = frame.NativeFrame.SurfaceHandle;
                                frameNative.FrameData.NativeFrameInfo.NativeBufferHandle = frame.NativeFrame.NativeBufferHandle;
                                Array.Copy(frame.NativeFrame.Transform, frameNative.FrameData.NativeFrameInfo.Transform, frameNative.FrameData.NativeFrameInfo.Transform.Length);
                            }
                            else
                            {
                                frameNative.FrameData.ImageFrameInfo.ImagePlanes = nativeImagePlanesBuffer.Get();
                                for (int i = 0; i < frameNative.FrameData.ImageFrameInfo.ImagePlanes.Length; ++i)
                                {
                                    frameNative.FrameData.ImageFrameInfo.ImagePlanes[i].Data = frame.ImagePlanes[i];
                                }
                            }

                            return frameNative;
                        }
                    }

                    /// <summary>
                    /// !!!!!!!!!!!!!!! BEWARE !!!!!!!!!!!!!!!
                    /// Explicit size of the union specified here!
                    /// Needs to ALWAYS match the c-api.
                    /// </summary>
                    [StructLayout(LayoutKind.Explicit, Size = 104)]
                    public struct MLWebRTCFrameUnion
                    {
                        [FieldOffset(0)]
                        public MLWebRTCImageFrameInfo ImageFrameInfo;

                        [FieldOffset(0)]
                        public MLWebRTCNativeFrameInfo NativeFrameInfo;
                    }

                    [StructLayout(LayoutKind.Sequential)]
                    public struct MLWebRTCImageFrameInfo
                    {
                        /// <summary>
                        /// Number of valid image planes in the ImagePlanes array.
                        /// </summary>
                        public byte PlaneCount;

                        /// <summary>
                        /// ImagePlanes array containing the image data.
                        /// </summary>
                        [MarshalAs(UnmanagedType.ByValArray, SizeConst = PlaneInfo.MaxImagePlanes)]
                        public ImagePlaneInfoNative[] ImagePlanes;
                    }

                    [StructLayout(LayoutKind.Sequential)]
                    public struct MLWebRTCNativeFrameInfo
                    {
                        public uint Width;

                        public uint Height;

                        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
                        public float[] Transform;

                        public ulong SurfaceHandle;

                        public ulong NativeBufferHandle;
                    }

                    /// <summary>
                    /// Buffer for native image plane arrays.
                    /// </summary>
                    static CircularBuffer<ImagePlaneInfoNative[]> nativeImagePlanesBuffer = CircularBuffer<ImagePlaneInfoNative[]>.Create(new ImagePlaneInfoNative[PlaneInfo.MaxImagePlanes], new ImagePlaneInfoNative[PlaneInfo.MaxImagePlanes], new ImagePlaneInfoNative[PlaneInfo.MaxImagePlanes]);

                    /// <summary>
                    /// Representation of the native image plane structure.
                    /// </summary>
                    [StructLayout(LayoutKind.Sequential)]
                    public struct ImagePlaneInfoNative
                    {
                        /// <summary>
                        /// Width of the image plane.
                        /// </summary>
                        public uint Width;

                        /// <summary>
                        /// Height of the image plane.
                        /// </summary>
                        public uint Height;

                        /// <summary>
                        /// The stride of the image plane, representing how many bytes one row of the image plane contains.
                        /// </summary>
                        public uint Stride;

                        /// <summary>
                        /// The bytes per pixel of the image plane.
                        /// </summary>
                        public uint BytesPerPixel;

                        /// <summary>
                        /// Data of the image plane.
                        /// </summary>
                        public IntPtr ImageDataPtr;

                        /// <summary>
                        /// Size of the image plane.
                        /// </summary>
                        public uint Size;

                        /// <summary>
                        /// Sets data from an MLWebRTC.VideoSink.Frame.ImagePlane object.
                        /// </summary>
                        public MLWebRTC.VideoSink.Frame.PlaneInfo Data
                        {
                            set
                            {
                                this.Width = value.Width;
                                this.Height = value.Height;
                                this.Stride = value.Stride;
                                this.BytesPerPixel = value.BytesPerPixel;
                                this.ImageDataPtr = value.DataPtr;
                                this.Size = value.Size;
                            }
                        }
                    }
                }
            }
        }
    }
}
