using System;
using UnityEngine;
using XMediator.Api;

namespace XMediator.iOS
{
    [Serializable]
    internal class InstanceInformationDto
    {
        [SerializeField] internal string id;
        [SerializeField] internal string name;
        [SerializeField] internal string classname;
        [SerializeField] internal double ecpm;

        internal InstanceInformation ToInstanceInformation()
        {
            return new InstanceInformation(id, name, classname, (decimal)ecpm);
        }
    }
}