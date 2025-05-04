using System.Collections.Generic;
using System.Linq;
using XMediator.Editor.Tools.MetaMediation.Repository.Dto;

namespace XMediator.Editor.Tools.MetaMediation.Entities
{
    internal class SelectableDependencies
    {
        public const string X3MMediationName = "X3M";
        public HashSet<string> Networks { get; }
        public HashSet<string> Mediations { get; }
        public IOSManifestDto IOSManifest { get; }
        public AndroidManifestDto AndroidManifest { get; }

        public SelectableDependencies(HashSet<string> networks, HashSet<string> mediations, IOSManifestDto iosManifest,
            AndroidManifestDto androidManifest)
        {
            Networks = networks;
            Mediations = mediations;
            IOSManifest = iosManifest;
            AndroidManifest = androidManifest;
        }
    }
}