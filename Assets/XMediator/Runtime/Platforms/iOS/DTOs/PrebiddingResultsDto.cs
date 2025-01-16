using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XMediator.Api;

namespace XMediator.iOS
{
    [Serializable]
    internal class PrebiddingResultsDto
    {
        [SerializeField] internal List<PrebidResultDto> results;

        internal PrebiddingResults ToPrebiddingResults()
        {
            var prebidResults = results?.Select(dto => dto.ToPrebidResult()).ToList();
            return new PrebiddingResults(prebidResults);
        }
    }

    [Serializable]
    internal class PrebidResultDto
    {
        [SerializeField] internal string bidderName;
        [SerializeField] internal string bidId;
        [SerializeField] internal double latency;
        [SerializeField] internal double ecpm; // nullable
        [SerializeField] internal InstanceErrorDto error; // nullable

        internal PrebidResult ToPrebidResult()
        {
            var timeSpan = TimeSpan.FromSeconds(latency);
            if (error.errorCode == 0)
            {
                return new PrebidResult.Success(bidderName, bidId, timeSpan, (decimal)ecpm);
            }

            return new PrebidResult.Failure(bidderName, bidId, timeSpan, error.ToInstanceError());
        }
    }
}