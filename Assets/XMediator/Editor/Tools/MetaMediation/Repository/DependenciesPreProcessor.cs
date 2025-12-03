using System;
using System.Collections.Generic;
using System.Linq;
using XMediator.Editor.Tools.MetaMediation.Entities;
using XMediator.Editor.Tools.MetaMediation.Repository.Dto;

namespace XMediator.Editor.Tools.MetaMediation.Repository
{
    internal class DependenciesPreProcessor
    {
        public ProcessedDependencies Invoke(SelectedDependencies selectableDependencies)
        {
            var networks = selectableDependencies.Networks.ToList();
            var mediators = selectableDependencies.Mediations.ToList();
            var tools = selectableDependencies.Tools.ToList();
            
            var androidRepositories = new HashSet<string>();
            var artifacts = new HashSet<string>();
            ProcessManifest(selectableDependencies.AndroidManifest, networks, mediators, tools, dependencyList =>
            {
                AddReposAndArtifacts(dependencyList, artifacts, androidRepositories);
            });
            
            var iOSSources = new HashSet<string>();
            var pods = new HashSet<Pod>();
            ProcessManifest(selectableDependencies.IOSManifest, networks, mediators, tools, dependencyList =>
            {
                AddSourcesAndPods(dependencyList, iOSSources, pods);
            });
            
            return new ProcessedDependencies(androidRepositories, artifacts, iOSSources, pods);
        }

        private static void ProcessManifest<TDependency>(ManifestDto<TDependency> manifest, ICollection<string> networks,
            List<string> mediators, List<string> tools, Action<List<TDependency>> addDependencies)
        {
            if (manifest == null)
            {
                return;
            }
            
            // We always add core dependencies
            addDependencies(manifest.core);

            foreach (var standaloneAdapter in manifest.standalone_metamediation_adapters.Where(
                         standaloneAdapter => mediators.Contains(standaloneAdapter.metamediation_adapter_for)))
            {
                addDependencies(standaloneAdapter.dependencies);
            }
            foreach (var mediatorAdapter in SelectMediatorsBundledWithinNetworks(manifest, mediators))
            {
                addDependencies(mediatorAdapter.dependencies);
            }
            
            foreach (var network in manifest.networks.Where(network => networks.Contains(network.display_name)))
            {
                addDependencies(network.dependencies);

                foreach (var networkAdapterDto in network.adapters.Where(adapter =>
                             mediators.Contains(adapter.mediator)))
                {
                    addDependencies(networkAdapterDto.dependencies);
                }
            }
            
            // Add additional tools dependencies
            if (manifest.additional_tools != null)
            {
                foreach (var tool in manifest.additional_tools.Where(tool => tools.Contains(tool.tool)))
                {
                    addDependencies(tool.dependencies);
                }
            }
        }

        private static IEnumerable<AdapterDto<T>> SelectMediatorsBundledWithinNetworks<T>(ManifestDto<T> manifest, ICollection<string> mediators)
        {
            return manifest.networks.
                SelectMany( n => n.adapters).
                Where(a => a.metamediation_adapter_for != null
                           && mediators.Contains(a.metamediation_adapter_for));
        }

        private static void AddSourcesAndPods(List<IOSDependencyDto> dependencies, HashSet<string> iosSources, HashSet<Pod> pods)
        {
            foreach (var dependency in dependencies)
            {
                pods.Add(new Pod(dependency.pod, dependency.suggested_version));
                foreach (var source in dependency.repositories)
                {
                    iosSources.Add(source);
                }
            }
        }

        private static void AddReposAndArtifacts(List<AndroidDependencyDto> dependencies, HashSet<string> artifacts,
            HashSet<string> androidRepositories)
        {
            foreach (var dependency in dependencies)
            {
                artifacts.Add(ResolveArtifactName(dependency));
                foreach (var dependencyRepository in dependency.repositories)
                {
                    androidRepositories.Add(dependencyRepository);
                }
            }
        }

        private static string ResolveArtifactName(AndroidDependencyDto dependencyDto)
        {
            return dependencyDto.group_id + ":" + dependencyDto.artifact_name + ":" + dependencyDto.suggested_version;
        }
    }
}