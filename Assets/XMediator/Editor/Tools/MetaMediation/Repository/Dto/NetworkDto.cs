using System;
using System.Collections.Generic;
using UnityEngine;

namespace XMediator.Editor.Tools.MetaMediation.Repository.Dto
{
    [Serializable]
    internal class NetworkDto<TDependencyDto>
    {
        [SerializeField] internal string network;
        [SerializeField] internal string display_name;
        [SerializeField] internal List<TDependencyDto> dependencies;
        [SerializeField] internal List<AdapterDto<TDependencyDto>> adapters;
    }
}