using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XMediator.Editor.Tools.DependencyManager.Entities;
using XMediator.Editor.Tools.DependencyManager.Repository;

namespace XMediator.Editor.Tools.DependencyManager.Actions
{
    internal class GetInstalledDependencies
    {
        private readonly InstalledDependenciesRepository _installedDependenciesRepository;

        public GetInstalledDependencies(InstalledDependenciesRepository installedDependenciesRepository)
        {
            _installedDependenciesRepository = installedDependenciesRepository;
        }
        
        public InstalledVersions Invoke()
        {
            var xmeditorDependecy = _installedDependenciesRepository.GetXMediatorDependency();
            var adaptersDependencies = _installedDependenciesRepository.GetAdaptersDependencies();
            var installedVersions = new InstalledVersions(sdk: xmeditorDependecy, adapters: adaptersDependencies);
			return installedVersions;
        }
        
    }
}