using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace XMediator.Editor.Tools.DependencyManager.Entities
{
    [DataContract]
    internal class InstalledVersions
    {
        [DataMember(Name = "sdk", Order = 0)]
        internal InstalledVersion Sdk { get; set; }
        
        [DataMember(Name = "adapters", Order = 1)]
        internal IEnumerable<InstalledVersion> Adapters { get; set; }
        internal static InstalledVersions Blank = new InstalledVersions(null, new InstalledVersion[] { });

        public InstalledVersions(InstalledVersion sdk, IEnumerable<InstalledVersion> adapters)
        {
            Sdk = sdk;
            Adapters = adapters;
        }

        public InstalledVersion GetByName(string name)
        {
            return Adapters.SingleOrDefault(installed => installed.Name.Equals(name));
        }

        public Dictionary<string, HashSet<string>> ReportBrokenConstraints()
        {
            var versions = Sdk != null ? Adapters.Append(Sdk) : Adapters;
            return new DependenciesReport(versions).Resolve();
        }
    }
}