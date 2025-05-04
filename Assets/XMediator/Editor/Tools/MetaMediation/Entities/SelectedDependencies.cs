using System.Collections.Generic;
using System.Linq;
using XMediator.Editor.Tools.MetaMediation.Repository.Dto;

namespace XMediator.Editor.Tools.MetaMediation.Entities
{
    internal class SelectedDependencies
    {
        public HashSet<string> Networks { get; }
        public HashSet<string> Mediations { get; }
        public ManifestDto<IOSDependencyDto> IOSManifest { get; }
        public ManifestDto<AndroidDependencyDto> AndroidManifest { get; }

        public SelectedDependencies(HashSet<string> networks, HashSet<string> mediations, ManifestDto<IOSDependencyDto> iosManifest,
            ManifestDto<AndroidDependencyDto> androidManifest)
        {
            Networks = networks;
            Mediations = mediations;
            IOSManifest = iosManifest;
            AndroidManifest = androidManifest;
        }

        public HashSet<string> GetNetworkIds()
        {
            return GetNetworkNames(IOSManifest).Union(GetNetworkNames(AndroidManifest)).ToHashSet();
        }

        private IEnumerable<string> GetNetworkNames<TDependency>(ManifestDto<TDependency> manifest)
        {
            return manifest?.networks
                       .Where(network => Networks.Contains(network.display_name))
                       .Select(e => e.network)
                   ?? Enumerable.Empty<string>();
        }
    }
}