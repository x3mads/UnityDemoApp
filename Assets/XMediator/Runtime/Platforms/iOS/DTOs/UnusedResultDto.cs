using System;
using UnityEngine;
using XMediator.Api;

namespace XMediator.iOS
{
    [Serializable]
    internal class UnusedResultDto
    {
        [SerializeField] internal InstanceInformationDto information;

        internal InstanceResult ToUnusedResult()
        {
            return new InstanceResult.Unused(information.ToInstanceInformation());
        }
    }
}