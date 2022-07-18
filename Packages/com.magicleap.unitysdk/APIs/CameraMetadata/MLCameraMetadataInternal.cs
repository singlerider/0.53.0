// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLCameraMetadata.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
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

	public partial class MLCamera
	{
		/// <summary>
		/// MLCameraMetadata Summary placeholder.
		/// </summary>
		public partial class Metadata
		{

			/// <summary>
			/// Gets sensor sensitivity.
			/// </summary>
			private MLResult.Code InternalGetSensorSensitivityRequestMetadata(out int OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetSensorSensitivityRequestMetadata(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetSensorSensitivityRequestMetadata));
				return result;
#else
				OutData = 0;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Sets AE lock.
			/// </summary>
			private MLResult.Code InternalSetControlAELock(ControlAELock Data)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataSetControlAELock(Handle, ref Data);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetControlAELock));
				return result;
#else
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets AE Max Regions.
			/// </summary>
			private MLResult.Code InternalGetControlAEMaxRegions(out int OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetControlAEMaxRegions(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAEMaxRegions));
				return result;
#else
				OutData = 0;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets AE mode.
			/// </summary>
			private MLResult.Code InternalGetControlAEModeResultMetadata(out ControlAEMode OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetControlAEModeResultMetadata(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAEModeResultMetadata));
				return result;
#else
				OutData = ControlAEMode.Off;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets AE antibanding mode.
			/// </summary>
			private MLResult.Code InternalGetControlAEAntibandingModeResultMetadata(out ControlAEAntibandingMode OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetControlAEAntibandingModeResultMetadata(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAEAntibandingModeResultMetadata));
				return result;
#else
				OutData = ControlAEAntibandingMode.Off;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets AE compensation step.
			/// </summary>
			private MLResult.Code InternalGetControlAECompensationStep(out Rational OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				int sizeOfMLCameraMetadataRational = Marshal.SizeOf(typeof(Rational));
				IntPtr outDataPointer = Marshal.AllocHGlobal(sizeOfMLCameraMetadataRational);
				var result = NativeBindings.MLCameraMetadataGetControlAECompensationStep(Handle, outDataPointer);
				OutData = Marshal.PtrToStructure<Rational>(outDataPointer);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAECompensationStep));
				Marshal.FreeHGlobal(outDataPointer);
				
				return result;
#else
				OutData = new Rational();
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets color correction transform.
			/// </summary>
			private MLResult.Code InternalGetColorCorrectionTransformRequestMetadata(out Rational[][] OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				const int outDataRowSize = 3;
				const int outDataColSize = 3;
				int sizeOfMLCameraMetadataRational = Marshal.SizeOf(typeof(NativeBindings.MLCameraMetadataRational));
				int arraySize = sizeOfMLCameraMetadataRational * outDataRowSize * outDataColSize;					
				IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(arraySize));
				var result = NativeBindings.MLCameraMetadataGetColorCorrectionTransformRequestMetadata(Handle, ptr);
				OutData = new Rational[outDataColSize][];
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetColorCorrectionTransformRequestMetadata));

				if (result == MLResult.Code.Ok)
				{
					for(int i = 0; i < outDataColSize; ++i)
					{
						OutData[i] = new Rational[outDataRowSize];
						for (int j = 0; j < outDataRowSize; ++j)
						{
							OutData[i][j] = ConvertRational(Marshal.PtrToStructure<NativeBindings.MLCameraMetadataRational>(ptr));
							ptr += sizeOfMLCameraMetadataRational;
						}
					}
				}
				
				Marshal.FreeHGlobal(ptr);
				return result;
#else
				OutData = null;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets sensor info sensitivity range.
			/// </summary>
			private MLResult.Code InternalGetSensorInfoSensitivityRange(out int[] OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				OutData = new int[2];
				var result = NativeBindings.MLCameraMetadataGetSensorInfoSensitivityRange(Handle, OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetSensorInfoSensitivityRange));
				return result;
#else
				OutData = null;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Sets Effect mode.
			/// </summary>
			private MLResult.Code InternalSetControlEffectMode(ControlEffectMode Data)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataSetControlEffectMode(Handle, ref Data);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetControlEffectMode));
				return result;
