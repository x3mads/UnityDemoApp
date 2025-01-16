using System;
using UnityEngine;
using XMediator.Api;

namespace XMediator.iOS
{
    [Serializable]
    internal class InstanceResultDto
    {
        [SerializeField] SuccessResultDto successResult;
        [SerializeField] FailureResultDto failureResult;
        [SerializeField] UnusedResultDto unusedResult;

        internal InstanceResult ToInstanceResult()
        {
            if (successResult?.information != null) // TODO improve this "null" check
            {
                return successResult?.ToSuccessResult();
            }
            if (failureResult?.information != null)
            {
                return failureResult?.ToFailureResult();
            }

            return unusedResult?.ToUnusedResult();
        }
    }
}