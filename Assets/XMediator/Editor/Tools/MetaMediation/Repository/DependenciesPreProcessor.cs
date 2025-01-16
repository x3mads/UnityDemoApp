using System.Collections.Generic;
using System.Linq;
using XMediator.Editor.Tools.MetaMediation.Entities;
using XMediator.Editor.Tools.MetaMediation.Repository.Dto;

namespace XMediator.Editor.Tools.MetaMediation.Repository
{
    internal class DependenciesPreProcessor
    {
        public ProcessedDependencies Invoke(Dependencies dependencies)
        {
            var androidRepositories = new HashSet<string>();
            var artifacts = new HashSet<string>();
            var pods = new HashSet<Pod>();
            var networks = dependencies.Networks.ToList();
            var mediators = dependencies.Mediations.ToList();
            if (dependencies.AndroidManifest != null)
                ProcessAndroidManifest(dependencies.AndroidManifest, networks, mediators, androidRepositories,
                    artifacts);
            if (dependencies.IOSManifest != null)
                ProcessIOSManifest(dependencies.IOSManifest, networks, mediators, pods);
            return new ProcessedDependencies(androidRepositories, artifacts, pods);
        }

        private static void ProcessAndroidManifest(AndroidManifestDto androidManifest, ICollection<string> networks,
            ICollection<string> mediators,
            HashSet<string> androidRepositories, HashSet<string> artifacts)
        {
            if (mediators.Contains("X3M"))
            {
                AddReposAndArtifacts(androidManifest.core, artifacts, androidRepositories);
            }

            foreach (var standaloneAdapter in androidManifest.standalone_metamediation_adapters.Where(
                         standaloneAdapter => mediators.Contains(standaloneAdapter.metamediation_adapter_for)))
            {
                AddReposAndArtifacts(standaloneAdapter.dependencies, artifacts, androidRepositories);
            }

            foreach (var network in androidManifest.networks.Where(network => networks.Contains(network.display_name)))
            {
                AddReposAndArtifacts(network.dependencies, artifacts, androidRepositories);

                foreach (var networkAdapterDto in network.adapters.Where(networkAdapterDto =>
                             mediators.Contains(networkAdapterDto.mediator)))
                {
                    AddReposAndArtifacts(networkAdapterDto.dependencies, artifacts, androidRepositories);
                }
            }
        }

        private static void ProcessIOSManifest(IOSManifestDto androidManifest, List<string> networks,
            ICollection<string> mediators,
            HashSet<Pod> pods)
        {
            if (mediators.Contains("X3M"))
            {
                AddPodsAndVersions(pods, androidManifest.core);
            }

            foreach (var standaloneAdapter in androidManifest.standalone_metamediation_adapters.Where(
                         standaloneAdapter => mediators.Contains(standaloneAdapter.metamediation_adapter_for)))
            {
                AddPodsAndVersions(pods, standaloneAdapter.dependencies);
            }

            foreach (var network in androidManifest.networks.Where(network => networks.Contains(network.display_name)))
            {
                AddPodsAndVersions(pods, network.dependencies);

                foreach (var networkAdapterDto in network.adapters.Where(networkAdapterDto =>
                             mediators.Contains(networkAdapterDto.mediator)))
                {
                    AddPodsAndVersions(pods, networkAdapterDto.dependencies);
                }
            }
        }

        private static void AddPodsAndVersions(HashSet<Pod> pods,
            List<IOSDependencyDto> dependencies)
        {
            foreach (var dependency in dependencies)
            {
                pods.Add(new Pod(dependency.pod, dependency.suggested_version));
            }
        }

        private static void AddReposAndArtifacts(List<DependencyDto> dependencies, HashSet<string> artifacts,
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

        private static string ResolveArtifactName(DependencyDto dependencyDto)
        {
            return dependencyDto.group_id + ":" + dependencyDto.artifact_name + ":" + dependencyDto.suggested_version;
        }
    }
}