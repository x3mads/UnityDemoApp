namespace DemoApp
{
    public class AppConfiguration
    {
        public string AppKey { get; }
        public string BannerPlacementId { get; }
        public string InterstitialPlacementId { get; }
        public string RewardedPlacementId { get; }

        public AppConfiguration(string appKey, string bannerPlacementId, string interstitialPlacementId, string rewardedPlacementId)
        {
            AppKey = appKey;
            BannerPlacementId = bannerPlacementId;
            InterstitialPlacementId = interstitialPlacementId;
            RewardedPlacementId = rewardedPlacementId;
        }
    }
}