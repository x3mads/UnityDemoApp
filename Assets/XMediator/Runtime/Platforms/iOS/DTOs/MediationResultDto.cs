using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XMediator.Api;

namespace XMediator.iOS
{
    [Serializable]
    internal class MediationResultDto
    {
        [SerializeField] internal List<NetworkInitResultDto> networks;
        
        internal MediationResult ToMediationResult()
        {
            var networks = this.networks?.Select(dto => dto.ToNetworkResult()).ToList();
            return new MediationResult(networks);
        }
    }
}