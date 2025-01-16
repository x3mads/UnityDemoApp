using System;
using UnityEngine.Serialization;

namespace XMediator.Editor.Tools.MetaMediation.Repository.Dto
{
    [Serializable]
    public class AvailableVersionDto
    {
        [FormerlySerializedAs("Version")] public string version;
    }
}