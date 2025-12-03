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
                onSuccess: platformsDependency =>
                {
                    processOnSuccess(onSuccess, onError, platformsDependency, !_settingsRepository.RetrieveCMPToolWasShown());
                },
                onError: onError
            );
        }

        private static void processOnSuccess(Action<SelectableDependencies> onSuccess, Action<Exception> onError,
            DependencyRepository.PlatformsDependency platformsDependency, bool preselectCmpTool)
        {
            var networks = new HashSet<string>();
            var mediations = new HashSet<string>();
            var tools = new Dictionary<string, ToolInfo>();
            if (platformsDependency != null)
            {
                var iosManifest = platformsDependency.IOSManifest;
                ExtractMediationAndNetworks(iosManifest, mediations, networks);
                ExtractTools(iosManifest, tools, preselectCmpTool);
                var androidManifest = platformsDependency.AndroidManifest;
                ExtractMediationAndNetworks(androidManifest, mediations, networks);
                ExtractTools(androidManifest, tools, preselectCmpTool);

                onSuccess.Invoke(new SelectableDependencies(networks: networks, mediations: mediations, tools: tools, iosManifest: iosManifest,
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

        private static void ExtractTools<T>(FlavoredManifestDto<T> flavoredManifest,
            Dictionary<string, ToolInfo> tools, bool preselectCmpTool)
        {
            // Only extract tools from tagless flavors
            var taglessFlavors = flavoredManifest.flavors.Where(f => f.tags == null || !f.tags.Any());
            
            foreach (var flavor in taglessFlavors)
            {
                var manifest = flavor.versions;
                if (manifest.additional_tools == null) continue;
                
                foreach (var tool in manifest.additional_tools)
                {
                    if (!tools.ContainsKey(tool.tool))
                    {
                        var preselected = tool.tool.Equals(SelectableDependencies.UMPName) && preselectCmpTool;
                        tools[tool.tool] = new ToolInfo(tool.tool, tool.display_name, tool.description, preselected);
                    }
                }
            }
        }
    }
}