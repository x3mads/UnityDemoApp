using System;
using System.Collections.Generic;

namespace XMediator.Editor.Tools.MetaMediation.Repository.Dto
{
    [Serializable]
    public class AndroidManifestDto
    {
        public List<DependencyDto> core;
        public List<StandaloneMetaMediationAdapterDto> standalone_metamediation_adapters;
        public List<NetworkDto> networks;
    }
}