using System;
using UnityEngine;
using XMediator.Api;

namespace XMediator.iOS
{
    [Serializable]
    internal class SuccessResultDto
    {
        [SerializeField] internal double latency;
        [SerializeField] internal InstanceInformationDto information;
        [SerializeField] internal string subNetworkName;
        [SerializeField] internal string creativeId;
        [SerializeField] internal double ecpm;
        [SerializeField] internal string adNetwork;
        [SerializeField] internal string mediation;

        internal InstanceResult ToSuccessResult()
        {
            return new InstanceResult.Success(information.ToInstanceInformation(), TimeSpan.FromSeconds(latency),
                creativeId, subNetworkName, (decimal) ecpm, adNetwork, mediation);
        }
    }
}