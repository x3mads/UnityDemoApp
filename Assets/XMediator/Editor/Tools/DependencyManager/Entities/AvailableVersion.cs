using System;

namespace XMediator.Editor.Tools.DependencyManager.Entities
{
    internal class AvailableVersion : IComparable<AvailableVersion>
    {
        public string Name { get; }
        public string Description { get; }
        public SemanticVersion Version { get; }
        public string AndroidVersion { get; }
        public string IOSVersion { get; }
        public string DownloadUrl { get; }
        public string DownloadFilename { get; }
        public bool IsCompliant { get; }
        public string[] NonCompliantRules { get; }

        public AvailableVersion(string name, string description, SemanticVersion version, string androidVersion,
            string iOSVersion, string downloadUrl,
            string downloadFilename, bool isCompliant, string[] nonCompliantRules = null)
        {
            Name = name;
            Description = description;
            Version = version;
            AndroidVersion = androidVersion;
            IOSVersion = iOSVersion;
            DownloadUrl = downloadUrl;
            DownloadFilename = downloadFilename;
            IsCompliant = isCompliant;
            NonCompliantRules = nonCompliantRules;
        }

        public AvailableVersion(Dependency dependency)
        {
            Name = dependency.Name;
            Description = dependency.Description;
            Version = dependency.LatestVersion;
            AndroidVersion = dependency.LatestAndroidVersion;
            IOSVersion = dependency.InstalledIOSVersion;
            DownloadUrl = dependency.LatestVersionDownloadUrl;
            DownloadFilename = dependency.LatestVersionDownloadFilename;
        }

        public int CompareTo(AvailableVersion other)
        {
            return other == null ? 1 : Version.Version.CompareTo(other.Version.Version);
        }
    }
}