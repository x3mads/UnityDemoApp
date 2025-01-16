using System.Collections.Generic;
using System.Linq;

namespace XMediator.Editor.Tools.DependencyManager.Entities
{
    internal class AvailableVersions
    {
        internal AvailableVersion Sdk { get; }
        internal IEnumerable<AvailableVersion> Adapters { get; }

        internal static AvailableVersions Blank = new AvailableVersions(null, new AvailableVersion[] { });

        public AvailableVersions(AvailableVersion sdk, IEnumerable<AvailableVersion> adapters)
        {
            Sdk = sdk;
            Adapters = adapters;
        }

        public AvailableVersion GetByName(string name)
        {
            return Adapters.SingleOrDefault(available => available.Name.Equals(name));
        }
    }
}