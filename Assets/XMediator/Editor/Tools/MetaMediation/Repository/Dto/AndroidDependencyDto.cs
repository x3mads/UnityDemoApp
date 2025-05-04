using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace XMediator.Editor.Tools.MetaMediation.Repository.Dto
{
    [Serializable]
    internal class AndroidDependencyDto
    {
        [SerializeField] internal string group_id;
        [SerializeField] internal string artifact_name;
        [SerializeField] internal List<string> repositories;
        [SerializeField] internal string suggested_version;
        [SerializeField] [CanBeNull] internal List<Excludes> excludes;
    }

    [Serializable]
    internal class Excludes
    {
        [SerializeField] internal string group;
        [SerializeField] internal string module;
    }
}