#else
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Sets color correction aberration.
			/// </summary>
			private MLResult.Code InternalSetColorCorrectionAberrationMode(ColorCorrectionAberrationMode Data)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataSetColorCorrectionAberrationMode(Handle, ref Data);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetColorCorrectionAberrationMode));
				return result;
#else
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets color correction aberration modes.
			/// </summary>
			private MLResult.Code InternalGetColorCorrectionAvailableAberrationModes(
				out ColorCorrectionAberrationMode[] OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				const int sizeOfMLCameraMetadataColorCorrectionAberrationMode = sizeof(ColorCorrectionAberrationMode);
				var result = NativeBindings.MLCameraMetadataGetColorCorrectionAvailableAberrationModes(Handle,
					out IntPtr outDataPointer,
					out int outCount);
				OutData = new ColorCorrectionAberrationMode[outCount];
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetColorCorrectionAvailableAberrationModes));
				if (result == MLResult.Code.Ok)
				{
					for(int i = 0; i < outCount; ++i)
					{
						OutData[i] = (ColorCorrectionAberrationMode)Marshal.ReadInt32(outDataPointer);
						outDataPointer += sizeOfMLCameraMetadataColorCorrectionAberrationMode;
					}
				}
				
				Marshal.FreeHGlobal(outDataPointer);
				return result;
#else
				OutData = null;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets color correction aberration.
			/// </summary>
			private MLResult.Code InternalGetColorCorrectionAberrationModeRequestMetadata(
				out ColorCorrectionAberrationMode OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetColorCorrectionAberrationModeRequestMetadata(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetColorCorrectionAberrationModeRequestMetadata));
				return result;
#else
				OutData = ColorCorrectionAberrationMode.Off;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets AE lock.
			/// </summary>
			private MLResult.Code InternalGetControlAELockAvailable(out ControlAELock OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetControlAELockAvailable(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAELockAvailable));
				return result;
#else
				OutData = ControlAELock.Off;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets jpeg GPS coordinates.
			/// </summary>
			private MLResult.Code InternalGetJpegGPSCoordinatesRequestMetadata(out double[] OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				OutData = new double[3];
				var result = NativeBindings.MLCameraMetadataGetJpegGPSCoordinatesRequestMetadata(Handle, OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetJpegGPSCoordinatesRequestMetadata));
				return result;
#else
				OutData = null;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets Effect modes.
			/// </summary>
			private MLResult.Code InternalGetControlAvailableEffectModes(out ControlEffectMode[] OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetControlAvailableEffectModes(Handle, out OutData, out int OutCount);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAvailableEffectModes));
				return result;
#else
				OutData = null;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets Scene modes.
			/// </summary>
			private MLResult.Code InternalGetControlAvailableSceneModes(out ControlSceneMode[] OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetControlAvailableSceneModes(Handle, out OutData, out int OutCount);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAvailableSceneModes));
				return result;
#else
				OutData = null;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets AE modes.
			/// </summary>
			private MLResult.Code InternalGetControlAEAvailableModes(out ControlAEMode[] OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetControlAEAvailableModes(Handle, out OutData, out int OutCount);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAEAvailableModes));
				return result;
#else
				OutData = null;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets AE compensation range.
			/// </summary>
			private MLResult.Code InternalGetControlAECompensationRange(out int[] OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetControlAECompensationRange(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAECompensationRange));
				return result;
#else
				OutData = null;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets AWB modes.
			/// </summary>
			private MLResult.Code InternalGetControlAWBAvailableModes(out ControlAWBMode[] OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetControlAWBAvailableModes(Handle, out OutData, out int OutCount);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAWBAvailableModes));
				return result;
