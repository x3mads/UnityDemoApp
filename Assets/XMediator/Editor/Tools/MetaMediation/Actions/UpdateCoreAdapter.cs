using System;
using System.Linq;
using UnityEngine;
using XMediator.Editor.Tools.DependencyManager.Entities;
using XMediator.Editor.Tools.MetaMediation.Entities;
using static XMediator.Editor.Tools.MetaMediation.DependencyVersionResolver;

namespace XMediator.Editor.Tools.MetaMediation.Actions
{
    internal class UpdateCoreAdapter
    {
        private ResolveCoreAdapterVersion ResolveCoreAdapterVersion { get; }
        private IInstallAdapter InstallPackage { get; }
        private IRetrieveDependencyAdapter RetrievePackage { get; }
        
        public UpdateCoreAdapter(ResolveCoreAdapterVersion resolveCoreAdapterVersion, IInstallAdapter installPackage, IRetrieveDependencyAdapter retrievePackage)
        {
            ResolveCoreAdapterVersion = resolveCoreAdapterVersion;
            InstallPackage = installPackage;
            RetrievePackage = retrievePackage;
        }

        public void Invoke(AvailableDependency availableDependency, SemanticVersion iosCoreVersion,
            SemanticVersion androidCoreVersion,
            Action onSuccess, Action<string> onError)
        {
            try {
                var result = ResolveCoreAdapterVersion.Invoke(availableDependency, iosCoreVersion, androidCoreVersion);
                if (result.Status == ResolveCoreAdapterVersion.Status.NoChanges)
                {
                    Debug.Log("XMediator Core is already up to date.");
                    onSuccess();
                    return;
                }

                RetrievePackage.Invoke(result.Version,
                    onSuccess: path => { InstallPackage.Invoke(path, onSuccess, onError: onError); },
                    onError: error => { onError("An error occurred downloading the XMediator Core package: " + error); });
            }
            catch (Exception exception)
            {
                onError(exception.Message);
                return;
            }
        }
    }

    internal class ResolveCoreAdapterVersion
    {
        internal ResolveCoreAdapterVersion(XmlDependencyVersions currentCoreVersion)
        {
            CurrentCoreVersion = currentCoreVersion;
        }

        private XmlDependencyVersions CurrentCoreVersion { get; }

        internal Result Invoke(AvailableDependency availableDependency, SemanticVersion iosCoreVersion,
            SemanticVersion androidCoreVersion)
        {
            // Check if at least one platform is selected 
            if (iosCoreVersion == null && androidCoreVersion == null)
            {
                throw new Exception("You must select at least one Platform (Android or iOS)");
            }

            // Filter versions with the same major version as androidCoreVersion and iosCoreVersion and order by version
            var filteredVersions = availableDependency.Versions.Where(v =>
                ValidateMajorOrNull(androidCoreVersion, v.AndroidVersion) &&
                ValidateMajorOrNull(iosCoreVersion, v.IOSVersion)
            ).OrderByDescending(v => v.Version);

            // Find the first version with minor versions >= androidCoreVersion and iosCoreVersion
            var selectedVersion = filteredVersions.FirstOrDefault(v =>
            {
                var isAndroidVersionCompatible = ValidateLowerEqualOrNull(androidCoreVersion, v.AndroidVersion);
                var isIOSVersionCompatible = ValidateLowerEqualOrNull(iosCoreVersion, v.IOSVersion);

                return isAndroidVersionCompatible && isIOSVersionCompatible;
            });

            if (selectedVersion == null)
            {
                throw new Exception("No version found that matches XMediator Core version, contact the Support Team");
            }

            return new Result(GetStatus(selectedVersion), selectedVersion, CurrentCoreVersion.Version);
        }

        private Status GetStatus(AvailableVersion selectedVersion)
        {
            if (CurrentCoreVersion.Version.Equals(selectedVersion.Version))
            {
                return Status.NoChanges;
            }

            return CurrentCoreVersion.Version.IsLowerThan(selectedVersion.Version) ? Status.Upgrade : Status.Downgrade;
        }

        private static bool ValidateLowerEqualOrNull(SemanticVersion coreVersion, string proxyVersion)
        {
            return coreVersion == null || new SemanticVersion(proxyVersion).IsLowerOrEqualThan(coreVersion);
        }

        private static bool ValidateMajorOrNull(SemanticVersion coreVersion, string proxyVersion)
        {
            return coreVersion == null || new SemanticVersion(proxyVersion).IsMajorVersionEqual(coreVersion);
        }

        internal class Result
        {
            internal Status Status;
            internal AvailableVersion Version;
            internal SemanticVersion PreviousVersion;

            public Result(Status status, AvailableVersion version, SemanticVersion previousVersion)
            {
                Status = status;
                Version = version;
                PreviousVersion = previousVersion;
            }
        }

        internal enum Status
        {
            Upgrade,
            NoChanges,
            Downgrade
        }
    }
}