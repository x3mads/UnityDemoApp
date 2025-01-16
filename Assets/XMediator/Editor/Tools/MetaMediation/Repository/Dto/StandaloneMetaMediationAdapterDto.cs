using System;
using System.Collections.Generic;

namespace XMediator.Editor.Tools.MetaMediation.Repository.Dto
{
    [Serializable]
    public class StandaloneMetaMediationAdapterDto
    {
        public string mediator;
        public string metamediation_adapter_for;
        public List<DependencyDto> dependencies;
    }
}