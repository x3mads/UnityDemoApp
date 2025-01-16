using JetBrains.Annotations;
using UnityEngine;

namespace XMediator.iOS
{
    internal class iOSNativeFullScreenAdsListener
    {
        private readonly FullscreenLoadProxyListener _loadListener;
        private readonly FullscreenShowProxyListener _showListener;
        [CanBeNull] private readonly FullscreenRewardProxyListener _rewardListener;

        internal iOSNativeFullScreenAdsListener(FullscreenLoadProxyListener loadListener,
            FullscreenShowProxyListener showListener, [CanBeNull] FullscreenRewardProxyListener rewardListener = null)
        {
            _loadListener = loadListener;
            _showListener = showListener;
            _rewardListener = rewardListener;
        }

        internal void OnIOSNativeEvent(IOSNativeEvent nativeEvent, string placementId, string resultJsonString)
        {
            switch (nativeEvent)
            {
                case IOSNativeEvent.DidCompletePrebidding:

                    break;
                case IOSNativeEvent.DidLoad:
                    var loadResult = JsonUtility.FromJson<LoadResultDto>(resultJsonString).ToLoadResult();
                    _loadListener.OnLoaded(placementId, loadResult);
                    break;
                case IOSNativeEvent.FailedToLoad:

                    break;
                case IOSNativeEvent.DidPresent:
                    _showListener.OnShowed(placementId);
                    break;
                case IOSNativeEvent.FailedToPresent:
                    var showError = JsonUtility.FromJson<PresentErrorDto>(resultJsonString).ToShowError();
                    _showListener.OnFailedToShow(placementId, showError);
                    break;
                case IOSNativeEvent.DidRecordImpression:
                    var impressionData = JsonUtility.FromJson<ImpressionDataDto>(resultJsonString).ToImpressionData();
                    _showListener.OnImpression(placementId, impressionData);
                    break;
                case IOSNativeEvent.WillDismiss:
                    // WillDismiss has no Unity callback equivalent. We only map DidDismiss to OnDismissed. 
                    break;
                case IOSNativeEvent.DidDismiss:
                    _showListener.OnDismissed(placementId);
                    break;
                case IOSNativeEvent.DidClick:
                    _showListener.OnClicked(placementId);
                    break;
                case IOSNativeEvent.DidEarnReward:
                    _rewardListener?.OnEarnedReward(placementId);
                    break;
                default:
                    Debug.Log("Unhandled IOSNativeEvent, should not have happened!");
                    break;
            }
        }
    }
}