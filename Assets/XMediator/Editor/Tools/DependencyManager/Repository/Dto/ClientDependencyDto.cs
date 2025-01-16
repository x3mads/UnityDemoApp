using System;

namespace XMediator.Editor.Tools.DependencyManager.Repository.Dto
{
    [Serializable]
    internal class ClientDependencyDto
    {
        public string dependency;
        public string version;
        
        public ClientDependencyDto(string dependency, string version)
        {
            this.dependency = dependency;
            this.version = version;
        }

    }
}