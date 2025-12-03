using System.Collections.Generic;
using System.Linq;
using XMediator.Editor.Tools.MetaMediation.Repository.Dto;

namespace XMediator.Editor.Tools.MetaMediation.Entities
{
    internal class ToolInfo
    {
        public string Tool { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public bool PreSelected { get; }

        public ToolInfo(string tool, string displayName, string description, bool preSelected)
        {
            Tool = tool;
            DisplayName = displayName;
            Description = description;
            PreSelected = preSelected;
        }
    }

    internal class SelectableDependencies
    {
        public const string X3MMediationName = "X3M";
        public const string UMPName = "UMP";
        public HashSet<string> Networks { get; }
        public HashSet<string> Mediations { get; }
        public Dictionary<string, ToolInfo> Tools { get; }
        public IOSManifestDto IOSManifest { get; }
        public AndroidManifestDto AndroidManifest { get; }

        public SelectableDependencies(HashSet<string> networks, HashSet<string> mediations, Dictionary<string, ToolInfo> tools, IOSManifestDto iosManifest,
            AndroidManifestDto androidManifest)
        {
            Networks = networks;
            Mediations = mediations;
            Tools = tools;
            IOSManifest = iosManifest;
            AndroidManifest = androidManifest;
        }
    }
}