#else
				OutData = null;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets AE regions.
			/// </summary>
			private MLResult.Code InternalGetControlAERegionsRequestMetadata(out int[][] OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				const int maxColSize = 3;
				const int rowSize = 5;
				const int arraySize = sizeof(int) * maxColSize* rowSize;
				IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(arraySize));
				var result = NativeBindings.MLCameraMetadataGetControlAERegionsRequestMetadata(Handle, ptr, out int OutCount);
				OutData = new int[OutCount][];
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAERegionsRequestMetadata));

				if (result == MLResult.Code.Ok)
				{
					for (int i = 0; i < OutCount; ++i)
					{
						OutData[i] = new int[rowSize];
						for (int j = 0; j < rowSize; ++j)
						{
							OutData[i][j] = Marshal.ReadInt32(ptr);
							ptr += sizeof(int);
						}
					}
				}
				
				Marshal.FreeHGlobal(ptr);
				return result;
#else
				OutData = null;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets Available Modes.
			/// </summary>
			private MLResult.Code InternalGetControlAvailableModes(out ControlMode[] OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetControlAvailableModes(Handle, out OutData, out int OutCount);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAvailableModes));
				return result;
#else
				OutData = null;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Sets color correction transform.
			/// </summary>
			private MLResult.Code InternalSetColorCorrectionTransform(Rational[][] Data)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				int elementSize = Marshal.SizeOf<NativeBindings.MLCameraMetadataRational>();
				const int rowSize = 3;
				const int colSize = 3;
				int dataSize = elementSize * rowSize * colSize;
				NativeBindings.MLCameraMetadataRational[][] inputData = ConvertRational(Data);
				IntPtr ptr = Marshal.AllocHGlobal(dataSize);
				for(int i = 0; i < inputData.Length; ++i)
                {
					for(int j = 0; j < inputData[i].Length; ++j)
                    {
						IntPtr hptr = ptr + (i * rowSize + j) * elementSize;
                    }
                }
				var result = NativeBindings.MLCameraMetadataSetColorCorrectionTransform(Handle, ptr);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetColorCorrectionTransform));
				Marshal.FreeHGlobal(ptr);
				return result;
#else
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets AWB lock.
			/// </summary>
			private MLResult.Code InternalGetControlAWBLockAvailable(out ControlAWBLock OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetControlAWBLockAvailable(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAWBLockAvailable));
				return result;
#else
				OutData = ControlAWBLock.Off;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets sensor info active array sizes.
			/// </summary>
			private MLResult.Code InternalGetSensorInfoActiveArraySize(out int[] OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				OutData = new int[4];
				var result = NativeBindings.MLCameraMetadataGetSensorInfoActiveArraySize(Handle, OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetSensorInfoActiveArraySize));
				return result;
#else
				OutData = null;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets color correction mode.
			/// </summary>
			private MLResult.Code InternalGetColorCorrectionModeRequestMetadata(out ColorCorrectionMode OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetColorCorrectionModeRequestMetadata(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetColorCorrectionModeRequestMetadata));
				return result;
#else
				OutData = ColorCorrectionMode.Fast;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets sensor orientation degree.
			/// </summary>
			private MLResult.Code InternalGetSensorOrientation(out int OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetSensorOrientation(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetSensorOrientation));
				return result;
#else
				OutData = 0;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets color correction gains.
			/// </summary>
			private MLResult.Code InternalGetColorCorrectionGainsRequestMetadata(out float[] OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				float[] outData = new float[4];
				var result = NativeBindings.MLCameraMetadataGetColorCorrectionGainsRequestMetadata(Handle, outData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetColorCorrectionGainsRequestMetadata));
				OutData = outData;
				return result;
#else
				OutData = null;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets color correction transform.
			/// </summary>
			private MLResult.Code InternalGetColorCorrectionTransformResultMetadata(out Rational[][] OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				const int colSize = 3;
				const int rowSize = 3;
				int elementSize = Marshal.SizeOf(typeof(NativeBindings.MLCameraMetadataRational));
				IntPtr ptr = Marshal.AllocHGlobal(colSize * rowSize * elementSize);
				var result = NativeBindings.MLCameraMetadataGetColorCorrectionTransformResultMetadata(Handle, ptr);
				OutData = new Rational[colSize][];
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetColorCorrectionTransformResultMetadata));

				if (result == MLResult.Code.Ok)
				{
					for(int i = 0; i < colSize; ++i)
					{
						OutData[i] = new Rational[rowSize];
						for(int j = 0; j < rowSize; ++j)
						{
							NativeBindings.MLCameraMetadataRational element = Marshal.PtrToStructure<NativeBindings.MLCameraMetadataRational>(ptr);
							OutData[i][j] = ConvertRational(element);
							ptr += elementSize;
						}
					}
				}
				
				Marshal.FreeHGlobal(ptr);
				return result;
