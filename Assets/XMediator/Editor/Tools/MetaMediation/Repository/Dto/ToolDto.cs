using System;
using System.Collections.Generic;
using UnityEngine;

namespace XMediator.Editor.Tools.MetaMediation.Repository.Dto
{
    [Serializable]
    internal class ToolDto<TDependencyDto>
    {
        [SerializeField] internal string tool;
        [SerializeField] internal string display_name;
        [SerializeField] internal string description;
        [SerializeField] internal List<TDependencyDto> dependencies;
    }
}
