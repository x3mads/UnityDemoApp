using XMediator.Api;

namespace XMediator
{
    public interface BannerAdsProxyListener
    {
        void OnLoaded(string placementId, LoadResult loadResult);

        void OnImpression(string placementId, ImpressionData impressionData);

        void OnClicked(string placementId);
    }
}
