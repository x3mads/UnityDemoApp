using System;
using UnityEngine;
using XMediator.Api;

namespace XMediator.iOS
{
    [Serializable]
    internal class NetworkInitResultDto
    {
        [SerializeField] internal string name;
        [SerializeField] internal string description;
        [SerializeField] internal bool isSuccess;
        [SerializeField] internal InstanceErrorDto error;
        
        internal NetworkInitResult ToNetworkResult()
        {
            return isSuccess ? (NetworkInitResult)new NetworkInitResult.Success(name,description) :
                new NetworkInitResult.Failure(name,description,error.ToInstanceError());
        }
    }
}