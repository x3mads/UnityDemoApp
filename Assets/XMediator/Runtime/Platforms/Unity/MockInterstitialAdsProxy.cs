using System.Collections.Generic;
using System.Linq;
using XMediator.Api;

namespace XMediator.Unity
{
    internal class MockInterstitialAdsProxy : InterstitialAdsProxy
    {
        internal InterstitialAdsProxyListener Listener;
        private Dictionary<string, MockInterstitialProxy> Ads = new Dictionary<string, MockInterstitialProxy>();

        public void SetListener(InterstitialAdsProxyListener listener)
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
            if (!Ads.Any())
            {
                return false;
            }

            return Ads.First().Value.IsReady();
        }
        
        public bool IsReady(string placementId)
        {
            if (!Ads.ContainsKey(placementId))
            {
                return false;
            }

            return Ads[placementId].IsReady();
        }

        public bool IsAdSpaceCapped(string adSpace)
        {
            // TODO-CAPPING Revisar si es correcto esto
            foreach (var ad in Ads.Values)
            {
                if (ad.IsAdSpaceCapped(adSpace))
                {
                    return true;
                }
            }

            return false;
        }
        
        public void Show()
        {
            InternalShowAny();
        }

        public void Show(string placementId)
        {
            InternalShowPlacement(placementId);
        }

        public void ShowFromAdSpace(string adSpace)
        {
            InternalShowAny(adSpace);
        }

        public void ShowFromAdSpace(string placementId, string adSpace)
        {
            InternalShowPlacement(placementId, adSpace);
        }
        
        private void InternalShowAny(string adSpace = null)
        {
            if (!Ads.Any())
            {
                Listener.OnFailedToShow("",
                    new ShowError(ShowError.ErrorType.NotRequested, "Remember to call Load before trying to show an ad."));
                return;
            }

            Ads.First().Value.Show(adSpace);
        }
        
        private void InternalShowPlacement(string placementId, string adSpace = null)
        {
            if (!Ads.ContainsKey(placementId))
            {
                Listener.OnFailedToShow(placementId,
                    new ShowError(ShowError.ErrorType.NotRequested, "Remember to call Load before trying to show an ad."));
                return;
            }

            Ads[placementId]?.Show(adSpace);
        }

        internal void InternalLoad(string placementId)
        {
            var mockInterstitialProxy = new MockInterstitialProxy();
            var mockInterstitialProxyListener = new MockInterstitialProxyListener(placementId, this);
            mockInterstitialProxy.Create(placementId, mockInterstitialProxyListener, true, true);

            Ads[placementId] = mockInterstitialProxy;
            
            mockInterstitialProxy.Load(new CustomProperties.Builder().Build());
        }
        
        private class MockInterstitialProxyListener : InterstitialProxyListener
            {
                private readonly string _placementId;
                private readonly MockInterstitialAdsProxy _proxy;
        
                public MockInterstitialProxyListener(string placementId, MockInterstitialAdsProxy proxy)
                {
                    _placementId = placementId;
                    _proxy = proxy;
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
        
                public void OnFailedToShow(ShowError showError)
                {
                    _proxy.InternalLoad(_placementId);
                    _proxy.Listener.OnFailedToShow(_placementId, showError);
                }
        
                public void OnShowed()
                {
                    _proxy.Listener.OnShowed(_placementId);
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