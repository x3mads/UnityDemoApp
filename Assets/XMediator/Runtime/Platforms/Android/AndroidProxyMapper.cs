using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using XMediator.Api;

namespace XMediator.Android
{
    internal static class AndroidProxyMapper
    {
        internal static List<InstanceResult> MapToInstanceResults(AndroidJavaObject[] instancesArray) =>
            instancesArray
                .Select(InstanceResultDto.FromAndroidJavaObject)
                .Select(e => e.ToInstanceResult())
                .ToList();

        internal static PrebiddingResults MapToPrebiddingResults(AndroidJavaObject[] result) =>
            new PrebiddingResults(results:
                result
                    .Select(PrebidResultDto.FromAndroidJavaObject)
                    .Select(e => e.ToPrebidResult())
                    .ToList()
            );

        internal static ShowError MapShowError(AndroidJavaObject androidJavaObject)
        {
            return ShowErrorDto.FromAndroidJavaObject(androidJavaObject).ToShowError();
        }

        internal static LoadError MapLoadError(AndroidJavaObject loadError)
        {
            return LoadErrorDto.FromAndroidJavaObject(loadError).ToLoadError();
        }

        internal static LoadResult MapLoadResult([CanBeNull] AndroidJavaObject loadResult)
        {
            using(loadResult)
            {
                var results = loadResult != null
                    ? MapToInstanceResults(loadResult.Call<AndroidJavaObject[]>("getInstances"))
                    : new List<InstanceResult>();

                return new LoadResult(
                    waterfallId: loadResult?.Call<string>("getWaterfallId") ?? "",
                    lifecycleId: loadResult?.Call<string>("getLifecycleId") ?? "",
                    results
                );
            }
        }

        internal static ImpressionData MapImpressionData(AndroidJavaObject impressionData)
        {
            using (impressionData)
            {
                return new ImpressionData(
                    revenue: (decimal) impressionData.Call<float>("getRevenue"),
                    ecpm: (decimal) impressionData.Call<float>("getEcpm"),
                    placementId: impressionData.Call<string>("getPlacementId"),
                    networkName: impressionData.Call<string>("getNetworkName"),
                    subNetworkName: impressionData.Call<string>("getSubNetworkName"),
                    creativeId: impressionData.Call<string>("getCreativeId"),
                    adSpace: impressionData.Call<string>("getAdSpace"),
                    waterfallResult: MapLoadResult(impressionData.Call<AndroidJavaObject>("getWaterfallResult")),
                    adNetwork: impressionData.Call<string>("getAdNetwork"),
                    mediation: impressionData.Call<string>("getMediation"),
                    id: impressionData.Call<string>("getId")
                );
            }
        }

        internal static InitResult ParseOnInitCompleted(AndroidJavaObject initResult) =>
            InitResultDto.FromAndroidJavaObject(initResult).ToInitResult();

        internal static MediationResult ParseOnMediationInitCompleted(AndroidJavaObject mediationResult) =>
            MediationResultDto.FromAndroidJavaObject(mediationResult).ToMediationResult();
    }
}