using System;
using System.Linq;
using UnityEngine;
using XMediator.Editor.Tools.DependencyManager.Entities;

namespace XMediator.Editor.Tools.DependencyManager.Repository
{
    [Serializable]
    public class XMediatorSdkVersionsJson
    {
        public VersionJson sdk;
        public VersionJson[] adapters;

        public static XMediatorSdkVersionsJson CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<XMediatorSdkVersionsJson>(jsonString);
        }

        [Serializable]
        public class VersionJson
        {
            public string name;
            public string description;
            public string version;
            public string android_version;
            public string ios_version;
            public string download_url;
            public string download_filename;

            internal AvailableVersion ToAvailableVersion() => new AvailableVersion(
                name: name,
                description: description,
                version: new SemanticVersion(version),
                androidVersion: android_version,
                iOSVersion: ios_version,
                downloadUrl: download_url,
                downloadFilename: download_filename,
                isCompliant: true
            );
        }

        internal AvailableVersions ToAvailableVersions() =>
            new AvailableVersions(
                sdk: sdk.ToAvailableVersion(),
                adapters: adapters.Select(json => json.ToAvailableVersion())
            );
    }
}