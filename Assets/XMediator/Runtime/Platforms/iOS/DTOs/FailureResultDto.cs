using System;
using UnityEngine;
using XMediator.Api;

namespace XMediator.iOS
{
    [Serializable]
    internal class FailureResultDto
    {
        [SerializeField] internal double latency;
        [SerializeField] internal InstanceErrorDto error;
        [SerializeField] internal InstanceInformationDto information;

        internal InstanceResult ToFailureResult()
        {
            return new InstanceResult.Failure(information.ToInstanceInformation(), TimeSpan.FromSeconds(latency), error.ToInstanceError());
        }
    }
}