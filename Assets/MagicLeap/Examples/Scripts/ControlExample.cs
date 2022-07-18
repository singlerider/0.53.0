// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="ControlExample.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2019 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;

namespace MagicLeap.Examples
{
    /// <summary>
    /// This class provides examples of how you can use haptics
    /// on the Control.
    /// </summary>
    public class ControlExample : MonoBehaviour
    {
        private MagicLeapInputs mlInputs;
        private MagicLeapInputs.ControllerActions controllerActions;

        [SerializeField] private GestureSubsystemComponent gestureComponent;

        [SerializeField, Tooltip("The text used to display status information for the example..")]
        private Text _statusText = null;

        /// <summary>
        /// Initialize variables, callbacks and check null references.
        /// </summary>
        void Start()
        {
            mlInputs = new MagicLeapInputs();
            mlInputs.Enable();
            controllerActions = new MagicLeapInputs.ControllerActions(mlInputs);

            controllerActions.Touchpad1Position.performed += HandleOnTouchpad;
            // canceled event used to detect when bumper button is released
            controllerActions.Bumper.canceled += HandleOnBumper;
            controllerActions.Bumper.performed += HandleOnBumper;
            controllerActions.Trigger.performed += HandleOnTrigger;

            InputSubsystem.Extensions.Controller.AttachTriggerListener(HandleOnTriggerEvent);
        }

        /// <summary>
        /// Update controller input based feedback.
        /// </summary>
        void Update()
        {
            UpdateStatus();
        }

        /// <summary>
        /// Stop input api and unregister callbacks.
        /// </summary>
        void OnDestroy()
        {
            controllerActions.Touchpad1Position.performed -= HandleOnTouchpad;
            controllerActions.Bumper.canceled -= HandleOnBumper;
            controllerActions.Bumper.performed -= HandleOnBumper;
            controllerActions.Trigger.performed -= HandleOnTrigger;

            InputSubsystem.Extensions.Controller.RemoveTriggerListener(HandleOnTriggerEvent);

            mlInputs.Dispose();
        }

        private void UpdateStatus()
        {
            _statusText.text = $"<color=#dbfb76><b>Controller Data</b></color>\n Status: {ControllerStatus.Text}\n";

#if UNITY_MAGICLEAP || UNITY_ANDROID
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append($"Position: <i>{controllerActions.Position.ReadValue<Vector3>().ToString("n2")}</i>\n");
            strBuilder.Append($"Rotation: <i>{controllerActions.Rotation.ReadValue<Quaternion>().ToString("n1")}</i>\n\n");
            strBuilder.Append($"<color=#dbfb76><b>Buttons</b></color>\n");
            strBuilder.Append($"Menu: <i>{controllerActions.Menu.IsPressed()}</i>\n\n");
            strBuilder.Append($"Trigger: <i>{controllerActions.Trigger.ReadValue<float>():n2}</i>\n");
            strBuilder.Append($"Bumper: <i>{controllerActions.Bumper.IsPressed()}</i>\n\n");
            strBuilder.Append($"<color=#dbfb76><b>Touchpad</b></color>\n");
            strBuilder.Append($"Location: <i>({controllerActions.Touchpad1Position.ReadValue<Vector2>().x:n2}," +
                              $"{controllerActions.Touchpad1Position.ReadValue<Vector2>().y:n2})</i>\n");
            strBuilder.Append($"Pressure: <i>{controllerActions.Touchpad1Force.ReadValue<float>()}</i>\n\n");
            strBuilder.Append($"<color=#dbfb76><b>Gestures</b></color>\n<i></i>");
            foreach (var touchpadEvent in gestureComponent.gestureSubsystem.touchpadGestureEvents)
            {
                strBuilder.Append($"<i>{touchpadEvent.type} {touchpadEvent.state}</i>");
            }

            _statusText.text += strBuilder.ToString();
#endif
        }

        /// <summary>
        /// Handles the event for bumper.
        /// </summary>
        /// <param name="obj">Input Callback</param>
        private void HandleOnBumper(InputAction.CallbackContext obj)
        {
            bool bumperDown = obj.ReadValueAsButton();

            Debug.Log("Bumper was released this frame: " + obj.action.WasReleasedThisFrame());
        }

        private void HandleOnTrigger(InputAction.CallbackContext obj)
        {
            float triggerValue = obj.ReadValue<float>();
        }

        private void HandleOnTouchpad(InputAction.CallbackContext obj)
        {
            Vector2 triggerValue = obj.ReadValue<Vector2>();
        }

        private void HandleOnTriggerEvent(ushort controllerId, InputSubsystem.Extensions.Controller.MLInputControllerTriggerEvent triggerEvent, float depth)
        {
            Debug.Log($"Received trigger event: {triggerEvent} with trigger depth: {depth}, on controller id: {controllerId} ");
        }
    }
}
