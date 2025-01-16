using XMediator.Api;

namespace XMediator
{
    internal interface BannerAdsProxy
    {
        void SetListener(BannerAdsProxyListener listener);
        
        void Create(string placementId, BannerAds.Size size, BannerAds.Position position);
        
        void Load(string placementId);
        
        void SetPosition(string placementId, BannerAds.Position position);

        void SetAdSpace(string placementId, string adSpace);
        
        void Show(string placementId, BannerAds.Position position);
        
        void Show(string placementId);
        
        void Hide(string placementId);

    }
}