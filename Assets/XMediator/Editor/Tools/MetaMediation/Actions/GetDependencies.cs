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

        public GetDependencies(DependencyRepository repository)
        {
            _repository = repository;
        }

        public void Invoke(Action<Dependencies> onSuccess, Action<Exception> onError)
        {
            _repository.DownloadAndParseJson(
                onSuccess: platformsDependency => { processOnSuccess(onSuccess, onError, platformsDependency); },
                onError: onError
            );
        }

        private static void processOnSuccess(Action<Dependencies> onSuccess, Action<Exception> onError,
            DependencyRepository.PlatformsDependency platformsDependency)
        {
            var networks = new HashSet<string>();
            var mediations = new HashSet<string>();
            if (platformsDependency != null)
            {
                var androidManifest = platformsDependency.AndroidManifest;
                ExtractMediationAndNetworksAndroid(androidManifest, mediations, networks);
                var iosManifest = platformsDependency.IOSManifest;
                ExtractMediationAndNetworksIOS(iosManifest, mediations, networks);

                onSuccess.Invoke(new Dependencies(networks: networks, mediations: mediations, iosManifest: iosManifest,
                    androidManifest: androidManifest
                ));
            }
            else
            {
                Debug.LogError("Failed to download one or both manifests.");
                onError.Invoke(new Exception("Failed to download"));
            }
        }

        private static void ExtractMediationAndNetworksAndroid(AndroidManifestDto androidManifest,
            ISet<string> mediations,
            ISet<string> networks)
        {
            foreach (var standaloneAdapter in androidManifest.standalone_metamediation_adapters)
            {
                mediations.Add(standaloneAdapter.metamediation_adapter_for);
            }

            foreach (var network in androidManifest.networks)
            {
                networks.Add(network.display_name);
                foreach (var mediationAdapter in network.adapters.Where(mediationAdapter =>
                             mediationAdapter.metamediation_adapter_for != null))
                {
                    mediations.Add(mediationAdapter.metamediation_adapter_for);
                }
            }
        }

        private static void ExtractMediationAndNetworksIOS(IOSManifestDto iosManifest, ISet<string> mediations,
            ISet<string> networks)
        {
            foreach (var standaloneAdapter in iosManifest.standalone_metamediation_adapters)
            {
                mediations.Add(standaloneAdapter.metamediation_adapter_for);
            }

            foreach (var network in iosManifest.networks)
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