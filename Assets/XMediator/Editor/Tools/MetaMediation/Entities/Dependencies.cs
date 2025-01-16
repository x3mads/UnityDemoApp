using System.Collections.Generic;
using System.Linq;
using XMediator.Editor.Tools.MetaMediation.Repository.Dto;

namespace XMediator.Editor.Tools.MetaMediation.Entities
{
    internal class Dependencies
    {
        public HashSet<string> Networks { get; }
        public HashSet<string> Mediations { get; }
        public IOSManifestDto IOSManifest { get; }
        public AndroidManifestDto AndroidManifest { get; }

        public Dependencies(HashSet<string> networks, HashSet<string> mediations, IOSManifestDto iosManifest,
            AndroidManifestDto androidManifest)
        {
            Networks = networks;
            Mediations = mediations;
            IOSManifest = iosManifest;
            AndroidManifest = androidManifest;
        }

        public HashSet<string> GetNetworkIds()
        {
            var iosNetworkDtos = IOSManifest?.networks
                                     .Where(network => Networks.Contains(network.display_name))
                                 ?? Enumerable.Empty<IOSNetworkDto>();
            var androidNetworkDtos = AndroidManifest?.networks
                                         .Where(network => Networks.Contains(network.display_name))
                                     ?? Enumerable.Empty<NetworkDto>();

            return iosNetworkDtos.Select(e => e.network)
                .Union(androidNetworkDtos.Select(e => e.network))
                .ToHashSet();
        }
    }
}