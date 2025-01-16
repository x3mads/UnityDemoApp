using System;
using UnityEngine;
using XMediator.Editor.Tools.DependencyManager.Repository;

namespace XMediator.Editor.Tools.DependencyManager.Actions
{
    internal class RemoveDependency
    {
        private readonly InstalledDependenciesRepository _installedDependenciesRepository;

        public RemoveDependency(InstalledDependenciesRepository installedDependenciesRepository)
        {
            _installedDependenciesRepository = installedDependenciesRepository;
        }

        public void Invoke(string dependency, Action<bool> onResult)
        {
            try
            {
                onResult(_installedDependenciesRepository.RemoveDependency(dependency));
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                onResult(false);
            }
        }
    }
}