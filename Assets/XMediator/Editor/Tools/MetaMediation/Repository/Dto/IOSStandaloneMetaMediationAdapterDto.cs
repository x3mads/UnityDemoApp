using System;
using System.Collections.Generic;

namespace XMediator.Editor.Tools.MetaMediation.Repository.Dto
{
    [Serializable]
    public class IOSStandaloneMetaMediationAdapterDto
    {
        public string mediator;
        public string metamediation_adapter_for;
        public List<IOSDependencyDto> dependencies;
    }
}