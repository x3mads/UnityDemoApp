using System;
using System.Collections.Generic;

namespace XMediator.Editor.Tools.MetaMediation.Repository.Dto
{
    [Serializable]
    public class IOSNetworkDto
    {
        public string network;
        public string display_name;
        public List<IOSDependencyDto> dependencies;
        public List<IOSNetworkAdapterDto> adapters;
    }
}