#else
				OutData = null;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets AE antibanding mode.
			/// </summary>
			private MLResult.Code InternalGetControlAEAntibandingModeRequestMetadata(out ControlAEAntibandingMode OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetControlAEAntibandingModeRequestMetadata(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAEAntibandingModeRequestMetadata));
				return result;
#else
				OutData = ControlAEAntibandingMode.Off;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets AE exposure compensation.
			/// </summary>
			private MLResult.Code InternalGetControlAEExposureCompensationRequestMetadata(out int OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetControlAEExposureCompensationRequestMetadata(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAEExposureCompensationRequestMetadata));
				return result;
#else
				OutData = 0;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets AE lock.
			/// </summary>
			private MLResult.Code InternalGetControlAELockRequestMetadata(out ControlAELock OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetControlAELockRequestMetadata(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAELockRequestMetadata));
				return result;
#else
				OutData = ControlAELock.Off;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets AE mode.
			/// </summary>
			private MLResult.Code InternalGetControlAEModeRequestMetadata(out ControlAEMode OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetControlAEModeRequestMetadata(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAEModeRequestMetadata));
				return result;
#else
				OutData = ControlAEMode.Off;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets AWB lock.
			/// </summary>
			private MLResult.Code InternalGetControlAWBLockRequestMetadata(out ControlAWBLock OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetControlAWBLockRequestMetadata(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAWBLockRequestMetadata));
				return result;
#else
				OutData = ControlAWBLock.Off;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets AWB mode.
			/// </summary>
			private MLResult.Code InternalGetControlAWBModeRequestMetadata(out ControlAWBMode OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetControlAWBModeRequestMetadata(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAWBModeRequestMetadata));
				return result;
#else
				OutData = ControlAWBMode.Off;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets mode.
			/// </summary>
			private MLResult.Code InternalGetControlModeRequestMetadata(out ControlMode OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetControlModeRequestMetadata(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlModeRequestMetadata));
				return result;
#else
				OutData = ControlMode.Off;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets Scene mode.
			/// </summary>
			private MLResult.Code InternalGetControlSceneModeRequestMetadata(out ControlSceneMode OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetControlSceneModeRequestMetadata(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlSceneModeRequestMetadata));
				return result;
#else
				OutData = ControlSceneMode.Action;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets sensor exposure time.
			/// </summary>
			private MLResult.Code InternalGetSensorExposureTimeRequestMetadata(out long OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetSensorExposureTimeRequestMetadata(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetSensorExposureTimeRequestMetadata));
				return result;
#else
				OutData = 0;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Effect mode.
			/// </summary>
			private MLResult.Code InternalGetControlEffectModeRequestMetadata(out ControlEffectMode OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetControlEffectModeRequestMetadata(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlEffectModeRequestMetadata));
				return result;
#else
				OutData = ControlEffectMode.Off;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets AE lock.
			/// </summary>
			private MLResult.Code InternalGetControlAELockResultMetadata(out ControlAELock OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetControlAELockResultMetadata(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAELockResultMetadata));
				return result;
#else
				OutData = ControlAELock.Off;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets exposure time upper limit.
			/// </summary>
			private MLResult.Code InternalGetControlExposureUpperTimeLimitRequestMetadata(out long OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetControlExposureUpperTimeLimitRequestMetadata(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlExposureUpperTimeLimitRequestMetadata));
				return result;
#else
				OutData = 0;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets jpeg timestamp.
			/// </summary>
			private MLResult.Code InternalGetJpegGPSTimestampRequestMetadata(out long OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetJpegGPSTimestampRequestMetadata(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetJpegGPSTimestampRequestMetadata));
				return result;
