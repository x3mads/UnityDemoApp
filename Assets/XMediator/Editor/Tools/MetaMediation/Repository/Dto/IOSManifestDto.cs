using System;
using System.Collections.Generic;

namespace XMediator.Editor.Tools.MetaMediation.Repository.Dto
{
    [Serializable]
    public class IOSManifestDto
    {
        public List<IOSDependencyDto> core;
        public List<IOSStandaloneMetaMediationAdapterDto> standalone_metamediation_adapters;
        public List<IOSNetworkDto> networks;
    }
}