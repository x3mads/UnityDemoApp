using System;
using System.Runtime.InteropServices;
using AOT;

namespace XMediator.iOS
{
    internal class iOSInterstitialAdsProxy : InterstitialAdsProxy
    {        
        private delegate void Callback(string placementId, string eventName, string resultJsonString);
    
        //TODO: VER Como mejorar que esto es estatico
        private static iOSNativeFullScreenAdsListener _iOSNativeFullScreenAdsListener;

        public void SetListener(InterstitialAdsProxyListener listener)
        {
            _iOSNativeFullScreenAdsListener = new iOSNativeFullScreenAdsListener(listener, listener);
            X3MInterstitialAdsSetCallback(ExecuteCallback);
        }

        public void Load(string placementId) => X3MLoadInterstitial(placementId);

        public bool IsReady(string placementId) => X3MIsReadyInterstitialWithPlacementId(placementId);

        public void Show(string placementId) => X3MPresentInterstitialWithPlacementId(placementId);
        
        public void ShowFromAdSpace(string adSpace) => X3MPresentInterstitialWithAdSpace(adSpace);
        
        public void ShowFromAdSpace(string placementId, string adSpace) => X3MPresentInterstitialWithPlacementIdAndAdSpace(placementId, adSpace);

        public bool IsReady() => X3MIsReadyInterstitial();

        public void Show() => X3MPresentInterstitial();
        
        [DllImport("__Internal")]
        private static extern void X3MLoadInterstitial(string placementId);
        
        [DllImport("__Internal")]
        private static extern bool X3MIsReadyInterstitial();        
        
        [DllImport("__Internal")]
        private static extern bool X3MIsReadyInterstitialWithPlacementId(string placementId);       
        
        [DllImport("__Internal")]
        private static extern void X3MPresentInterstitial();
        
        [DllImport("__Internal")]
        private static extern void X3MPresentInterstitialWithAdSpace(string adSpace);
        
        [DllImport("__Internal")]
        private static extern void X3MPresentInterstitialWithPlacementId(string placementId);

        [DllImport("__Internal")]
        private static extern void X3MPresentInterstitialWithPlacementIdAndAdSpace(string placementId, string adSpace);
        
        [DllImport("__Internal")]
        private static extern void X3MInterstitialAdsSetCallback(Callback callback);

        [MonoPInvokeCallback(typeof(Callback))]
        private static void ExecuteCallback(string placementId, string eventName, string resultJsonString)
        {
            Enum.TryParse(eventName, out IOSNativeEvent nativeAdEvent);
            _iOSNativeFullScreenAdsListener.OnIOSNativeEvent(nativeAdEvent, placementId, resultJsonString);
        }
    }
}