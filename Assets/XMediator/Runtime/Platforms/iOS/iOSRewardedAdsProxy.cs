using System;
using System.Runtime.InteropServices;
using AOT;

namespace XMediator.iOS
{
    internal class iOSRewardedAdsProxy : RewardedAdsProxy
    { 
        private delegate void Callback(string placementId, string eventName, string resultJsonString);
    
        //TODO: VER Como mejorar que esto es estatico
        private static iOSNativeFullScreenAdsListener _iOSNativeFullScreenAdsListener;

        public void SetListener(RewardedAdsProxyListener listener)
        {
            _iOSNativeFullScreenAdsListener = new iOSNativeFullScreenAdsListener(listener, listener, listener);
            X3MRewardedAdsSetCallback(ExecuteCallback);
        }

        public void Load(string placementId) => X3MLoadRewarded(placementId);

        public bool IsReady(string placementId) => X3MIsReadyRewardedWithPlacementId(placementId);

        public bool IsAdSpaceCapped(string adSpace) => X3MIsRewardedAdSpaceCapped(adSpace);

        public void Show(string placementId)=> X3MPresentRewardedWithPlacementId(placementId);
        
        public void ShowFromAdSpace(string adSpace) => X3MPresentRewardedWithAdSpace(adSpace);

        public void ShowFromAdSpace(string placementId, string adSpace) => X3MPresentRewardedWithPlacementIdAndAdSpace(placementId, adSpace);

        public bool IsReady() => X3MIsReadyRewarded();

        public void Show() => X3MPresentRewarded();
        
        [DllImport("__Internal")]
        private static extern void X3MLoadRewarded(string placementId);

        [DllImport("__Internal")]
        private static extern bool X3MIsReadyRewarded();        
        
        [DllImport("__Internal")]
        private static extern bool X3MIsReadyRewardedWithPlacementId(string placementId);
        
        [DllImport("__Internal")]
        private static extern bool X3MIsRewardedAdSpaceCapped(string placementId);  
        
        [DllImport("__Internal")]
        private static extern void X3MPresentRewarded();

        [DllImport("__Internal")]
        private static extern void X3MPresentRewardedWithAdSpace(string adSpace);
        
        [DllImport("__Internal")]
        private static extern void X3MPresentRewardedWithPlacementId(string placementId);
        
        [DllImport("__Internal")]
        private static extern void X3MPresentRewardedWithPlacementIdAndAdSpace(string placementId, string adSpace);

        [DllImport("__Internal")]
        private static extern void X3MRewardedAdsSetCallback(Callback callback);

        [MonoPInvokeCallback(typeof(Callback))]
        private static void ExecuteCallback(string placementId, string eventName, string resultJsonString)
        {
            Enum.TryParse(eventName, out IOSNativeEvent nativeAdEvent);
            _iOSNativeFullScreenAdsListener.OnIOSNativeEvent(nativeAdEvent, placementId, resultJsonString);
        }
    }
}