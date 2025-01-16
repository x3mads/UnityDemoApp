using System;
using System.Collections.Generic;
using XMediator.Api;

namespace XMediator.Unity
{
    public class MockMotherObject
    {
        public static LoadResult SuccessfulLoadResult =>
            new LoadResult(
                waterfallId: "waterfall-id",
                lifecycleId: "lifecycle-id",
                instances: new InstanceResult[]
                {
                    new InstanceResult.Failure(
                        information: new InstanceInformation(
                            id: "instance-id",
                            name: "network-1",
                            classname: "network-classname",
                            ecpm: 10M
                        ),
                        error: new InstanceError(
                            type: InstanceError.ErrorType.LoadFailed,
                            adapterCode: 3,
                            name: "NO_FILL",
                            message: "Ad request failed."),
                        latency: TimeSpan.FromMilliseconds(300)),
                    SuccessInstance,
                    new InstanceResult.Unused(
                        information: new InstanceInformation(
                            id: "instance-id",
                            name: "network-1",
                            classname: "network-classname",
                            ecpm: 1M
                        ))
                });

        public static LoadResult EmptyLoadResult =>
            new LoadResult(
                "waterfall-id",
                "lifecycle-id",
                new List<InstanceResult>()
            );

        private static InstanceResult.Success SuccessInstance
        {
            get
            {
                var information = new InstanceInformation(
                    id: "instance-id",
                    name: "network-1",
                    classname: "network-classname",
                    ecpm: 5M
                );
                return new InstanceResult.Success(
                    information: information,
                    latency: TimeSpan.FromMilliseconds(300), creativeId: "creative-id",
                    subNetworkName: "subnetwork-name",
                    ecpm: 6M,
                    adNetwork: "Network 1",
                    mediation: "X3M"
                );
            }
        }

        public static ImpressionData ImpressionData(string placementId, string adSpace = null)
        {
            var success = SuccessInstance;
            return new ImpressionData(
                success.Information.Ecpm / 1000,
                success.Information.Ecpm,
                placementId,
                success.NetworkName,
                success.SubNetworkName,
                success.CreativeId,
                adSpace: adSpace,
                SuccessfulLoadResult,
                adNetwork: success.AdNetwork,
                mediation: success.Mediation);
        }

        public static PrebiddingResults PrebiddingResults =>
            new PrebiddingResults(new PrebidResult[]
            {
                new PrebidResult.Failure(
                    bidderName: "bidder-1",
                    bidId: "bid-id",
                    latency: TimeSpan.FromMilliseconds(50),
                    instanceError: new InstanceError(
                        type: InstanceError.ErrorType.LoadFailed,
                        adapterCode: 3,
                        name: "NO_FILL",
                        message: "Ad request failed.")),
                new PrebidResult.Success(
                    bidderName: "bidder-2",
                    bidId: "bid-id",
                    latency: TimeSpan.FromMilliseconds(50),
                    bidEcpm: 0.5M)
            });

        public static LoadError AlreadyUsedLoadError =>
            new LoadError(LoadError.ErrorType.AlreadyUsed, "Ad already used");

        public static ShowError NotRequestedShowError =>
            new ShowError(ShowError.ErrorType.NotRequested, "Ad not requested");

        public static ShowError LoadingShowError => new ShowError(ShowError.ErrorType.Loading, "Ad is being loaded");

        public static ShowError AlreadyUsedShowError =>
            new ShowError(ShowError.ErrorType.AlreadyUsed, "Ad already used");
    }
}