using System;
using System.Collections.Generic;
using XMediator.Editor.Tools.DependencyManager.Entities;
using XMediator.Editor.Tools.DependencyManager.Repository.Dto;

namespace XMediator.Editor.Tools.DependencyManager.Repository
{
    internal interface DependenciesRepository
    {
        void FindDependencies(string publisher, IEnumerable<ClientDependencyDto> clientDependencies, Action<AvailableDependencies> onSuccess, Action<Exception> onError);
    }
}