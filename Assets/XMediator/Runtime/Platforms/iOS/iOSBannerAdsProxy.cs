using System;
using System.Runtime.InteropServices;
using AOT;
using XMediator.Api;

namespace XMediator.iOS
{
    internal class iOSBannerAdsProxy : BannerAdsProxy
    {
        private delegate void Callback(string placementId, string eventName, string resultJsonString);

        private static iOSNativeBannerAdsListener _iOSNativeBannerAdsListener;

        public void SetListener(BannerAdsProxyListener bannerAdsProxyListener)
        {
            _iOSNativeBannerAdsListener =
                new iOSNativeBannerAdsListener(bannerAdsProxyListener);
            X3MBannerAdsSetCallback(ExecuteCallback);
        }

        public void Create(string placementId, BannerAds.Size size, BannerAds.Position position)
        {
            if (position.IsCustom())
            {
                X3MCreateBannerWithCustomPosition(placementId, size.Identifier, position.X, position.Y);
            }
            else
            {
                X3MCreateBanner(placementId, size.Identifier, position.Identifier);
            }
        }

        public void Load(string placementId) => X3MLoadBanner(placementId);

        public bool IsReady(string placementId) => X3MIsReadyBannerWithPlacementId(placementId);

        public void SetPosition(string placementId, BannerAds.Position position)
        {
            if (position.IsCustom())
            {
                X3MSetPositionBannerCustom(placementId, position.X, position.Y);
            }
            else
            {
                X3MSetPositionBanner(placementId, position.Identifier);
            }
        }

        public void SetAdSpace(string placementId, string adSpace) => X3MSetBannerAdSpace(placementId, adSpace);

        public void Show(string placementId, BannerAds.Position position)
        {
            if (position.IsCustom())
            {
                X3MPresentBannerWithCustomPosition(placementId, position.X, position.Y);
            }
            else
            {
                X3MPresentBannerWithPosition(placementId, position.Identifier);
            }
        }

        public void Show(string placementId) => X3MPresentBanner(placementId);

        public void Hide(string placementId) => X3MHideBanner(placementId);

        [DllImport("__Internal")]
        private static extern void X3MCreateBanner(string placementId,
            int size, int position);

        [DllImport("__Internal")]
        private static extern void X3MCreateBannerWithCustomPosition(string placementId,
            int size, int x, int y);

        [DllImport("__Internal")]
        private static extern void X3MLoadBanner(string placementId);

        [DllImport("__Internal")]
        private static extern bool X3MIsReadyBannerWithPlacementId(string placementId);

        [DllImport("__Internal")]
        private static extern bool X3MPresentBannerWithPosition(string placementId, int position);

        [DllImport("__Internal")]
        private static extern bool X3MPresentBannerWithCustomPosition(string placementId, int x, int y);

        [DllImport("__Internal")]
        private static extern bool X3MPresentBanner(string placementId);

        [DllImport("__Internal")]
        private static extern void X3MHideBanner(string placementId);
        
        [DllImport("__Internal")]
        private static extern void X3MSetBannerAdSpace(string placementId, string adSpace);
        
        [DllImport("__Internal")]
        private static extern void X3MSetPositionBannerCustom(string placementId, int x, int y);

        [DllImport("__Internal")]
        private static extern void X3MSetPositionBanner(string placementId, int position);

        [DllImport("__Internal")]
        private static extern void X3MBannerAdsSetCallback(Callback callback);

        [MonoPInvokeCallback(typeof(Callback))]
        private static void ExecuteCallback(string placementId, string eventName, string resultJsonString)
        {
            Enum.TryParse(eventName, out IOSNativeEvent nativeAdEvent);
            _iOSNativeBannerAdsListener.OnIOSNativeEvent(nativeAdEvent, placementId, resultJsonString);
        }
    }
}