#else
				OutData = 0;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets jpeg thumbnail size.
			/// </summary>
			private MLResult.Code InternalGetJpegThumbnailSizeRequestMetadata(out JpegThumbnailSize OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetJpegThumbnailSizeRequestMetadata(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetJpegThumbnailSizeRequestMetadata));
				return result;
#else
				OutData = JpegThumbnailSize.Size_160x120;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets jpeg quality.
			/// </summary>
			private MLResult.Code InternalGetJpegQualityRequestMetadata(out byte OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetJpegQualityRequestMetadata(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetJpegQualityRequestMetadata));
				return result;
#else
				OutData = 0;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Sets force apply mode.  Main camera and CV Camera share the same camera hardware resources.  When both the cameras are
			/// streaming, request metadata properties for both cameras are merged and then applied.  While merging, the metadata
			/// properties from Main Camera take precedence over CV camera metadata properties.  The force apply mode property can be used to
			/// override this.  If CV Camera metadata has force apply mode on, the CV Camera metadata properties take precedence over
			/// Main Camera metadata properties.
			/// </summary>
			private MLResult.Code InternalSetControlForceApplyMode(ControlForceApplyMode Data)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataSetControlForceApplyMode(Handle, Data);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetControlForceApplyMode));
				return result;
#else
				return MLResult.Code.NotImplemented;
#endif
			}
			
			/// <summary>
			/// Gets force apply mode.  Main camera and CV Camera share the same camera hardware resources.  When both the cameras are
			/// streaming, request metadata properties for both cameras are merged and then applied.  While merging, the metadata
			/// properties from Main Camera take precedence over CV camera metadata properties.  The force apply mode property can be used to
			/// override this.  If CV Camera metadata has force apply mode on, the CV Camera metadata properties take precedence over
			/// Main Camera metadata properties.
			/// </summary>
			private MLResult.Code InternalGetControlForceApplyModeRequestMetadata(out ControlForceApplyMode OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetControlForceApplyModeRequestMetadata(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlForceApplyModeRequestMetadata));
				return result;
#else
				OutData = ControlForceApplyMode.Off;
				return MLResult.Code.NotImplemented;
#endif
			}
			
			/// <summary>
			/// Sets color correction mode.
			/// </summary>
			private MLResult.Code InternalSetColorCorrectionMode(ColorCorrectionMode Data)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataSetColorCorrectionMode(Handle, ref Data);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetColorCorrectionMode));
				return result;
#else
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets color correction gains.
			/// </summary>
			private MLResult.Code InternalGetColorCorrectionGainsResultMetadata(out float[] OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				float[] outData = new float[4];
				var result = NativeBindings.MLCameraMetadataGetColorCorrectionGainsResultMetadata(Handle, outData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetColorCorrectionGainsResultMetadata));
				OutData = outData;
				return result;
#else
				OutData = null;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Sets color correction gains.
			/// </summary>
			private MLResult.Code InternalSetColorCorrectionGains(float[] Data)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataSetColorCorrectionGains(Handle, Data);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetColorCorrectionGains));
				return result;
#else
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Sets AE antiband mode.
			/// </summary>
			private MLResult.Code InternalSetControlAEAntibandingMode(ControlAEAntibandingMode Data)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataSetControlAEAntibandingMode(Handle, ref Data);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetControlAEAntibandingMode));
				return result;
#else
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Sets AE exposure compensation.
			/// </summary>
			private MLResult.Code InternalSetControlAEExposureCompensation(int Data)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataSetControlAEExposureCompensation(Handle, ref Data);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetControlAEExposureCompensation));
				return result;
#else
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Sets AE mode.  MLCameraMetadataControlAEMode_Off is not supported if camera is configured for 15FPS
			/// </summary>
			private MLResult.Code InternalSetControlAEMode(ControlAEMode Data)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataSetControlAEMode(Handle, ref Data);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetControlAEMode));
				return result;
