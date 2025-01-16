using XMediator.Api;

namespace XMediator.Unity
{
    public class UnityBannerAdsProxy : BannerAdsProxy
    {

        internal static BannerAdsProxy ProxiedAds = new MockBannerAdsProxy();

        public void SetListener(BannerAdsProxyListener listener)
        {
            ProxiedAds.SetListener(listener);
        }

        public void Create(string placementId, BannerAds.Size size, BannerAds.Position position)
        {
            ProxiedAds.Create(placementId, size, position);
        }

        public void Load(string placementId)
        {
            ProxiedAds.Load(placementId);
        }

        public void SetPosition(string placementId, BannerAds.Position position)
        {
            ProxiedAds.SetPosition(placementId, position);
        }

        public void SetAdSpace(string placementId, string adSpace)
        {
            ProxiedAds.SetAdSpace(placementId, adSpace);
        }

        public void Show(string placementId, BannerAds.Position position)
        {
            ProxiedAds.Show(placementId, position);
        }

        public void Show(string placementId)
        {
            ProxiedAds.Show(placementId);
        }

        public void Hide(string placementId)
        {
            ProxiedAds.Hide(placementId);
        }

    }
}