using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using XMediator.Api;

namespace XMediator.Android
{
    internal class PrebidResultDto
    {
        internal const int PREBID_RESULT_FAILURE = -1;
        internal const int PREBID_RESULT_SUCCESS = 0;

        internal int Result { get; }
        internal string BidderName { get; }
        internal string BidId { get; }
        internal long Latency { get; }
        internal float BidEcpm { get; }
        internal InstanceErrorDto Error { get; }
        internal Dictionary<string, object> Extras { get; }

        internal PrebidResultDto(
            int result,
            string bidderName,
            string bidId,
            long latency,
            float bidEcpm,
            InstanceErrorDto error,
            Dictionary<string, object> extras
        )
        {
            Result = result;
            BidderName = bidderName;
            BidId = bidId;
            Latency = latency;
            BidEcpm = bidEcpm;
            Error = error;
            Extras = extras;
        }

        internal static PrebidResultDto FromAndroidJavaObject(AndroidJavaObject androidJavaObject)
        {
            using (androidJavaObject)
            {
                return new PrebidResultDto(
                    result: androidJavaObject.Call<int>("getResult"),
                    bidderName: androidJavaObject.Call<string>("getBidderName"),
                    bidId: androidJavaObject.Call<string>("getBidId"),
                    latency: androidJavaObject.Call<long>("getLatency"),
                    bidEcpm: BidEcpmFromAndroidJavaObject(androidJavaObject.Call<AndroidJavaObject>("getBidEcpm")),
                    error: InstanceErrorDto.FromAndroidJavaObject(
                        androidJavaObject.Call<AndroidJavaObject>("getError")),
                    extras: new Dictionary<string, object>()
                );
            }
        }

        private static float BidEcpmFromAndroidJavaObject([CanBeNull] AndroidJavaObject javaObject)
        {
            if (javaObject == null) return default;
            using (javaObject)
            {
                return javaObject.Call<float>("floatValue");    
            }
        }

        internal PrebidResult ToPrebidResult()
        {
            switch (Result)
            {
                case PREBID_RESULT_SUCCESS:
                    return new PrebidResult.Success(
                        bidderName: BidderName,
                        bidId: BidId,
                        latency: LatencyAsTimeSpan(),
                        bidEcpm: (decimal)BidEcpm
                    );
                case PREBID_RESULT_FAILURE:
                    return new PrebidResult.Failure(
                        bidderName: BidderName,
                        bidId: BidId,
                        latency: LatencyAsTimeSpan(),
                        instanceError: Error.ToInstanceError()
                    );
                default:
                    throw new Exception("Invalid result type.");
            }
        }

        private TimeSpan LatencyAsTimeSpan() => TimeSpan.FromMilliseconds(Latency);
    }
}