#else
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// set AE regions.
			/// </summary>
			private MLResult.Code InternalSetControlAERegions(int[][] Data)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				const int colSize = 3;
				const int rowSize = 5;
				const int elementSize = sizeof(int);
				IntPtr ptr = Marshal.AllocHGlobal(elementSize * colSize * rowSize);
				for(int i = 0; i < Data.Length; ++i)
				{
					for(int j = 0; j < rowSize; ++j)
					{
						Marshal.WriteInt32(ptr, (i * rowSize + j) * elementSize, Data[i][j]);
					}
				}
				int count = Data.Length;
				var result = NativeBindings.MLCameraMetadataSetControlAERegions(Handle, ptr, count);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetControlAERegions));
				
				Marshal.FreeHGlobal(ptr);
				return result;
#else
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Sets AWB lock.
			/// </summary>
			private MLResult.Code InternalSetControlAWBLock(ControlAWBLock Data)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataSetControlAWBLock(Handle, ref Data);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetControlAWBLock));
				return result;
#else
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Sets AWB mode.
			/// </summary>
			private MLResult.Code InternalSetControlAWBMode(ControlAWBMode Data)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataSetControlAWBMode(Handle, ref Data);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetControlAWBMode));
				return result;
#else
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Sets mode.
			/// </summary>
			private MLResult.Code InternalSetControlMode(ControlMode Data)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataSetControlMode(Handle, ref Data);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetControlMode));
				return result;
#else
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Sets Scene mode.
			/// </summary>
			private MLResult.Code InternalSetControlSceneMode(ControlSceneMode Data)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataSetControlSceneMode(Handle, ref Data);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetControlSceneMode));
				return result;
#else
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets AWB lock.
			/// </summary>
			private MLResult.Code InternalGetControlAWBLockResultMetadata(out ControlAWBLock OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetControlAWBLockResultMetadata(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAWBLockResultMetadata));
				return result;
#else
				OutData = ControlAWBLock.Off;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Sets sensor exposure time.
			/// </summary>
			private MLResult.Code InternalSetSensorExposureTime(long Data)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataSetSensorExposureTime(Handle, ref Data);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetSensorExposureTime));
				return result;
#else
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Sets sensor sensitivity.
			/// </summary>
			private MLResult.Code InternalSetSensorSensitivity(int Data)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataSetSensorSensitivity(Handle, ref Data);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetSensorSensitivity));
				return result;
#else
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Sets exposure time upper limit.
			/// </summary>
			private MLResult.Code InternalSetControlExposureUpperTimeLimit(long Data)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataSetControlExposureUpperTimeLimit(Handle, Data);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetControlExposureUpperTimeLimit));
				return result;
#else
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Sets jpeg GPS coordinates.
			/// </summary>
			private MLResult.Code InternalSetJpegGPSCoordinates(double[] Data)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataSetJpegGPSCoordinates(Handle, Data);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetJpegGPSCoordinates));
				return result;
#else
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Sets jpeg timestamp.
			/// </summary>
			private MLResult.Code InternalSetJpegGPSTimestamp(long Data)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataSetJpegGPSTimestamp(Handle, Data);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetJpegGPSTimestamp));
				return result;
#else
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Sets jpeg thumbnail size.
			/// </summary>
			private MLResult.Code InternalSetJpegThumbnailSize(JpegThumbnailSize Data)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataSetJpegThumbnailSize(Handle, ref Data);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetJpegThumbnailSize));
				return result;
#else
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Sets jpeg quality.
			/// </summary>
			private MLResult.Code InternalSetJpegQuality(byte Data)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataSetJpegQuality(Handle, ref Data);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetJpegQuality));
				return result;
#else
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets color correction.
			/// </summary>
			private MLResult.Code InternalGetColorCorrectionModeResultMetadata(out ColorCorrectionMode OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetColorCorrectionModeResultMetadata(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetColorCorrectionModeResultMetadata));
				return result;
#else
				OutData = ColorCorrectionMode.Fast;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets color correction aberration.
			/// </summary>
			private MLResult.Code InternalGetColorCorrectionAberrationModeResultMetadata(out ColorCorrectionAberrationMode OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetColorCorrectionAberrationModeResultMetadata(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetColorCorrectionAberrationModeResultMetadata));
				return result;
