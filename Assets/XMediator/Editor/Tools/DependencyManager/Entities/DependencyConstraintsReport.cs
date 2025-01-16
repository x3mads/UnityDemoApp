using System;
using System.Collections.Generic;
using System.Linq;

namespace XMediator.Editor.Tools.DependencyManager.Entities
{
    internal class DependencyConstraintsReport
    {
        private readonly InstalledVersion _source;
        private readonly IEnumerable<InstalledVersion> _targets;
        private HashSet<Tuple<string, string>> _dependenciesToUpdate;

        public DependencyConstraintsReport(InstalledVersion source, IEnumerable<InstalledVersion> targets)
        {
            _source = source;
            _targets = targets;
        }

        internal HashSet<Tuple<string, string>> Resolve()
        {
            _dependenciesToUpdate = new HashSet<Tuple<string, string>>();
            
            foreach (var constraint in _source.androidDependencies)
            {
                var versionToCheck = FindDependency(constraint)?.InstalledAndroidVersion;
                CheckConstraint(constraint, versionToCheck);
            }
            
            foreach (var constraint in _source.iosDependencies)
            {
                var versionToCheck = FindDependency(constraint)?.InstalledIOSVersion;
                CheckConstraint(constraint, versionToCheck);
            }

            return _dependenciesToUpdate;
        }

        private InstalledVersion FindDependency(DependencyConstraint constraint)
        {
            return _targets.First(version => version.Name == constraint.Name);
        }

        private void CheckConstraint(DependencyConstraint constraint, string versionToCheck)
        {
            if (!ConstraintFulfilsMinRequirement(constraint, versionToCheck))
                AddDependency(constraint.Name, _source.Name);
            else if (!ConstraintFulfilsMaxRequirement(constraint, versionToCheck))
                AddDependency(_source.Name,constraint.Name);
        }

        private void AddDependency(string needsUpdate, string dependsOn)
        {
            _dependenciesToUpdate.Add(new Tuple<string, string>(needsUpdate, dependsOn));
        }

        private static bool ConstraintFulfilsMinRequirement(DependencyConstraint constraint, string versionToCheck)
        {
            return versionToCheck == null || constraint.MinVersionIsSatisfied(new Version(versionToCheck));
        }
        
        private static bool ConstraintFulfilsMaxRequirement(DependencyConstraint constraint, string versionToCheck)
        {
            return versionToCheck == null || constraint.MaxVersionIsSatisfied(new Version(versionToCheck));
        }
    }
}