using System.Collections.Generic;
using System.Linq;

namespace XMediator.Editor.Tools.DependencyManager.Entities
{
    internal class DependenciesReport
    {
        private readonly IEnumerable<InstalledVersion> _versions;
        public DependenciesReport(IEnumerable<InstalledVersion> versions)
        {
            _versions = versions;
        }

        internal Dictionary<string, HashSet<string>> Resolve()
        {
            return _versions
                .SelectMany(version => new DependencyConstraintsReport(version, _versions).Resolve())
                .GroupBy(tuple => tuple.Item1, tuple => tuple.Item2)
                .ToDictionary(g => g.Key, g => new HashSet<string>(g.ToList()));
        }
    }
}