#else
				OutData = ColorCorrectionAberrationMode.Off;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets AE exposure compensation.
			/// </summary>
			private MLResult.Code InternalGetControlAEExposureCompensationResultMetadata(out int OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetControlAEExposureCompensationResultMetadata(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAEExposureCompensationResultMetadata));
				return result;
#else
				OutData = 0;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets AE regions.
			/// </summary>
			private MLResult.Code InternalGetControlAERegionsResultMetadata(out int[][] OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				const int colSize = 3;
				const int rowSize = 5;
				const int elementSize = sizeof(int);
				IntPtr ptr = Marshal.AllocHGlobal(colSize * rowSize * elementSize);
				var result = NativeBindings.MLCameraMetadataGetControlAERegionsResultMetadata(Handle, ptr, out int OutCount);
				OutData = new int[OutCount][];
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAERegionsResultMetadata));

				if (result == MLResult.Code.Ok)
				{
					for (int i = 0; i < OutCount; ++i)
					{
						OutData[i] = new int[rowSize];
						for (int j = 0; j < rowSize; ++j)
						{
							OutData[i][j] = Marshal.ReadInt32(ptr);
							ptr += elementSize;
						}
					}
				}
				
				Marshal.FreeHGlobal(ptr);
				return result;
#else
				OutData = null;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets AE target FPS range.
			/// </summary>
			private MLResult.Code InternalGetControlAETargetFPSRangeResultMetadata(out int[] OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				int[] outData = new int[2];
				var result = NativeBindings.MLCameraMetadataGetControlAETargetFPSRangeResultMetadata(Handle, outData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAETargetFPSRangeResultMetadata));
				OutData = outData;
				return result;
#else
				OutData = null;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets AE state.
			/// </summary>
			private MLResult.Code InternalGetControlAEStateResultMetadata(out ControlAEState OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetControlAEStateResultMetadata(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAEStateResultMetadata));
				return result;
#else
				OutData = ControlAEState.Converged;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets AWB state.
			/// </summary>
			private MLResult.Code InternalGetControlAWBStateResultMetadata(out ControlAWBState OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetControlAWBStateResultMetadata(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAWBStateResultMetadata));
				return result;
#else
				OutData = ControlAWBState.Converged;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets AWB mode.
			/// </summary>
			private MLResult.Code InternalGetControlAWBModeResultMetadata(out ControlAWBMode OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetControlAWBModeResultMetadata(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAWBModeResultMetadata));
				return result;
#else
				OutData = ControlAWBMode.Off;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets mode.
			/// </summary>
			private MLResult.Code InternalGetControlModeResultMetadata(out ControlMode OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetControlModeResultMetadata(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlModeResultMetadata));
				return result;
#else
				OutData = ControlMode.Off;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets Scene mode.
			/// </summary>
			private MLResult.Code InternalGetControlSceneModeResultMetadata(out ControlSceneMode OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetControlSceneModeResultMetadata(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlSceneModeResultMetadata));
				return result;
#else
				OutData = ControlSceneMode.Action;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets sensor exposure time.
			/// </summary>
			private MLResult.Code InternalGetSensorExposureTimeResultMetadata(out long OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetSensorExposureTimeResultMetadata(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetSensorExposureTimeResultMetadata));
				return result;
#else
				OutData = 0;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets sensor sensitivity.
			/// </summary>
			private MLResult.Code InternalGetSensorSensitivityResultMetadata(out int OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetSensorSensitivityResultMetadata(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetSensorSensitivityResultMetadata));
				return result;
#else
				OutData = 0;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets frame captured timestamp.
			/// </summary>
			private MLResult.Code InternalGetSensorTimestampResultMetadata(out long OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetSensorTimestampResultMetadata(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetSensorTimestampResultMetadata));
				return result;
#else
				OutData = 0;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets sensor frame duration.
			/// </summary>
			private MLResult.Code InternalGetSensorFrameDurationResultMetadata(out long OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetSensorFrameDurationResultMetadata(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetSensorFrameDurationResultMetadata));
				return result;
#else
				OutData = 0;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets Effect mode.
			/// </summary>
			private MLResult.Code InternalGetControlEffectModeResultMetadata(out ControlEffectMode OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetControlEffectModeResultMetadata(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlEffectModeResultMetadata));
				return result;
