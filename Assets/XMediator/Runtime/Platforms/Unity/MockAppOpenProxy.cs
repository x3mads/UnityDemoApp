using XMediator.Api;

namespace XMediator.Unity
{
    internal class MockAppOpenProxy : AppOpenProxy
    {
        private MockFullScreenAd _fullScreenAd;
        
        public void Create(string placementId, AppOpenProxyListener listener, bool test, bool verbose)
        {
            _fullScreenAd = new MockFullScreenAd("AppOpen", "apo");
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
