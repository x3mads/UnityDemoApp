using System;
using System.Runtime.InteropServices;
using AOT;

namespace XMediator.iOS
{
    internal class iOSAppOpenAdsProxy : AppOpenAdsProxy
    {        
        private delegate void Callback(string placementId, string eventName, string resultJsonString);
    
        //TODO: VER Como mejorar que esto es estatico
        private static iOSNativeFullScreenAdsListener _iOSNativeFullScreenAdsListener;

        public void SetListener(AppOpenAdsProxyListener listener)
        {
            _iOSNativeFullScreenAdsListener = new iOSNativeFullScreenAdsListener(listener, listener);
            X3MAppOpenAdsSetCallback(ExecuteCallback);
        }

        public void Load(string placementId) => X3MLoadAppOpen(placementId);

        public bool IsReady() => X3MIsReadyAppOpen();

        public bool IsReady(string placementId) => X3MIsReadyAppOpenWithPlacementId(placementId);

        public bool IsAdSpaceCapped(string adSpace) => X3MIsAppOpenAdSpaceCapped(adSpace);

        public void Show() => X3MPresentAppOpen();

        public void Show(string placementId) => X3MPresentAppOpenWithPlacementId(placementId);

        public void ShowFromAdSpace(string adSpace) => X3MPresentAppOpenWithAdSpace(adSpace);

        public void ShowFromAdSpace(string placementId, string adSpace) => X3MPresentAppOpenWithPlacementIdAndAdSpace(placementId, adSpace);

        [DllImport("__Internal")]
        private static extern void X3MLoadAppOpen(string placementId);
        
        [DllImport("__Internal")]
        private static extern bool X3MIsReadyAppOpen();
        
        [DllImport("__Internal")]
        private static extern bool X3MIsReadyAppOpenWithPlacementId(string placementId);
        
        [DllImport("__Internal")]
        private static extern bool X3MIsAppOpenAdSpaceCapped(string placementId);
        
        [DllImport("__Internal")]
        private static extern void X3MPresentAppOpen();
        
        [DllImport("__Internal")]
        private static extern void X3MPresentAppOpenWithPlacementId(string placementId);
        
        [DllImport("__Internal")]
        private static extern void X3MPresentAppOpenWithAdSpace(string adSpace);
        
        [DllImport("__Internal")]
        private static extern void X3MPresentAppOpenWithPlacementIdAndAdSpace(string placementId, string adSpace);
        
        [DllImport("__Internal")]
        private static extern void X3MAppOpenAdsSetCallback(Callback callback);

        [MonoPInvokeCallback(typeof(Callback))]
        private static void ExecuteCallback(string placementId, string eventName, string resultJsonString)
        {
            Enum.TryParse(eventName, out IOSNativeEvent nativeAdEvent);
            _iOSNativeFullScreenAdsListener.OnIOSNativeEvent(nativeAdEvent, placementId, resultJsonString);
        }
    }
}
