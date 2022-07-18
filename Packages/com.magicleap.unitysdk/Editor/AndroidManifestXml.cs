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
using System.Text;
using System.Xml;
using UnityEditor;

namespace UnityEditor.XR.MagicLeap
{
    internal class AndroidManifestXml : XmlDocument
    {
        private readonly XmlElement activityElement;

        public static string AssetPath = "Assets/Plugins/Android/AndroidManifest.xml";

        public const string AndroidXmlNamespace = "http://schemas.android.com/apk/res/android";
        protected XmlNamespaceManager nsMgr;
        private string path;
        XmlNodeList permissionNodes;

        public AndroidManifestXml(string path)
        {
            this.path = path;
            using (var reader = new XmlTextReader(this.path))
            {
                reader.Read();
                base.Load(reader);
            }

            activityElement = SelectSingleNode("/manifest") as XmlElement;

            nsMgr = new XmlNamespaceManager(NameTable);
            nsMgr.AddNamespace("android", AndroidXmlNamespace);

            permissionNodes = SelectNodes("manifest/uses-permission", nsMgr);
        }

        public string Save()
        {
            return SaveAs(path);
        }

        public string SaveAs(string path)
        {
            using (var writer = new XmlTextWriter(path, new UTF8Encoding(false)))
            {
                writer.Formatting = Formatting.Indented;
                Save(writer);
            }

            return path;
        }

        public string[] GetIncludedPermissions()
        {
            if(permissionNodes != null)
            {
                List<string> permissions = new List<string>();
                for(int i = 0; i < permissionNodes.Count; i++)
                {
                    var node = permissionNodes[i];
                    string name = node.Attributes["android:name"].Value;
                    permissions.Add(name);
                }
                return permissions.ToArray();
            }
            return new string[0];
        }

        public void AddPermission(string permissionName)
        {
            if(Array.IndexOf(GetIncludedPermissions(), permissionName) >= 0)
            {
                // permission already exists
                return;
            }
            XmlNode metadataTag = activityElement.AppendChild(CreateElement("uses-permission"));
            metadataTag.Attributes.Append(CreateAndroidAttribute("name", $"{permissionName}")); 
        }

        public void RemovePermission(string permissionName)
        {
            if (Array.IndexOf(GetIncludedPermissions(), permissionName) == -1)
            {
                // permission doesn't exists
                return;
            }
            List<XmlNode> matchingNodes = new List<XmlNode>();
            for (int i = 0; i < permissionNodes.Count; i++)
            {
                var node = permissionNodes[i];
                if (node.Attributes["android:name"].Value == permissionName)
                {
                    matchingNodes.Add(node);
                }
            }
            // remove all matching in case of duplicates
            foreach (var node in matchingNodes)
            {
                activityElement.RemoveChild(node);
            }
        }

        private XmlAttribute CreateAndroidAttribute(string key, string value)
        {
            XmlAttribute attr = CreateAttribute("android", key, AndroidXmlNamespace);
            attr.Value = value;
            return attr;
        }
    }
}
