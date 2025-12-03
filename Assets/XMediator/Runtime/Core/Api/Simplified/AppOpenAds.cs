using System;

namespace XMediator.Api
{
    /// <summary>
    /// This class is the entry point for displaying app open ads.
    /// 
    /// To start, load an app open ad by providing a valid placement id in the
    /// <see cref="Load"/> method.
    /// </summary>
    ///
    /// <remarks>
    /// Note: Only one call to app open load per placement id is needed.
    /// Subsequent calls with the same placement id will be ignored.
    /// </remarks>
    public class AppOpenAds
    {
        /// <summary>
        /// Notifies when an app open ad was loaded successfully, containing a <see cref="LoadResult"/> object.
        /// </summary>
        public event Action<string, LoadResult> OnLoaded;

        /// <summary>
        /// Notifies when an app open ad was shown.
        /// </summary>
        public event Action<string> OnShowed;

        /// <summary>
        /// Notifies when an app open ad impression was recorded, containing an <see cref="ImpressionData"/> object.
        /// </summary>
        public event Action<string, ImpressionData> OnImpression;

        /// <summary>
        /// Notifies when an app open ad was dismissed.
        /// </summary>
        public event Action<string> OnDismissed;

        /// <summary>
        /// Notifies when an app open ad failed to show, containing a <see cref="ShowError"/> object describing the error.
        /// </summary>
        public event Action<string, ShowError> OnFailedToShow;

        /// <summary>
        /// Notifies when an app open ad was clicked.
        /// </summary>
        public event Action<string> OnClicked;

        private readonly AppOpenAdsProxy _appOpenAdsProxy;

        internal AppOpenAds()
        {
            _appOpenAdsProxy = ProxyFactory.CreateInstance<AppOpenAdsProxy>("AppOpenAdsProxy");
            _appOpenAdsProxy.SetListener(new DefaultAppOpenAdsProxyListener(this));
        }

        /// <summary>
        /// Starts loading a new app open ad.
        /// </summary>
        /// <remarks>After the first call, no more calls to this method are needed, as the ad will reload automatically.</remarks>
        /// <param name="placementId">The placement id of the ad to be loaded.</param>
        public void Load(string placementId)
        {
            _appOpenAdsProxy.Load(placementId);
        }

        /// <summary>
        /// Indicates if an app open ad for the placement id is ready to be shown.
        /// </summary>
        /// <param name="placementId">The placement id of the ad.</param>
        /// <returns>true if an ad is ready to be shown.</returns>
        public bool IsReady(string placementId)
        {
            return _appOpenAdsProxy.IsReady(placementId);
        }

        /// <summary>
        /// Indicates if there is an app open ad ready to be shown for any placement id.
        /// </summary>
        /// <returns>true if an ad is ready to be shown.</returns>
        public bool IsReady()
        {
            return _appOpenAdsProxy.IsReady();
        }

        /// <summary>
        /// Checks whether the given ad space is currently capped by the capping rules.
        /// </summary>
        /// <param name="adSpace">The space in your app from where the ad would be shown (eg: dashboard, settings).</param>
        /// <returns>true if the ad space is capped (show is not allowed by rules); false if it is allowed.</returns>
        public bool IsAdSpaceCapped(string adSpace)
        {
            return _appOpenAdsProxy.IsAdSpaceCapped(adSpace);
        }

        /// <summary>
        /// Shows a previously loaded app open ad for any placementId.
        ///
        /// If multiple ads with different placement ids were previously loaded, the sdk will try to present the best one available.
        /// </summary>
        public void Show()
        {
            _appOpenAdsProxy.Show();
        }

        /// <summary>
        /// Shows a previously loaded app open ad for the specified placementId.
        /// </summary>
        /// <param name="placementId">The placementId of the ad.</param>
        public void Show(string placementId)
        {
            _appOpenAdsProxy.Show(placementId);
        }
        
        /// <summary>
        /// Shows a previously loaded app open ad for any placementId.
        ///
        /// If multiple ads with different placement ids were previously loaded, the sdk will try to present the best one available.
        /// </summary>
        /// <param name="adSpace">The space in your app from where the ad will be shown (eg: dashboard, settings). Used for tracking.</param>
        public void ShowFromAdSpace(string adSpace)
        {
            _appOpenAdsProxy.ShowFromAdSpace(adSpace);
        }
        
        /// <summary>
        /// Shows a previously loaded app open ad for the specified placementId.
        /// </summary>
        /// <param name="placementId">The placementId of the ad.</param>
        /// <param name="adSpace">The space in your app from where the ad will be shown (eg: dashboard, settings). Used for tracking.</param>
        public void ShowFromAdSpace(string placementId, string adSpace)
        {
            _appOpenAdsProxy.ShowFromAdSpace(placementId, adSpace);
        }
        
        private class DefaultAppOpenAdsProxyListener : AppOpenAdsProxyListener
        {
            private readonly AppOpenAds _appOpenAds;

            public DefaultAppOpenAdsProxyListener(AppOpenAds appOpenAds)
            {
                _appOpenAds = appOpenAds;
            }

            public void OnLoaded(string placementId, LoadResult loadResult)
            {
                _appOpenAds.OnLoaded?.Invoke(placementId, loadResult);
            }

            public void OnShowed(string placementId)
            {
                _appOpenAds.OnShowed?.Invoke(placementId);
            }

            public void OnFailedToShow(string placementId, ShowError showError)
            {
                _appOpenAds.OnFailedToShow?.Invoke(placementId, showError);
            }

            public void OnImpression(string placementId, ImpressionData impressionData)
            {
                _appOpenAds.OnImpression?.Invoke(placementId, impressionData);
            }

            public void OnClicked(string placementId)
            {
                _appOpenAds.OnClicked?.Invoke(placementId);
            }

            public void OnDismissed(string placementId)
            {
                _appOpenAds.OnDismissed?.Invoke(placementId);
            }
        }
    }
}
