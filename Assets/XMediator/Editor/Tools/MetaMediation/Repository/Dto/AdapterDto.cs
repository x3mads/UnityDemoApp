using System;
using System.Collections.Generic;
using UnityEngine;

namespace XMediator.Editor.Tools.MetaMediation.Repository.Dto
{
    [Serializable]
    internal class AdapterDto<TDependencyDto>
    {
        [SerializeField] internal string mediator;
        [SerializeField] internal string metamediation_adapter_for;
        [SerializeField] internal List<TDependencyDto> dependencies;
    }
}