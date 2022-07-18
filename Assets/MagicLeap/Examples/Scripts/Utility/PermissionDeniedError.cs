// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="PermissionDeniedError.cs" company="Magic Leap, Inc">
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
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace MagicLeap.Examples
{
    /// <summary>
    /// Helper script to manage a permission denied error pop up
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class PermissionDeniedError : MonoBehaviour
    {
        /// <summary>
        /// The time the error popup will display before it destroys itself
        /// </summary>
        public float TimeToDisplay = 10.0f;

        /// <summary>
        /// Starts the coroutine that will ultimately destroy this game object and creates the headpose canvas tracker
        /// </summary>
        void Awake()
        {
            StartCoroutine(DestroyAfterTime(TimeToDisplay));
            GetComponent<Canvas>().worldCamera = Camera.main;
            MLHeadposeCanvasBehavior headposeCanvas = gameObject.AddComponent<MLHeadposeCanvasBehavior>();
            headposeCanvas.CanvasDistanceForwards = 1.0f;
            headposeCanvas.CanvasDistanceUpwards = 0.0f;
            headposeCanvas.PositionLerpSpeed = 1.0f;
            headposeCanvas.RotationLerpSpeed = 1.0f;
        }

        IEnumerator DestroyAfterTime(float timeInSeconds)
        {
            yield return new WaitForSeconds(timeInSeconds);
            Destroy(gameObject);
        }
    }
}
