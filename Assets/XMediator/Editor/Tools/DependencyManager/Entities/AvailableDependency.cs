using System.Collections.Generic;
using System.Linq;

namespace XMediator.Editor.Tools.DependencyManager.Entities
{
    internal class AvailableDependency
    {
        public string Name { get; }
        public string Description { get; }
        public IEnumerable<AvailableVersion> Versions { get; }
        public AvailableVersion SelectedVersion { get; }

        public AvailableDependency(string name, string description, IEnumerable<AvailableVersion> versions)
        {
            var availableVersions = versions.ToList();
            Name = name;
            Description = description;
            Versions = availableVersions;
            SelectedVersion = availableVersions.First();
        }

        public AvailableDependency(Dependency dependency)
        {
            Name = dependency.Name;
            Description = dependency.Description;
            Versions = dependency.AvailableVersions;
            SelectedVersion = new AvailableVersion(dependency);
        }
    }
}