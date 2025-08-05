using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XMediator.Editor.Tools.MetaMediation.Entities;
using XMediator.Editor.Tools.MetaMediation.Repository;
using XMediator.Editor.Tools.MetaMediation.Repository.Dto;

namespace XMediator.Editor.Tools.MetaMediation.Actions
{
    internal class GetDependencies
    {
        private readonly DependencyRepository _repository;
        private readonly SettingsRepository _settingsRepository; 

        public GetDependencies(DependencyRepository repository, SettingsRepository settingsRepository)
        {
            _repository = repository;
            _settingsRepository = settingsRepository;
        }

        public void Invoke(Action<SelectableDependencies> onSuccess, Action<Exception> onError)
        {
            _repository.DownloadAndParseJson(
                _settingsRepository.RetrieveIOSVersionsUrl(),
                _settingsRepository.RetrieveAndroidVersionsUrl(),
                onSuccess: platformsDependency => { processOnSuccess(onSuccess, onError, platformsDependency); },
                onError: onError
            );
        }

        private static void processOnSuccess(Action<SelectableDependencies> onSuccess, Action<Exception> onError,
            DependencyRepository.PlatformsDependency platformsDependency)
        {
            var networks = new HashSet<string>();
            var mediations = new HashSet<string>();
            if (platformsDependency != null)
            {
                var iosManifest = platformsDependency.IOSManifest;
                ExtractMediationAndNetworks(iosManifest, mediations, networks);
                var androidManifest = platformsDependency.AndroidManifest;
                ExtractMediationAndNetworks(androidManifest, mediations, networks);

                onSuccess.Invoke(new SelectableDependencies(networks: networks, mediations: mediations, iosManifest: iosManifest,
                    androidManifest: androidManifest
                ));
            }
            else
            {
                Debug.LogError("Failed to download one or both manifests.");
                onError.Invoke(new Exception("Failed to download"));
            }
        }

        private static void ExtractMediationAndNetworks<T>(FlavoredManifestDto<T> flavoredManifest,
            ISet<string> mediations,
            ISet<string> networks)
        {
            var manifest = flavoredManifest.GetDefaultManifest();
            
            mediations.Add(SelectableDependencies.X3MMediationName);
            foreach (var standaloneAdapter in manifest.standalone_metamediation_adapters)
            {
                mediations.Add(standaloneAdapter.metamediation_adapter_for);
            }

            foreach (var network in manifest.networks)
            {
                networks.Add(network.display_name);
                foreach (var mediationAdapter in network.adapters.Where(mediationAdapter =>
                             mediationAdapter.metamediation_adapter_for != null))
                {
                    mediations.Add(mediationAdapter.metamediation_adapter_for);
                }
            }
        }
    }
}