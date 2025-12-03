using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XMediator.Editor.Tools.MetaMediation.Actions;
using XMediator.Editor.Tools.MetaMediation.Entities;
using XMediator.Editor.Tools.MetaMediation.Repository;

namespace XMediator.Editor.Tools.MetaMediation.View
{
    internal class VersionsCheckerPresenter
    {
        private SelectableDependencies _selectableDependencies;
        private MetaMediationDependencies _currentDependencies;
        private readonly Action _onDataReady;

        internal bool IsReady { get; private set; }

        internal IEnumerable<string> Mediators => _selectableDependencies?.Mediations ?? Enumerable.Empty<string>();
        internal IEnumerable<string> Networks => _selectableDependencies?.Networks ?? Enumerable.Empty<string>();
        internal IEnumerable<string> Tools => _selectableDependencies?.Tools?.Keys ?? Enumerable.Empty<string>();

        internal VersionsCheckerPresenter(Action onDataReady)
        {
            _onDataReady = onDataReady;
            LoadData();
        }

        private void LoadData()
        {
            var settingsRepo = new SettingsRepository();
            var getSelectable = new GetDependencies(new DependencyRepository(), settingsRepo);
            getSelectable.Invoke(
                onSuccess: deps =>
                {
                    _selectableDependencies = deps;
                    _currentDependencies = new MetaMediationDependenciesRepository().Invoke();
                    IsReady = true;
                    _onDataReady?.Invoke();
                },
                onError: ex =>
                {
                    Debug.LogError($"[VersionsChecker] Failed to load data: {ex.Message}");
                    IsReady = true; // still allow UI to show error state
                    _onDataReady?.Invoke();
                });
        }

        // Public helper to get list of entries with versions applying search & filter rules

        internal IEnumerable<(string name, string android, string ios)> BuildEntries(IEnumerable<string> names, string searchTerm)
        {
            if (!IsReady || names == null) yield break;
            var normSearch = searchTerm?.ToLower() ?? string.Empty;
            foreach (var name in names.OrderBy(n => n))
            {
                if (!string.IsNullOrEmpty(normSearch) && !name.ToLower().Contains(normSearch)) continue;
                var android = GetAndroidVersion(name);
                var ios = GetIosVersion(name);
                if (android == "N/A" && ios == "N/A") continue;
                yield return (name, android, ios);
            }
        }

        private string GetAndroidVersion(string dependencyName)
        {
            if (_currentDependencies == null) return "N/A";

            switch (dependencyName)
            {
                case "X3M":
                    return FindAndroid("com.x3mads.android.xmediator", "core")?.Version ?? "N/A";
                case "MAX":
                    return FindAndroid("com.applovin", "applovin-sdk")?.Version ?? "N/A";
                case "GoogleAds":
                    return FindAndroid("com.google.android.gms", "play-services-ads")?.Version ?? "N/A";
                case "LevelPlay":
                    return FindAndroid("com.unity3d.ads-mediation", "mediation-sdk")?.Version ?? "N/A";
                case "FairBid":
                    return FindAndroid("com.x3mads.android.xmediator.mediation", "fairbid")?.Version ?? "N/A";
            }

            var manifest = _selectableDependencies?.AndroidManifest?.GetDefaultManifest();
            var dep = manifest?.standalone_metamediation_adapters?.FirstOrDefault(m => m.mediator == dependencyName)?.dependencies?.FirstOrDefault()
                      ?? manifest?.networks?.FirstOrDefault(n => n.network == dependencyName || n.display_name == dependencyName)?.dependencies?.FirstOrDefault()
                      ?? manifest?.additional_tools?.FirstOrDefault(t => t.tool == dependencyName || t.display_name == dependencyName)?.dependencies?.FirstOrDefault();
            string version = null;
            if (dep != null)
            {
                var pkg = _currentDependencies.AndroidPackages?.FirstOrDefault(p => p.Group == dep.group_id && p.Artifact == dep.artifact_name);
                version = pkg?.Version;
            }
            if (version == null)
            {
                var norm = dependencyName.ToLower().Replace(" ", "");
                var pkg = _currentDependencies.AndroidPackages?.FirstOrDefault(p => p.Artifact.ToLower().Contains(norm) || p.Group.ToLower().Contains(norm));
                version = pkg?.Version;
            }
            return string.IsNullOrEmpty(version) ? "N/A" : version;

            MetaMediationDependencies.AndroidPackage FindAndroid(string group, string artifact) =>
                _currentDependencies.AndroidPackages?.FirstOrDefault(p => p.Group == group && p.Artifact == artifact);
        }

        private string GetIosVersion(string dependencyName)
        {
            if (_currentDependencies == null) return "N/A";
            MetaMediationDependencies.IosPod FindPod(string pod) =>
                _currentDependencies.IosPods?.FirstOrDefault(p => p.Name == pod);
            switch (dependencyName)
            {
                case "X3M":
                    return FindPod("XMediator")?.Version ?? "N/A";
                case "MAX":
                    return FindPod("AppLovinSDK")?.Version ?? "N/A";
                case "GoogleAds":
                    return FindPod("Google-Mobile-Ads-SDK")?.Version ?? "N/A";
                case "LevelPlay":
                    return FindPod("IronSourceSDK")?.Version ?? "N/A";
                case "FairBid":
                    return FindPod("XMediatorFairBid")?.Version ?? "N/A";
            }
            var manifest = _selectableDependencies?.IOSManifest?.GetDefaultManifest();
            var dep = manifest?.standalone_metamediation_adapters?.FirstOrDefault(m => m.mediator == dependencyName)?.dependencies?.FirstOrDefault()
                      ?? manifest?.networks?.FirstOrDefault(n => n.network == dependencyName || n.display_name == dependencyName)?.dependencies?.FirstOrDefault()
                      ?? manifest?.additional_tools?.FirstOrDefault(t => t.tool == dependencyName || t.display_name == dependencyName)?.dependencies?.FirstOrDefault();
            string version = null;
            if (dep != null)
            {
                var pod = _currentDependencies.IosPods?.FirstOrDefault(p => p.Name == dep.pod);
                version = pod?.Version;
            }

            if (version != null) return string.IsNullOrEmpty(version) ? "N/A" : version;
            {
                var norm = dependencyName.ToLower().Replace(" ", "");
                var pod = _currentDependencies.IosPods?.FirstOrDefault(p => p.Name.ToLower().Contains(norm));
                version = pod?.Version;
            }
            return string.IsNullOrEmpty(version) ? "N/A" : version;
        }
    }
}
