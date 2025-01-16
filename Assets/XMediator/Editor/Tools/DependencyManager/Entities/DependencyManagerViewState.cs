using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace XMediator.Editor.Tools.DependencyManager.Entities
{
    internal class DependencyManagerViewState
    {
        internal static readonly DependencyManagerViewState Blank =
            new DependencyManagerViewState(null, new Dependency[] { });

        internal Dependency Sdk { get; }
        internal SortedSet<Dependency> Adapters { get; }
        internal string InstalledVersionsStatusMessage { get; }
        internal string AvailableVersionsStatusMessage { get; }
        internal string BrokenDependenciesStatusMessage { get; }
        internal bool ShowRetryLoadInstalledVersions { get; }
        internal bool ShowRetryFetchAvailableVersions { get; }
        internal bool IsDownloading { get; }

        internal bool ShowBrokenDependenciesStatusMessage =>
            InstalledVersionsStatusMessage == null && AvailableVersionsStatusMessage == null
                                                   && BrokenDependenciesStatusMessage != null;

        internal DependencyManagerViewState(Dependency sdk, IEnumerable<Dependency> adapters,
            bool isDownloading = false,
            string installedVersionsStatusMessage = null, string availableVersionsStatusMessage = null,
            bool showRetryLoadInstalledVersions = false, bool showRetryFetchAvailableVersions = false,
            string brokenDependenciesStatusMessage = null)
        {
            Sdk = sdk;
            Adapters = new SortedSet<Dependency>(adapters, new DependencyComparer());
            IsDownloading = isDownloading;
            InstalledVersionsStatusMessage = installedVersionsStatusMessage;
            AvailableVersionsStatusMessage = availableVersionsStatusMessage;
            ShowRetryLoadInstalledVersions = showRetryLoadInstalledVersions;
            ShowRetryFetchAvailableVersions = showRetryFetchAvailableVersions;
            BrokenDependenciesStatusMessage = brokenDependenciesStatusMessage;
        }

        internal static DependencyManagerViewState Build(
            InstalledVersions installedVersions,
            AvailableDependencies availableDependencies,
            bool isDownloading = false,
            string installedVersionsStatusMessage = null,
            string availableVersionsStatusMessage = null,
            bool showRetryLoadInstalledVersions = false,
            bool showRetryFetchAvailableVersions = false
        )
        {
            var brokenConstraintsReport = installedVersions.ReportBrokenConstraints();

            var sdk = installedVersions.Sdk != null
                ? Dependency.Build(
                    installedVersion: installedVersions.Sdk,
                    availableVersions: availableDependencies.Sdk != null ? availableDependencies.Sdk.Versions : new List<AvailableVersion>(),
                    brokenConstraints: FindBrokenConstraints(installedVersions.Sdk?.Name, brokenConstraintsReport), 
                    selectedVersion: availableDependencies.Sdk?.SelectedVersion)
                : null;

            var installed = installedVersions.Adapters.Select(installedVersion =>
            {
                var availableDependency = availableDependencies.GetByName(installedVersion.Name ?? "");
                var availableVersions = new List<AvailableVersion>();
                if (availableDependency != null)
                    availableVersions = availableDependency.Versions.ToList();

                return Dependency.Build(
                    installedVersion: installedVersion,
                    availableVersions: availableVersions,
                    brokenConstraints: FindBrokenConstraints(installedVersion.Name, brokenConstraintsReport),
                    selectedVersion: availableDependency?.SelectedVersion
                );
            });

            var available = availableDependencies.Adapters
                .Where(availableVersion => installedVersions.GetByName(availableVersion.Name) == null)
                .Select(availableVersion =>
                    {
                        var availableDependency = availableDependencies.GetByName(availableVersion.Name ?? "");
                        return Dependency.Build(
                            installedVersion: installedVersions.GetByName(availableVersion.Name),
                            availableVersions: availableDependencies.GetByName(availableVersion.Name).Versions,
                            selectedVersion: availableDependency?.SelectedVersion
                        );
                    }
                   
                );
            return new DependencyManagerViewState(
                sdk: sdk,
                adapters: installed.Concat(available),
                isDownloading: isDownloading,
                installedVersionsStatusMessage: installedVersionsStatusMessage,
                availableVersionsStatusMessage: availableVersionsStatusMessage,
                showRetryLoadInstalledVersions: showRetryLoadInstalledVersions,
                showRetryFetchAvailableVersions: showRetryFetchAvailableVersions
            );
        }

        private static HashSet<string> FindBrokenConstraints(string versionName,
            Dictionary<string, HashSet<string>> brokenConstraintsReport)
        {
            if (versionName != null && brokenConstraintsReport.ContainsKey(versionName))
            {
                return brokenConstraintsReport[versionName];
            }

            return null;
        }

        protected bool Equals(DependencyManagerViewState other)
        {
            return Adapters.SequenceEqual(other.Adapters) &&
                   InstalledVersionsStatusMessage == other.InstalledVersionsStatusMessage &&
                   AvailableVersionsStatusMessage == other.AvailableVersionsStatusMessage &&
                   ShowRetryLoadInstalledVersions == other.ShowRetryLoadInstalledVersions &&
                   ShowRetryFetchAvailableVersions == other.ShowRetryFetchAvailableVersions &&
                   IsDownloading == other.IsDownloading;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DependencyManagerViewState) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Adapters != null ? Adapters.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^
                           (InstalledVersionsStatusMessage != null ? InstalledVersionsStatusMessage.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^
                           (AvailableVersionsStatusMessage != null ? AvailableVersionsStatusMessage.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ ShowRetryLoadInstalledVersions.GetHashCode();
                hashCode = (hashCode * 397) ^ ShowRetryFetchAvailableVersions.GetHashCode();
                hashCode = (hashCode * 397) ^ IsDownloading.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return
                $"{nameof(Adapters)}: {string.Join(", ", Adapters)}, {nameof(InstalledVersionsStatusMessage)}: {InstalledVersionsStatusMessage}, {nameof(AvailableVersionsStatusMessage)}: {AvailableVersionsStatusMessage}, {nameof(ShowRetryLoadInstalledVersions)}: {ShowRetryLoadInstalledVersions}, {nameof(ShowRetryFetchAvailableVersions)}: {ShowRetryFetchAvailableVersions}, {nameof(IsDownloading)}: {IsDownloading}";
        }

        private class DependencyComparer : IComparer<Dependency>
        {
            readonly CaseInsensitiveComparer _caseiComp = new CaseInsensitiveComparer();

            public int Compare(Dependency x, Dependency y)
            {
                var xExt = x.Name;
                var yExt = y.Name;
                
                int vExt = _caseiComp.Compare(xExt, yExt);
                if (vExt != 0)
                {
                    return vExt;
                }
                else
                {
                    return _caseiComp.Compare(x, y);
                }
            }
        }
    }
}