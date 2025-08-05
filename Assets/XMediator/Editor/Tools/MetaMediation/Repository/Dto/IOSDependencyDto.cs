using System;
using System.Collections.Generic;
using UnityEngine;

namespace XMediator.Editor.Tools.MetaMediation.Repository.Dto
{
    [Serializable]
    internal class IOSDependencyDto
    {
        [SerializeField] internal string pod;
        [SerializeField] internal List<string> repositories;
        [SerializeField] internal string suggested_version;
    }
}