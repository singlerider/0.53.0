// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
//
// Copyright (c) 2019 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnityEngine.XR.MagicLeap
{
    /// <summary>
    /// Automatically requests specified Permissions. Exposes delegate for when the requests are done.
    /// Fails if at least one Permission is denied.
    /// </summary>
    [AddComponentMenu("XR/MagicLeap/MLPermissionRequesterBehavior")]
    public class MLPermissionRequesterBehavior : MonoBehaviour
    {
        // <summary/>
        public enum PermissionState
        {
            /// <summary>Requester has not started.</summary>
            Off,

            /// <summary>Failed to start because a Permission system failure.</summary>
            StartFailed,

            /// <summary>Requester has started requesting Permissions.</summary>
            Started,

            /// <summary>All Permissions have been requested. Waiting on results.</summary>
            Requested,

            /// <summary>All Permissions were granted.</summary>
            Succeeded,

            /// <summary>One or more of the Permissions were denied, or some other Permission failure request occured.</summary>
            Failed
        }

        PermissionState _state = PermissionState.Off;

        /// <summary>
        /// Current state of the requester.
        /// </summary>
        public PermissionState State => _state;

        /// <summary>
        /// Subscribed methods are called when all Permissions have been granted,
        /// or if a failure occured with the request. The possible result codes are:
        ///
        /// MLResult.Code.Ok if all Permissions were granted.
        ///
        /// MLResult.Code.PermissionDenied if one or more of the requested Permissions were denied.
        ///
        /// MLResult.Code.UnspecifiedFailure if the Permission system failed to startup.
        /// </summary>
        public event Action<MLResult> OnPermissionsDone = delegate { };

        // TODO: Disable editor edit when state is StartFailed, Started, Requested, Failed or Succeeded states.
        [SerializeField]
        [Tooltip("Requested Permissions. " +
            "Can also be modified via script using Permissions property. " +
            "Should only be changed in Editor mode. Changes with the component active will not be immediately reflected in behavior.")]
        string[] _permissions;

        /// <summary>
        /// Requested Permissions. Should be set on Awake.
        /// </summary>
        public string[] Permissions
        {
            get { return _permissions; }
            set { _permissions = value; }
        }

        readonly List<string> _permissionsToRequest = new List<string>();
        readonly List<string> _permissionsGranted = new List<string>();

        private readonly MLPermissions.Callbacks callbacks = new MLPermissions.Callbacks();

        private void OnEnable()
        {
            callbacks.OnPermissionGranted += OnPermissionGranted;
            callbacks.OnPermissionDenied += OnPermissionDenied;
            callbacks.OnPermissionDeniedAndDontAskAgain += OnPermissionDenied;
        }

        private void OnDisable()
        {
            callbacks.OnPermissionGranted -= OnPermissionGranted;
            callbacks.OnPermissionDenied -= OnPermissionDenied;
            callbacks.OnPermissionDeniedAndDontAskAgain -= OnPermissionDenied;
        }

        /// <summary>
        /// Start the Permissions API and set the Permission State
        /// </summary>
        void Start()
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            _permissionsToRequest.AddRange(_permissions);
            _state = PermissionState.Started;
#endif
        }

        /// <summary>
        /// Move through the Permission stages
        /// </summary>
        void Update()
        {
            /// Permissions have not yet been granted, go through the Permission states.
            if (_state != PermissionState.Succeeded)
            {
                UpdatePermission();
            }
        }

        /// <summary>
        /// Cannot make the assumption that a reality Permission is still granted after
        /// returning from pause. Return the application to the state where it
        /// requests Permissions needed and clear out the list of already granted
        /// Permissions. Also, disable the camera and unregister callbacks.
        /// </summary>
        /// <remarks>
        /// Not necessary, but harmless, for sensitive Permissions.
        /// </remarks>
        void OnApplicationPause(bool pause)
        {
            if (pause && _state != PermissionState.Off)
            {
                _permissionsGranted.Clear();

                _state = PermissionState.Started;
            }
        }

        /// <summary>
        /// Handle the Permission states.
        /// </summary>
        private void UpdatePermission()
        {
            switch (_state)
            {
                /// Permission API has been started successfully, ready to make requests.
                case PermissionState.Started:
                    {
                        RequestPermissions();
                        break;
                    }
                /// Permission requests have been made, wait until all Permissions are granted before enabling the feature that requires Permissions.
                case PermissionState.Requested:
                    {
                        foreach (var priv in _permissionsToRequest)
                        {
                            if (!_permissionsGranted.Contains(priv))
                            {
                                return;
                            }
                        }

                        _state = PermissionState.Succeeded;

#if UNITY_MAGICLEAP || UNITY_ANDROID
                        OnPermissionsDone(MLResult.Create(MLResult.Code.Ok));
#endif

                        break;
                    }
                /// Permissions have been denied, respond appropriately.
                case PermissionState.Failed:
                    {
#if UNITY_MAGICLEAP || UNITY_ANDROID
                        OnPermissionsDone(MLResult.Create(MLResult.Code.PermissionDenied));
#endif

                        enabled = false;
                        break;
                    }
            }
        }

        /// <summary>
        /// Request each needed Permission.
        /// </summary>
        private void RequestPermissions()
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            foreach (string priv in _permissionsToRequest)
            {
                MLResult result = MLPermissions.RequestPermission(priv, callbacks);
                if (!(result.Result == MLResult.Code.Ok))
                {
                    Debug.LogErrorFormat("Error: PermissionRequester failed requesting {0} Permission. Reason: {1}", priv, result);
                    _state = PermissionState.Failed;
                    return;
                }
            }
#endif

            _state = PermissionState.Requested;
        }

        private void OnPermissionDenied(string permission)
        {
            _state = PermissionState.Failed;
        }

        private void OnPermissionGranted(string permission)
        {
            _permissionsGranted.Add(permission);
        }
    }
}
