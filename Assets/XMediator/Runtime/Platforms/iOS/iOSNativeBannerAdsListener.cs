using UnityEngine;

namespace XMediator.iOS
{
    internal class iOSNativeBannerAdsListener
    {
        private readonly BannerAdsProxyListener _loadListener;

        internal iOSNativeBannerAdsListener(BannerAdsProxyListener loadListener)
        {
            _loadListener = loadListener;
        }

        internal void OnIOSNativeEvent(IOSNativeEvent nativeEvent, string placementId, string resultJsonString)
        {
            switch (nativeEvent)
            {
                case IOSNativeEvent.DidLoad:
                    var loadResult = JsonUtility.FromJson<LoadResultDto>(resultJsonString).ToLoadResult();
                    _loadListener.OnLoaded(placementId, loadResult);
                    break;
                case IOSNativeEvent.DidRecordImpression:
                    var impressionData = JsonUtility.FromJson<ImpressionDataDto>(resultJsonString).ToImpressionData();
                    _loadListener.OnImpression(placementId, impressionData);
                    break;
                case IOSNativeEvent.DidClick:
                    _loadListener.OnClicked(placementId);
                    break;
                default:
                    Debug.Log("Unhandled IOSNativeEvent, should not have happened!");
                    break;
            }
        }
    }
}