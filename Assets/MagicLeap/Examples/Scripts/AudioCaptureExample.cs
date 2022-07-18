// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="AudioCaptureExample.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2019 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;

namespace MagicLeap.Examples
{
    /// <summary>
    /// This class uses a controller to start/stop audio capture
    /// using the Unity Microphone class. The audio is then played
    /// through an audio source attached to the parrot in the scene.
    /// </summary>
    public class AudioCaptureExample : MonoBehaviour
    {
        public enum CaptureMode
        {
            Inactive = 0,
            Realtime,
            Delayed
        }

        [SerializeField, Tooltip("The reference to the place from camera script for the parrot.")]
        private PlaceFromCamera _placeFromCamera = null;

        [SerializeField, Tooltip("The reference to the permission requester in the scene.")]
        private MLPermissionRequesterBehavior _permissionRequester = null;

        [SerializeField, Tooltip("The audio source that should capture the microphone input.")]
        private AudioSource _inputAudioSource = null;

        [SerializeField, Tooltip("The audio source that should replay the captured audio.")]
        private AudioSource _playbackAudioSource = null;

        [SerializeField, Tooltip("The text to display the recording status.")]
        private Text _statusLabel = null;

        [Space]
        [Header("Delayed Playback")]
        [SerializeField, Range(1, 2), Tooltip("The pitch used for delayed audio playback.")]
        private float _pitch = 1.5f;

        [SerializeField, Tooltip("Game object to use for visualizing the root mean square of the microphone audio")]
        private GameObject _rmsVisualizer = null;

        [SerializeField, Min(0), Tooltip("Scale value to set for AmplitudeVisualizer when rms is 0")]
        private float _minScale = 0.1f;

        [SerializeField, Min(0), Tooltip("Scale value to set for AmplitudeVisualizer when rms is 1")]
        private float _maxScale = 1.0f;

        private bool hasPermission = false;
        private bool isCapturing = false;
        private CaptureMode captureMode = CaptureMode.Inactive;
        private string deviceMicrophone = string.Empty;

        private float audioMaxSample = 0;
        private readonly float[] audioSamples = new float[128];

        private int numSyncIterations = 30;
        private int numSamplesLatency = 1024;
        private bool playbackStarted = false;

        private bool isAudioDetected = false;
        private float audioLastDetectionTime = 0;
        private float audioDetectionStart = 0;
        private float audioDetectionEnd = 0;

        private float[] playbackSamples = null;

        private const int AUDIO_CLIP_LENGTH_SECONDS = 60;
        private const int AUDIO_CLIP_FREQUENCY_HERTZ = 48000;
        private const float AUDIO_SENSITVITY_DECIBEL = 0.00035f;
        private const float AUDIO_CLIP_TIMEOUT_SECONDS = 2;
        private const float AUDIO_CLIP_FALLOFF_SECONDS = 0.5f;

        private const int NUM_SYNC_ITERATIONS = 30;
        private const int NUM_SAMPLES_LATENCY = 1024;

        private MagicLeapInputs mlInputs;
        private MagicLeapInputs.ControllerActions controllerActions;

        void Awake()
        {
            if (_inputAudioSource == null)
            {
                Debug.LogError("Error: AudioCaptureExample._inputAudioSource is not set, disabling script.");
                enabled = false;
                return;
            }

            if (_playbackAudioSource == null)
            {
                Debug.LogError("Error: AudioCaptureExample._playbackAudioSource is not set, disabling script.");
                enabled = false;
                return;
            }

            if (_statusLabel == null)
            {
                Debug.LogError("Error: AudioCaptureExample._statusLabel is not set, disabling script.");
                enabled = false;
                return;
            }

            if (_placeFromCamera == null)
            {
                Debug.LogError("Error: AudioCaptureExample._placeFromCamera is not set, disabling script.");
                enabled = false;
                return;
            }

            if (_rmsVisualizer == null)
            {
                Debug.LogError("Error: AudioCaptureExample._rmsVisualizer is not set, disabling script.");
                enabled = false;
                return;
            }

            UpdateStatus();

#if UNITY_MAGICLEAP || UNITY_ANDROID
            // Before enabling the microphone, the scene must wait until the permissions have been granted.
            _permissionRequester.OnPermissionsDone += HandleOnPermissionsDone;
            mlInputs = new MagicLeapInputs();
            mlInputs.Enable();
            controllerActions = new MagicLeapInputs.ControllerActions(mlInputs);

            controllerActions.Bumper.performed += HandleOnBumperDown;
            controllerActions.Trigger.performed += HandleOnTriggerDown;
#endif

            // Frequency = number of samples per second
            // 1000ms => AUDIO_CLIP_FREQUENCY_HERTZ
            // 1ms => AUDIO_CLIP_FREQUENCY_HERTZ / 1000
            // 16ms => AUDIO_CLIP_FREQUENCY_HERTZ * 16 / 1000
            playbackSamples = new float[AUDIO_CLIP_FREQUENCY_HERTZ * 16 / 1000];
        }

