using System;
using UnityEngine;
using XMediator.Api;

namespace XMediator.iOS
{
    [Serializable]
    internal class ImpressionDataDto
    {
        [SerializeField] internal string placementId;
        [SerializeField] internal string networkName;
        [SerializeField] internal double ecpm;
        [SerializeField] internal double revenue;
        [SerializeField] internal LoadResultDto waterfallResult;
        [SerializeField] internal string subNetworkName;
        [SerializeField] internal string creativeId;
        [SerializeField] internal string adSpace;
        [SerializeField] internal string adNetwork;
        [SerializeField] internal string mediation;
        [SerializeField] internal string id;
        
        internal ImpressionData ToImpressionData()
        {
            return new ImpressionData((decimal) revenue, (decimal) ecpm, placementId, networkName,
                subNetworkName, creativeId, adSpace, ToLoadResult(), adNetwork, mediation, id);
        }

        private LoadResult ToLoadResult()
        {
            return waterfallResult.ToLoadResult();
        }
    }
}