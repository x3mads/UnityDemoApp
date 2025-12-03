using System;

namespace XMediator.Api
{
    /// <summary>
    /// This class is the entry point for displaying interstitial ads.
    /// 
    /// To start, load an interstitial ad by providing a valid placement id in the
    /// <see cref="Load"/> method.
    /// </summary>
    ///
    /// <remarks>
    /// Note: Only one call to interstitial load per placement id is needed.
    /// Subsequent calls with the same placement id will be ignored.
    /// </remarks>
    public class InterstitialAds
    {
        /// <summary>
        /// Notifies when an interstitial ad was loaded successfully, containing a <see cref="LoadResult"/> object.
        /// </summary>
        public event Action<string, LoadResult> OnLoaded;

        /// <summary>
        /// Notifies when an interstitial ad was shown.
        /// </summary>
        public event Action<string> OnShowed;

        /// <summary>
        /// Notifies when an interstitial ad impression was recorded, containing an <see cref="ImpressionData"/> object.
        /// </summary>
        public event Action<string, ImpressionData> OnImpression;

        /// <summary>
        /// Notifies when an interstitial ad was dismissed.
        /// </summary>
        public event Action<string> OnDismissed;

        /// <summary>
        /// Notifies when an interstitial ad failed to show, containing a <see cref="ShowError"/> object describing the error.
        /// </summary>
        public event Action<string, ShowError> OnFailedToShow;

        /// <summary>
        /// Notifies when an interstitial ad was clicked.
        /// </summary>
        public event Action<string> OnClicked;

        private readonly InterstitialAdsProxy _interstitialAdsProxy;

        internal InterstitialAds()
        {
            _interstitialAdsProxy = ProxyFactory.CreateInstance<InterstitialAdsProxy>("InterstitialAdsProxy");
            _interstitialAdsProxy.SetListener(new DefaultInterstitialAdsProxyListener(this));
        }

        /// <summary>
        /// Starts loading a new interstitial ad.
        /// </summary>
        /// <remarks>After the first call, no more calls to this method are needed, as the ad will reload automatically.</remarks>
        /// <param name="placementId">The placement id of the ad to be loaded.</param>
        public void Load(string placementId)
        {
            _interstitialAdsProxy.Load(placementId);
        }

        /// <summary>
        /// Indicates if an interstitial ad for the placement id is ready to be shown.
        /// </summary>
        /// <param name="placementId">The placement id of the ad.</param>
        /// <returns>true if an ad is ready to be shown.</returns>
        public bool IsReady(string placementId)
        {
            return _interstitialAdsProxy.IsReady(placementId);
        }

        /// <summary>
        /// Indicates if there is an interstitial ad ready to be shown for any placement id.
        /// </summary>
        /// <returns>true if an ad is ready to be shown.</returns>
        public bool IsReady()
        {
            return _interstitialAdsProxy.IsReady();
        }

        /// <summary>
        /// Checks whether the given ad space is currently capped by the capping rules.
        /// </summary>
        /// <param name="adSpace">The space in your app from where the ad would be shown (eg: dashboard, settings).</param>
        /// <returns>true if the ad space is capped (show is not allowed by rules); false if it is allowed.</returns>
        public bool IsAdSpaceCapped(string adSpace)
        {
            return _interstitialAdsProxy.IsAdSpaceCapped(adSpace);
        }

        /// <summary>
        /// Shows a previously loaded interstitial ad for any placementId.
        ///
        /// If multiple ads with different placement ids were previously loaded, the sdk will try to present the best one available.
        /// </summary>
        public void Show()
        {
            _interstitialAdsProxy.Show();
        }

        /// <summary>
        /// Shows a previously loaded interstitial ad for the specified placementId.
        /// </summary>
        /// <param name="placementId">The placementId of the ad.</param>
        public void Show(string placementId)
        {
            _interstitialAdsProxy.Show(placementId);
        }
        
        /// <summary>
        /// Shows a previously loaded interstitial ad for any placementId.
        ///
        /// If multiple ads with different placement ids were previously loaded, the sdk will try to present the best one available.
        /// </summary>
        /// <param name="adSpace">The space in your app from where the ad will be shown (eg: dashboard, settings). Used for tracking.</param>
        public void ShowFromAdSpace(string adSpace)
        {
            _interstitialAdsProxy.ShowFromAdSpace(adSpace);
        }
        
        /// <summary>
        /// Shows a previously loaded interstitial ad for the specified placementId.
        /// </summary>
        /// <param name="placementId">The placementId of the ad.</param>
        /// <param name="adSpace">The space in your app from where the ad will be shown (eg: dashboard, settings). Used for tracking.</param>
        public void ShowFromAdSpace(string placementId, string adSpace)
        {
            _interstitialAdsProxy.ShowFromAdSpace(placementId, adSpace);
        }
        
        private class DefaultInterstitialAdsProxyListener : InterstitialAdsProxyListener
        {
            private readonly InterstitialAds _interstitialAds;

            public DefaultInterstitialAdsProxyListener(InterstitialAds interstitialAds)
            {
                _interstitialAds = interstitialAds;
            }

            public void OnLoaded(string placementId, LoadResult loadResult)
            {
                _interstitialAds.OnLoaded?.Invoke(placementId, loadResult);
            }

            public void OnShowed(string placementId)
            {
                _interstitialAds.OnShowed?.Invoke(placementId);
            }

            public void OnFailedToShow(string placementId, ShowError showError)
            {
                _interstitialAds.OnFailedToShow?.Invoke(placementId, showError);
            }

            public void OnImpression(string placementId, ImpressionData impressionData)
            {
                _interstitialAds.OnImpression?.Invoke(placementId, impressionData);
            }

            public void OnClicked(string placementId)
            {
                _interstitialAds.OnClicked?.Invoke(placementId);
            }

            public void OnDismissed(string placementId)
            {
                _interstitialAds.OnDismissed?.Invoke(placementId);
            }
        }
    }
}