        void OnDestroy()
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            _permissionRequester.OnPermissionsDone -= HandleOnPermissionsDone;
            controllerActions.Bumper.performed -= HandleOnBumperDown;
            controllerActions.Trigger.performed -= HandleOnTriggerDown;

            mlInputs.Dispose();
#endif

            StopCapture();
        }

        private void Update()
        {
            if (isCapturing)
            {
                ProcessAudioPlayback();
            }

            if (playbackStarted)
            {
                if (captureMode == CaptureMode.Delayed)
                {
                    DetectAudio();
                }
            }

            UpdateStatus();
        }

        void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                // require privledges to be checked again.
                hasPermission = false;
                captureMode = CaptureMode.Inactive;

                if (playbackStarted)
                {
                    StopCapture();
                }
            }
        }

        private void StartMicrophone()
        {
            if (captureMode == CaptureMode.Inactive)
            {
                Debug.LogError("Error: AudioCaptureExample.StartMicrophone() cannot start with CaptureMode.Inactive.");
                return;
            }

            // Use the first detected Microphone device.
            if (Microphone.devices.Length > 0)
            {
                deviceMicrophone = Microphone.devices[0];
            }

            // If no microphone is detected, exit early and log the error.
            if (string.IsNullOrEmpty(deviceMicrophone))
            {
                Debug.LogError("Error: AudioCaptureExample._deviceMicrophone could not find a microphone device, disabling script.");
                enabled = false;
                return;
            }

            _playbackAudioSource.Stop();
            _inputAudioSource.loop = true;
            _inputAudioSource.clip = Microphone.Start(deviceMicrophone, true, AUDIO_CLIP_LENGTH_SECONDS, AUDIO_CLIP_FREQUENCY_HERTZ);

            isCapturing = true;
            numSamplesLatency = NUM_SAMPLES_LATENCY;
            numSyncIterations = NUM_SYNC_ITERATIONS;
        }

        private void ProcessAudioPlayback()
        {
            if (numSyncIterations > 0)
            {
                --numSyncIterations;
            }

            if (!playbackStarted && (numSyncIterations == 0) && (Microphone.GetPosition(deviceMicrophone) > numSamplesLatency))
            {
                _inputAudioSource.Play();
                _inputAudioSource.timeSamples = Microphone.GetPosition(deviceMicrophone) - numSamplesLatency;
                playbackStarted = true;

                switch (captureMode)
                {
                    case CaptureMode.Realtime:
                        {
                            _playbackAudioSource.pitch = 1;
                            _playbackAudioSource.clip = _inputAudioSource.clip;
                            _playbackAudioSource.loop = true;
                            _playbackAudioSource.Play();

                            break;
                        }

                    case CaptureMode.Delayed:
                        {
                            _playbackAudioSource.pitch = _pitch;
                            _playbackAudioSource.loop = false;
                            break;
                        }
                }
            }

            // Increasing latency
            if ((_inputAudioSource.timeSamples > Microphone.GetPosition(deviceMicrophone)) && (Microphone.GetPosition(deviceMicrophone) > numSamplesLatency * 4))
            {
                numSamplesLatency = numSamplesLatency * 2;
                _inputAudioSource.timeSamples = Microphone.GetPosition(deviceMicrophone) - numSamplesLatency;
            }

            if (_playbackAudioSource.isPlaying)
            {
                _playbackAudioSource.clip.GetData(playbackSamples, _playbackAudioSource.timeSamples);

                float squaredSum = 0;
                for (int i = 0; i < playbackSamples.Length; ++i)
                {
                    squaredSum += playbackSamples[i] * playbackSamples[i];
                }

                float rootMeanSq = Mathf.Sqrt(squaredSum / playbackSamples.Length);
                float scaleFactor = rootMeanSq * (_maxScale - _minScale) + _minScale;
                _rmsVisualizer.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            }
        }

        private void StopCapture()
        {
            isCapturing = false;
            playbackStarted = false;

            // Stop microphone and input audio source.
            _inputAudioSource.Stop();

            if (!string.IsNullOrEmpty(deviceMicrophone))
            {
                Microphone.End(deviceMicrophone);
            }

            // Stop audio playback source and reset settings.
            _playbackAudioSource.Stop();
            _playbackAudioSource.loop = false;
            _playbackAudioSource.clip = null;
        }

        /// <summary>
        /// Update the example status label.
        /// </summary>
        private void UpdateStatus()
        {
            _statusLabel.text = string.Format("<color=#dbfb76><b>Controller Data</b></color>\nStatus: {0}\n", ControllerStatus.Text);

            _statusLabel.text += "\n<color=#dbfb76><b>AudioCapture Data</b></color>\n";
            if (!hasPermission)
            {
                _statusLabel.text += (_permissionRequester.State != MLPermissionRequesterBehavior.PermissionState.Failed) ? "Status: Requesting Permissions\n" : "Status: Permissions Denied\n";
            }
            else
            {
                _statusLabel.text += string.Format("Status: {0}\n", captureMode.ToString());
            }

            _statusLabel.text += "\n<color=#dbfb76><b>Microphone devices</b></color>\n";

            string[] microphones = Microphone.devices;

            for (int i = 0; i < Microphone.devices.Length; i++)
            {
                _statusLabel.text += $"Device {i + 1}: {microphones[i]}\n";
            }
        }

        private void DetectAudio()
        {
            // Analyze the input spectrum data, to determine when someone is speaking.
            _inputAudioSource.GetSpectrumData(audioSamples, 0, FFTWindow.Rectangular);
            audioMaxSample = audioSamples.Max();

            if (audioMaxSample > AUDIO_SENSITVITY_DECIBEL)
            {
                // Note the first moment speech was detected.
                audioLastDetectionTime = Time.time;

                if (isAudioDetected == false)
                {
                    isAudioDetected = true;
                    audioDetectionStart = _inputAudioSource.time;
                }
            }
            else if (isAudioDetected && (Time.time > audioLastDetectionTime + AUDIO_CLIP_TIMEOUT_SECONDS))
            {
                // Note the last moment speach was detected.
                audioDetectionEnd = _inputAudioSource.time - (AUDIO_CLIP_TIMEOUT_SECONDS - AUDIO_CLIP_FALLOFF_SECONDS);

                // Create the playback clip.
                _playbackAudioSource.clip = CreateAudioClip(_inputAudioSource.clip, audioDetectionStart, audioDetectionEnd);
                if (_playbackAudioSource.clip != null)
                {
                    _playbackAudioSource.Play();
                }

                // Reset and allow for new captured speech.
                isAudioDetected = false;
                audioDetectionStart = 0;
                audioDetectionEnd = 0;
            }
        }

        /// <summary>
        /// Creates a new audio clip within the start and stop range.
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <returns></returns>
        private AudioClip CreateAudioClip(AudioClip clip, float start, float stop)
        {
            int length = (int)(clip.frequency * (stop - start));
            if (length <= 0)
            {
                return null;
            }

            AudioClip audioClip = AudioClip.Create("Parrot_Voice", length, 1, clip.frequency, false);

            float[] data = new float[length];
            clip.GetData(data, (int)(clip.frequency * start));
            audioClip.SetData(data, 0);

            return audioClip;
        }

        /// <summary>
        /// Responds to permission requester result.
        /// </summary>
        /// <param name="result"/>
        private void HandleOnPermissionsDone(MLResult result)
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            if (!result.IsOk)
            {
                Debug.LogErrorFormat("Error: AudioCaptureExample failed to get all requested permissions, disabling script. Reason: {0}", result);
                UpdateStatus();
                enabled = false;
                return;
            }
#endif

            hasPermission = true;
            Debug.Log("Succeeded in requesting all permissions");
        }

        private void HandleOnTriggerDown(InputAction.CallbackContext inputCallback)
        {
            if (!hasPermission)
                return;

            if (!controllerActions.Trigger.WasPressedThisFrame())
                return;

            captureMode = (captureMode == CaptureMode.Delayed) ? CaptureMode.Inactive : captureMode + 1;

            // Stop & Start to clear the previous mode.
            if (isCapturing)
            {
                StopCapture();
            }

            if (captureMode != CaptureMode.Inactive)
            {
                StartMicrophone();
            }
        }

        private void HandleOnBumperDown(InputAction.CallbackContext inputCallback)
        {
            StartCoroutine(nameof(SingleFrameUpdate));
        }

        private IEnumerator SingleFrameUpdate()
        {
            _placeFromCamera.PlaceOnUpdate = true;
            yield return new WaitForEndOfFrame();
            _placeFromCamera.PlaceOnUpdate = false;
        }
    }
}
