using System;
using System.Collections.Generic;

namespace XMediator.Editor.Tools.MetaMediation.Repository.Dto
{
    [Serializable]
    public class NetworkDto
    {
        public string network;
        public string display_name;
        public List<DependencyDto> dependencies;
        public List<NetworkAdapterDto> adapters;
    }
}