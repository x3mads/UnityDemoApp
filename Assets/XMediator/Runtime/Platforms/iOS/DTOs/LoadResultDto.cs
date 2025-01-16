using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XMediator.Api;

namespace XMediator.iOS
{
    [Serializable]
    internal class LoadResultDto
    {
        [SerializeField] internal string waterfallId;
        [SerializeField] internal string lifecycleId;
        [SerializeField] internal List<InstanceResultDto> instanceResults;

        internal LoadResult ToLoadResult()
        {
            var instanceResultsList = this.instanceResults?.Select(dto => dto.ToInstanceResult()).ToList();
            return new LoadResult(waterfallId ?? "", lifecycleId ?? "", instanceResultsList ?? new List<InstanceResult>());
        }
    }
}