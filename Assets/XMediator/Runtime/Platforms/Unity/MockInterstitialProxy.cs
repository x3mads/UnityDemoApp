using XMediator.Api;

namespace XMediator.Unity
{
    internal class MockInterstitialProxy : InterstitialProxy
    {
        private MockFullScreenAd _fullScreenAd;
        
        public void Create(string placementId, InterstitialProxyListener listener, bool test, bool verbose)
        {
            _fullScreenAd = new MockFullScreenAd("Interstitial", "itt");
            _fullScreenAd.Create(placementId, new MockToBackgroundAdListener(listener, listener));
        }

        public void Load(CustomProperties customProperties)
        {
            _fullScreenAd.Load(customProperties);
        }

        public bool IsReady()
        {
            return _fullScreenAd.IsReady();
        }

        public bool IsAdSpaceCapped(string adSpace)
        {
            return _fullScreenAd.IsAdSpaceCapped(adSpace);
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