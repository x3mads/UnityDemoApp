using System;
using System.Collections.Generic;
using UnityEngine;

namespace XMediator.Editor.Tools.MetaMediation.Repository.Dto
{
    [Serializable]
    internal class ManifestDto<TDependencyDto>
    {
        [SerializeField] internal List<TDependencyDto> core;
        [SerializeField] internal List<AdapterDto<TDependencyDto>> standalone_metamediation_adapters;
        [SerializeField] internal List<NetworkDto<TDependencyDto>> networks;
        [SerializeField] internal List<ToolDto<TDependencyDto>> additional_tools;
    }
}