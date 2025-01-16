using System;

namespace XMediator.Unity
{
    internal class UnityInterstitialAdsProxy : InterstitialAdsProxy
    {
        internal static InterstitialAdsProxy ProxiedAds = new MockInterstitialAdsProxy();
            
        public void SetListener(InterstitialAdsProxyListener listener)
        {
            ProxiedAds.SetListener(listener);
        }

        public void Load(string placementId)
        {
            ProxiedAds.Load(placementId);
        }

        public bool IsReady(string placementId)
        {
            return ProxiedAds.IsReady(placementId);
        }

        public void Show(string placementId)
        {
            ProxiedAds.Show(placementId);
        }

        public void ShowFromAdSpace(string adSpace)
        {
            ProxiedAds.ShowFromAdSpace(adSpace);
        }

        public void ShowFromAdSpace(string placementId, string adSpace)
        {
            ProxiedAds.ShowFromAdSpace(placementId, adSpace);
        }

        public bool IsReady()
        {
            return ProxiedAds.IsReady();
        }

        public void Show()
        {
            ProxiedAds.Show();
        }
    }
}