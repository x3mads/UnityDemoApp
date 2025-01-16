using System;
using UnityEngine;
using XMediator.Api;

namespace XMediator.iOS
{
    [Serializable]
    internal class InitResultDto
    {
        [SerializeField] internal bool isSuccess;
        [SerializeField] internal string sessionId;
        [SerializeField] internal int errorCode;
        [SerializeField] internal string errorDescription;

        internal InitResult ToInitResult()
        {
            if (isSuccess)
            {
                return new InitResult.Success(sessionId);
            }
            else
            {
                return new InitResult.Failure(errorCode, errorDescription);
            }
        }
    }
}