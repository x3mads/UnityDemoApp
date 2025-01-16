using System.Collections.Generic;

namespace XMediator.Editor.Tools.MetaMediation.Entities
{
    internal class MetaMediationDependencies
    {
        internal List<AndroidPackage> AndroidPackages { get; }
        internal List<IosPod> IosPods { get; }

        internal MetaMediationDependencies(List<AndroidPackage> androidPackages, List<IosPod> iosPods) => (AndroidPackages, IosPods) = (androidPackages, iosPods);

        internal class AndroidPackage
        {
            public string Group { get; set; }
            public string Artifact { get; set; }
            public string Version { get; set; }
        }

        internal class IosPod
        {
            public string Name { get; set; }
            public string Version { get; set; }
        }
    }
}