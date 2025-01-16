using JetBrains.Annotations;
using UnityEngine;

namespace XMediator.iOS
{
    internal class iOSNativeEventCallback
    {
        private readonly LoadProxyListener _loadListener;
        private readonly ShowProxyListener _showListener;
        [CanBeNull] private readonly RewardProxyListener _rewardListener;

        internal iOSNativeEventCallback(LoadProxyListener loadListener, ShowProxyListener showListener, [CanBeNull] RewardProxyListener rewardListener = null)
        {
            _loadListener = loadListener;
            _showListener = showListener;
            _rewardListener = rewardListener;
        }

        internal void OnIOSNativeEvent(IOSNativeEvent nativeEvent, string resultJsonString)
        {
            // Debug.Log($"Ad event {nativeEvent} with result {resultJsonString}");
            switch (nativeEvent)
            {
                case IOSNativeEvent.DidCompletePrebidding:
                    var prebiddingResults = JsonUtility.FromJson<PrebiddingResultsDto>(resultJsonString).ToPrebiddingResults();
                    _loadListener.OnPrebiddingFinished(prebiddingResults);
                    break;
                case IOSNativeEvent.DidLoad:
                    var loadResult = JsonUtility.FromJson<LoadResultDto>(resultJsonString).ToLoadResult();
                    _loadListener.OnLoaded(loadResult);
                    break;
                case IOSNativeEvent.FailedToLoad:
                    var loadError = JsonUtility.FromJson<FailedToLoadDto>(resultJsonString).ToLoadError();
                    var loadFailingResult = JsonUtility.FromJson<FailedToLoadDto>(resultJsonString).ToLoadResult();
                    _loadListener.OnFailedToLoad(loadError, loadFailingResult);
                    break;
                case IOSNativeEvent.DidPresent:
                    _showListener.OnShowed();
                    break;
                case IOSNativeEvent.FailedToPresent:
                    var showError = JsonUtility.FromJson<PresentErrorDto>(resultJsonString).ToShowError();
                    _showListener.OnFailedToShow(showError);
                    break;
                case IOSNativeEvent.DidRecordImpression:
                    var impressionData = JsonUtility.FromJson<ImpressionDataDto>(resultJsonString).ToImpressionData();
                    _showListener.OnImpression(impressionData);
                    break;
                case IOSNativeEvent.WillDismiss:
                    // WillDismiss has no Unity callback equivalent. We only map DidDismiss to OnDismissed. 
                    break;
                case IOSNativeEvent.DidDismiss:
                    _showListener.OnDismissed();
                    break;
                case IOSNativeEvent.DidClick:
                    _showListener.OnClicked();
                    break;
                case IOSNativeEvent.DidEarnReward:
                    _rewardListener?.OnEarnedReward();
                    break;
                default:
                    Debug.Log("Unhandled IOSNativeEvent, should not have happened!");
                    break;
            }
        }
    }
}