using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace XMediator.Editor.Tools.DependencyManager.Entities
{
    internal class Dependency
    {
        internal string Name { get; }
        internal string Description { get; }
        internal bool IsInstalled { get; }
        internal bool IsUpdateAvailable { get; }
        internal long? InstalledAt { get; }
        internal bool IsCompliant { get; }
        [CanBeNull] internal SemanticVersion InstalledVersion { get; }
        [CanBeNull] internal SemanticVersion LatestVersion { get; set; }
        [CanBeNull] internal string InstalledAndroidVersion { get; }
        [CanBeNull] internal string InstalledIOSVersion { get; }
        [CanBeNull] internal string LatestVersionDownloadUrl { get; set; }
        [CanBeNull] internal string LatestVersionDownloadFilename { get; set; }
        [CanBeNull] internal string LatestAndroidVersion { get; set; }
        [CanBeNull] internal string LatestIOSVersion { get; set; }
        [CanBeNull] internal HashSet<string> BrokenConstraints { get; }
        [CanBeNull] internal IEnumerable<AvailableVersion> AvailableVersions { get; }
        [CanBeNull] internal string[] NonCompliantRules { get; }

        internal Dependency(string name,
            string description,
            bool isInstalled,
            bool isUpdateAvailable,
            bool isCompliant,
            long? installedAt = null,
            SemanticVersion installedVersion = null,
            string installedAndroidVersion = null,
            string installedIOSVersion = null,
            SemanticVersion latestVersion = null,
            string latestVersionDownloadUrl = null,
            string latestVersionDownloadFilename = null,
            string latestAndroidVersion = null,
            string latestIOSVersion = null,
            HashSet<string> brokenConstraints = null,
            IEnumerable<AvailableVersion> availableVersions = null,
            string[] nonCompliantRules = null)
        {
            Name = name;
            Description = description;
            IsInstalled = isInstalled;
            InstalledAt = installedAt;
            IsCompliant = isCompliant;
            InstalledVersion = installedVersion;
            InstalledAndroidVersion = installedAndroidVersion;
            InstalledIOSVersion = installedIOSVersion;
            IsUpdateAvailable = isUpdateAvailable;
            LatestVersion = latestVersion;
            LatestVersionDownloadUrl = latestVersionDownloadUrl;
            LatestVersionDownloadFilename = latestVersionDownloadFilename;
            LatestAndroidVersion = latestAndroidVersion;
            LatestIOSVersion = latestIOSVersion;
            BrokenConstraints = brokenConstraints;
            AvailableVersions = availableVersions;
            NonCompliantRules = nonCompliantRules;
        }

        internal static Dependency Build(InstalledVersion installedVersion,
            IEnumerable<AvailableVersion> availableVersions,
            HashSet<string> brokenConstraints = null,
            AvailableVersion selectedVersion = null)
        {
            var availableVersionsList = availableVersions.ToList();
            AvailableVersion availableVersion = null;
            if (availableVersionsList.Count > 0)
                availableVersion = availableVersionsList.First();
            var isUpdateAvailable = (availableVersion != null && installedVersion != null) &&
                                    installedVersion.Version.NotEquals(selectedVersion?.Version ??
                                                                       availableVersion.Version);
            return new Dependency(
                name: availableVersion?.Name ?? installedVersion?.Name,
                description: availableVersion?.Description ?? installedVersion?.Description,
                isInstalled: installedVersion != null,
                isCompliant: ResolveIsCompliant(installedVersion, availableVersionsList),
                installedVersion: installedVersion?.Version,
                installedAndroidVersion: installedVersion?.InstalledAndroidVersion ??
                                         (installedVersion != null && !isUpdateAvailable
                                             ? availableVersion?.AndroidVersion
                                             : null),
                installedIOSVersion: installedVersion?.InstalledIOSVersion ??
                                     (installedVersion != null && !isUpdateAvailable
                                         ? availableVersion?.IOSVersion
                                         : null),
                installedAt: installedVersion?.InstalledAt,
                isUpdateAvailable: isUpdateAvailable,
                latestVersion: selectedVersion?.Version ?? availableVersion?.Version,
                latestVersionDownloadUrl: selectedVersion?.DownloadUrl ?? availableVersion?.DownloadUrl,
                latestVersionDownloadFilename: selectedVersion?.DownloadFilename ?? availableVersion?.DownloadFilename,
                latestAndroidVersion: selectedVersion?.AndroidVersion ?? availableVersion?.AndroidVersion,
                latestIOSVersion: selectedVersion?.IOSVersion ?? availableVersion?.IOSVersion,
                brokenConstraints: brokenConstraints,
                availableVersions: availableVersionsList,
                nonCompliantRules: availableVersion?.NonCompliantRules
            );
        }

        internal void UpdateLatestVersion(AvailableVersion latestAvailableVersion)
        {
            LatestVersion = latestAvailableVersion.Version;
            LatestAndroidVersion = latestAvailableVersion.AndroidVersion;
            LatestIOSVersion = latestAvailableVersion.IOSVersion;
            LatestVersionDownloadFilename = latestAvailableVersion.DownloadFilename;
            LatestVersionDownloadUrl = latestAvailableVersion.DownloadUrl;
        }

        protected bool Equals(Dependency other)
        {
            return Name == other.Name && Description == other.Description && IsInstalled == other.IsInstalled &&
                   IsUpdateAvailable == other.IsUpdateAvailable && InstalledAt == other.InstalledAt &&
                   Equals(InstalledVersion, other.InstalledVersion) && Equals(LatestVersion, other.LatestVersion) &&
                   LatestVersionDownloadUrl == other.LatestVersionDownloadUrl &&
                   LatestVersionDownloadFilename == other.LatestVersionDownloadFilename;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Dependency) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Description != null ? Description.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ IsInstalled.GetHashCode();
                hashCode = (hashCode * 397) ^ IsUpdateAvailable.GetHashCode();
                hashCode = (hashCode * 397) ^ InstalledAt.GetHashCode();
                hashCode = (hashCode * 397) ^ (InstalledVersion != null ? InstalledVersion.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (LatestVersion != null ? LatestVersion.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^
                           (LatestVersionDownloadUrl != null ? LatestVersionDownloadUrl.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^
                           (LatestVersionDownloadFilename != null ? LatestVersionDownloadFilename.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return
                $"<{nameof(Name)}: {Name}, {nameof(Description)}: {Description}, {nameof(IsInstalled)}: {IsInstalled}, {nameof(IsUpdateAvailable)}: {IsUpdateAvailable}, {nameof(InstalledAt)}: {InstalledAt}, {nameof(InstalledVersion)}: {InstalledVersion}, {nameof(LatestVersion)}: {LatestVersion}, {nameof(LatestVersionDownloadUrl)}: {LatestVersionDownloadUrl}, {nameof(LatestVersionDownloadFilename)}: {LatestVersionDownloadFilename}>";
        }

        private static bool ResolveIsCompliant(InstalledVersion installedVersion, List<AvailableVersion> availableVersionsList)
        {
            var matchingAvailableVersion = availableVersionsList.Find(x => x.Version.Equals(installedVersion?.Version));
            return matchingAvailableVersion == null || matchingAvailableVersion.IsCompliant;
        }
    }
}