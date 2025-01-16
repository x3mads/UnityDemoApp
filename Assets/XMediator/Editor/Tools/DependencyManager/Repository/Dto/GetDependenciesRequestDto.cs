using System;

namespace XMediator.Editor.Tools.DependencyManager.Repository.Dto
{
    [Serializable]
    internal class GetDependenciesRequestDto
    {
        public ClientDependencyDto[] client_dependencies;

        public GetDependenciesRequestDto(ClientDependencyDto[] clientDependencies)
        {
            this.client_dependencies = clientDependencies;
        }
    }
}