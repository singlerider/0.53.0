// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
//
// Copyright (c) 2022 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor.XR.MagicLeap
{
    [Serializable]
    internal struct Permission
    {
        public enum ProtectionLevel
        {
            Normal,
            Dangerous
        }

        public static readonly Dictionary<ProtectionLevel, string> LevelDescriptions = new Dictionary<ProtectionLevel, string>()
        {
            { ProtectionLevel.Normal, "Normal permissions will be autmatically granted at install time if included in AndroidManifest.xml." },
            { ProtectionLevel.Dangerous, "Dangerous permissions will require an additional runtime request from the app, and the user will have the option to deny the permission." }
        };

        [SerializeField]
        private string name;
        [SerializeField]
        private bool enabled;
        [SerializeField]
        private string description;
        [SerializeField]
        private ProtectionLevel level;
        [SerializeField]
        private int apiLevel;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public ProtectionLevel Level
        {
            get { return level; }
            set { level = value; }
        }

        public int MinimumApiLevel
        {
            get { return apiLevel; }
            set { apiLevel = value; }
        }

        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }
    }
}