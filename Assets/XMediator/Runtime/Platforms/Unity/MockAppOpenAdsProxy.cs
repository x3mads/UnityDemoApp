using System.Collections.Generic;
using XMediator.Api;

namespace XMediator.Unity
{
    internal class MockAppOpenAdsProxy : AppOpenAdsProxy
    {
        internal AppOpenAdsProxyListener Listener;
        private Dictionary<string, MockAppOpenProxy> Ads = new Dictionary<string, MockAppOpenProxy>();

        public void SetListener(AppOpenAdsProxyListener listener)
        {
            Listener = listener;
        }

        public void Load(string placementId)
        {
            if (Ads.ContainsKey(placementId))
            {
                return;
            }

            InternalLoad(placementId);
        }

        public bool IsReady()
        {
            foreach (var ad in Ads.Values)
            {
                if (ad.IsReady())
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsReady(string placementId)
        {
            return Ads.ContainsKey(placementId) && Ads[placementId].IsReady();
        }

        public void Show()
        {
            foreach (var ad in Ads.Values)
            {
                if (ad.IsReady())
                {
                    ad.Show();
                    return;
                }
            }
        }

        public void Show(string placementId)
        {
            if (Ads.ContainsKey(placementId) && Ads[placementId].IsReady())
            {
                Ads[placementId].Show();
            }
        }

        public void ShowFromAdSpace(string adSpace)
        {
            Show();
        }

        public void ShowFromAdSpace(string placementId, string adSpace)
        {
            Show(placementId);
        }

        private void InternalLoad(string placementId)
        {
            var ad = new MockAppOpenProxy();
            ad.Create(placementId, new MockAppOpenProxyListener(this, placementId), true, true);
            ad.Load(new CustomProperties.Builder().Build());
            Ads[placementId] = ad;
        }

        private class MockAppOpenProxyListener : AppOpenProxyListener
        {
            private readonly MockAppOpenAdsProxy _proxy;
            private readonly string _placementId;

            public MockAppOpenProxyListener(MockAppOpenAdsProxy proxy, string placementId)
            {
                _proxy = proxy;
                _placementId = placementId;
            }

            public void OnPrebiddingFinished(PrebiddingResults result)
            {
            }

            public void OnLoaded(LoadResult loadResult)
            {
                _proxy.Listener.OnLoaded(_placementId, loadResult);
            }

            public void OnFailedToLoad(LoadError loadError, LoadResult loadResult)
            {
                _proxy.InternalLoad(_placementId);
            }

            public void OnShowed()
            {
                _proxy.Listener.OnShowed(_placementId);
            }

            public void OnFailedToShow(ShowError showError)
            {
                _proxy.InternalLoad(_placementId);
                _proxy.Listener.OnFailedToShow(_placementId, showError);
            }

            public void OnImpression(ImpressionData impressionData)
            {
                _proxy.Listener.OnImpression(_placementId, impressionData);
            }

            public void OnClicked()
            {
                _proxy.Listener.OnClicked(_placementId);
            }

            public void OnDismissed()
            {
                _proxy.InternalLoad(_placementId);
                _proxy.Listener.OnDismissed(_placementId);
            }
        }
    }
}
