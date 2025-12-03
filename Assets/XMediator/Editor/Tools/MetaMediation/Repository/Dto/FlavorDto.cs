using System;
using System.Collections.Generic;
using UnityEngine;

namespace XMediator.Editor.Tools.MetaMediation.Repository.Dto
{
    [Serializable]
    internal class FlavorDto<TDependencyDto>
    {
        [SerializeField] internal string name;
        [SerializeField] internal List<string> mediators;
        [SerializeField] internal List<string> tags; // nullable
        [SerializeField] internal List<string> additional_tools; // nullable
        [SerializeField] internal ManifestDto<TDependencyDto> versions;
    }
}