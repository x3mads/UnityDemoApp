using System.Collections.Generic;
using XMediator.Editor.Tools.DependencyManager.Entities;

namespace XMediator.Editor.Tools.DependencyManager.Repository
{
    internal interface InstalledDependenciesRepository
    {
        InstalledVersion GetXMediatorDependency();
        IEnumerable<InstalledVersion> GetAdaptersDependencies();
        bool RemoveDependency(string dependency);
    }
}