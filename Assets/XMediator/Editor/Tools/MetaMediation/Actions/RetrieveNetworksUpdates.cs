using System;
using System.Collections.Generic;
using System.Linq;
using XMediator.Editor.Tools.MetaMediation.Entities;
using XMediator.Editor.Tools.MetaMediation.Repository;
using XMediator.Editor.Tools.MetaMediation.Repository.Dto;

namespace XMediator.Editor.Tools.MetaMediation.Actions
{
    internal class RetrieveNetworksUpdates
    {
        private readonly IMetaMediationDependenciesRepository _mmRepository;

        internal RetrieveNetworksUpdates(IMetaMediationDependenciesRepository metaMediationDependenciesRepository)
        {
            _mmRepository = metaMediationDependenciesRepository ?? throw new ArgumentNullException(nameof(metaMediationDependenciesRepository));
        }

        internal (Dictionary<MetaMediationDependencies.AndroidPackage, AndroidDependencyDto> AndroidDiff,
            Dictionary<MetaMediationDependencies.IosPod, IOSDependencyDto> IosDiff)
            Invoke(ManifestDto<IOSDependencyDto> iosManifest, ManifestDto<AndroidDependencyDto> androidManifest)
        {
            var metaMediationDependencies = _mmRepository.Invoke();
            if (metaMediationDependencies == null)
            {
                return (null, null);
            }

            var androidDiff = new Dictionary<MetaMediationDependencies.AndroidPackage, AndroidDependencyDto>();
            var iosDiff = new Dictionary<MetaMediationDependencies.IosPod, IOSDependencyDto>();

            if (androidManifest != null)
            {
                ExtractAndroidUpdates(androidManifest, metaMediationDependencies, androidDiff);
            }
            if (iosManifest != null)
            {
                ExtractIosUpdates(iosManifest, metaMediationDependencies, iosDiff);                
            }

            return (androidDiff, iosDiff);
        }

        private static void ExtractAndroidUpdates(ManifestDto<AndroidDependencyDto> manifest, MetaMediationDependencies metaMediationDependencies,
            IDictionary<MetaMediationDependencies.AndroidPackage, AndroidDependencyDto> androidDiff)
        {
            foreach (var androidPackage in metaMediationDependencies.AndroidPackages)
            {
                ExtractUpdatesFromCollection(manifest.core, androidPackage, androidDiff);
                ExtractUpdatesFromCollection(manifest.standalone_metamediation_adapters?.SelectMany(a => a.dependencies), androidPackage, androidDiff);
                ExtractUpdatesFromCollection(manifest.networks?.SelectMany(n => n.dependencies), androidPackage, androidDiff);
                ExtractUpdatesFromCollection(manifest.networks?.SelectMany(n => n.adapters).SelectMany(a => a.dependencies), androidPackage, androidDiff);
            }
        }

        private static void ExtractIosUpdates(ManifestDto<IOSDependencyDto> manifest, MetaMediationDependencies metaMediationDependencies,
            IDictionary<MetaMediationDependencies.IosPod, IOSDependencyDto> iosDiff)
        {
            foreach (var iosPod in metaMediationDependencies.IosPods)
            {
                ExtractUpdatesFromCollection(manifest.core, iosPod, iosDiff);
                ExtractUpdatesFromCollection(manifest.standalone_metamediation_adapters.SelectMany(n => n.dependencies), iosPod, iosDiff);
                ExtractUpdatesFromCollection(manifest.networks.SelectMany(n => n.dependencies), iosPod, iosDiff);
                ExtractUpdatesFromCollection(manifest.networks.SelectMany(n => n.adapters).SelectMany(a => a.dependencies), iosPod, iosDiff);
            }
        }

        private static void ExtractUpdatesFromCollection<TPackage, TDependency>(
            IEnumerable<TDependency> dependencies,
            TPackage package,
            IDictionary<TPackage, TDependency> diff)
            where TPackage : class
            where TDependency : class
        {
            var updatedDependency = dependencies?.FirstOrDefault(d => IsUpdateAvailable(d, package));
            if (updatedDependency != null)
            {
                diff.TryAdd(package, updatedDependency);
            }
        }

        private static bool IsUpdateAvailable(object dependency, object package)
        {
            if (dependency == null || package == null)
            {
                return false;
            }

            return (dependency, package) switch
            {
                (AndroidDependencyDto androidDep, MetaMediationDependencies.AndroidPackage androidPkg) =>
                    androidDep.group_id == androidPkg.Group &&
                    androidDep.artifact_name == androidPkg.Artifact &&
                    androidDep.suggested_version != androidPkg.Version,

                (IOSDependencyDto iosDep, MetaMediationDependencies.IosPod iosPod) =>
                    iosDep.pod == iosPod.Name &&
                    iosDep.suggested_version != iosPod.Version,

                _ => false
            };
        }
    }
}