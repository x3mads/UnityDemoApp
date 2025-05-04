using System;
using UnityEngine;

namespace XMediator.Editor.Tools.MetaMediation.Repository.Dto
{
    [Serializable]
    internal class IOSDependencyDto
    {
        [SerializeField] internal string pod;
        [SerializeField] internal string suggested_version;
    }
}