#else
				OutData = ControlEffectMode.Off;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets exposure time upper limit.
			/// </summary>
			private MLResult.Code InternalGetControlExposureUpperTimeLimitResultMetadata(out long OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetControlExposureUpperTimeLimitResultMetadata(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlExposureUpperTimeLimitResultMetadata));
				return result;
#else
				OutData = 0;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets jpeg GPS coordinates.
			/// </summary>
			private MLResult.Code InternalGetJpegGPSCoordinatesResultMetadata(out double[] OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetJpegGPSCoordinatesResultMetadata(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetJpegGPSCoordinatesResultMetadata));
				return result;
#else
				OutData = null;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets jpeg timestamp.
			/// </summary>
			private MLResult.Code InternalGetJpegGPSTimestampResultMetadata(out long OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetJpegGPSTimestampResultMetadata(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetJpegGPSTimestampResultMetadata));
				return result;
#else
				OutData = 0;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets jpeg thumbnail size.
			/// </summary>
			private MLResult.Code InternalGetJpegThumbnailSizeResultMetadata(out JpegThumbnailSize OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetJpegThumbnailSizeResultMetadata(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetJpegThumbnailSizeResultMetadata));
				return result;
#else
				OutData = JpegThumbnailSize.Size_160x120;
				return MLResult.Code.NotImplemented;
#endif
			}

			/// <summary>
			/// Gets force apply mode.  Main camera and CV Camera share the same camera hardware resources.  When both the cameras are
			/// streaming, request metadata properties for both cameras are merged and then applied.  While merging, the metadata
			/// properties from Main Camera take precedence over CV camera metadata properties.  The force apply mode property can be used to
			/// override this.  If CV Camera metadata has force apply mode on, the CV Camera metadata properties take precedence over
			/// Main Camera metadata properties.
			/// </summary>
			private MLResult.Code InternalGetControlForceApplyModeResultMetadata(out ControlForceApplyMode OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetControlForceApplyModeResultMetadata(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlForceApplyModeResultMetadata));
				return result;
#else
				OutData = ControlForceApplyMode.Off;
				return MLResult.Code.NotImplemented;
#endif
			}
			
			/// <summary>
			/// Gets jpeg quality.
			/// </summary>
			private MLResult.Code InternalGetJpegQualityResultMetadata(out byte OutData)
			{
#if UNITY_MAGICLEAP || UNITY_ANDROID
				var result = NativeBindings.MLCameraMetadataGetJpegQualityResultMetadata(Handle, out OutData);
				MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetJpegQualityResultMetadata));
				return result;
#else
				OutData = 0;
				return MLResult.Code.NotImplemented;
#endif
			}

#if UNITY_MAGICLEAP || UNITY_ANDROID
			private static Rational ConvertRational(NativeBindings.MLCameraMetadataRational rational)
            {
				Rational result = new Rational();
				result.Numerator = rational.Numerator;
				result.Denominator = rational.Denominator;
				return result;
			}

			private static NativeBindings.MLCameraMetadataRational ConvertRational(Rational rational)
			{
				NativeBindings.MLCameraMetadataRational result = new NativeBindings.MLCameraMetadataRational();
				result.Numerator = rational.Numerator;
				result.Denominator = rational.Denominator;
				return result;
			}

			private static NativeBindings.MLCameraMetadataRational[] ConvertRational(Rational[] rational)
			{
				NativeBindings.MLCameraMetadataRational[] result = new NativeBindings.MLCameraMetadataRational[rational.Length];
				for (int i = 0; i < rational.Length; ++i)
				{
					result[i] = ConvertRational(rational[i]);
				}
				return result;
			}

			private static NativeBindings.MLCameraMetadataRational[][] ConvertRational(Rational[][] rational)
            {
				NativeBindings.MLCameraMetadataRational[][] result = new NativeBindings.MLCameraMetadataRational[rational.Length][];
				for(int i = 0; i < rational.Length; ++i)
                {
					result[i] = ConvertRational(rational[i]);
                }
				return result;
			}
#endif
		}
	}
}
