using XMediator.Api;

namespace XMediator.Unity
{
    internal class MockRewardedProxy : RewardedProxy
    {
        private MockFullScreenAd _fullScreenAd;
        
        public void Create(string placementId, RewardedProxyListener listener, bool test, bool verbose)
        {
            _fullScreenAd = new MockFullScreenAd("Rewarded", "rew");
            _fullScreenAd.Create(placementId, new MockToBackgroundAdListener(listener, listener, listener));
        }

        public void Load(CustomProperties customProperties)
        {
            _fullScreenAd.Load(customProperties);
        }

        public bool IsReady()
        {
            return _fullScreenAd.IsReady();
        }

        public void Show()
        {
            _fullScreenAd.Show();
        }

        public void Show(string adSpace)
        {
            _fullScreenAd.Show(adSpace);
        }

        public void Dispose()
        {
            _fullScreenAd.Dispose();
        }
    }
}