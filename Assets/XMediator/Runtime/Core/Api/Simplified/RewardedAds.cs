using System;

namespace XMediator.Api
{
    /// <summary>
    /// This class is the entry point for displaying rewarded ads.
    /// 
    /// To start, load a rewarded ad by providing a valid placement id in the
    /// <see cref="Load"/> method.
    /// </summary>
    ///
    /// <remarks>
    /// Note: Only one call to rewarded load per placement id is needed.
    /// Subsequent calls with the same placement id will be ignored.
    /// </remarks>
    public class RewardedAds
    {
        /// <summary>
        /// Notifies when a rewarded ad was loaded successfully, containing a <see cref="LoadResult"/> object.
        /// </summary>
        public event Action<string, LoadResult> OnLoaded;
        
        /// <summary>
        /// Notifies when a rewarded ad impression was recorded, containing an <see cref="ImpressionData"/> object.
        /// </summary>
        public event Action<string, ImpressionData> OnImpression;
        
        /// <summary>
        /// Notifies when a rewarded ad failed to show, containing a <see cref="ShowError"/> object describing the error.
        /// </summary>
        public event Action<string, ShowError> OnFailedToShow;
        
        /// <summary>
        /// Notifies when a rewarded ad was shown.
        /// </summary>
        public event Action<string> OnShowed;
        
        /// <summary>
        /// Notifies when a rewarded ad was clicked.
        /// </summary>
        public event Action<string> OnClicked;
        
        /// <summary>
        /// Notifies when a rewarded ad was dismissed.
        /// </summary>
        public event Action<string> OnDismissed;
        
        /// <summary>
        /// Notifies when the user should be granted a reward for watching an ad.
        /// This callback won't be called if the user skipped the ad.
        /// </summary>
        public event Action<string> OnEarnedReward;

        private readonly RewardedAdsProxy _rewardedAdsProxy;

        internal RewardedAds()
        {
            _rewardedAdsProxy = ProxyFactory.CreateInstance<RewardedAdsProxy>("RewardedAdsProxy");
            _rewardedAdsProxy.SetListener(new DefaultRewardedAdsProxyListener(this));
        }

        /// <summary>
        /// Starts loading a new rewarded ad.
        /// </summary>
        /// <remarks>After the first call, no more calls to this method are needed, as the ad will reload automatically.</remarks>
        /// <param name="placementId">The placement id of the ad to be loaded.</param>
        public void Load(string placementId)
        {
            _rewardedAdsProxy.Load(placementId);
        }

        /// <summary>
        /// Indicates if a rewarded ad for the placement id is ready to be shown.
        /// </summary>
        /// <param name="placementId">The placement id of the ad.</param>
        /// <returns>true if an ad is ready to be shown.</returns>
        public bool IsReady(string placementId)
        {
            return _rewardedAdsProxy.IsReady(placementId);
        }

        /// <summary>
        /// Indicates if there is a rewarded ad ready to be shown for any placement id.
        /// </summary>
        /// <returns>true if an ad is ready to be shown.</returns>
        public bool IsReady()
        {
            return _rewardedAdsProxy.IsReady();
        }

        /// <summary>
        /// Checks whether the given ad space is currently capped by the capping rules.
        /// </summary>
        /// <param name="adSpace">The space in your app from where the ad would be shown (eg: dashboard, settings).</param>
        /// <returns>true if the ad space is capped (show is not allowed by rules); false if it is allowed.</returns>
        public bool IsAdSpaceCapped(string adSpace)
        {
            return _rewardedAdsProxy.IsAdSpaceCapped(adSpace);
        }
        
        /// <summary>
        /// Shows a previously loaded rewarded ad for any placementId.
        ///
        /// If multiple ads with different placement ids were previously loaded, the sdk will try to present the best one available.
        /// </summary>
        public void Show()
        {
            _rewardedAdsProxy.Show();
        }
        
        /// <summary>
        /// Shows a previously loaded rewarded ad for the specified placementId.
        /// </summary>
        /// <param name="placementId">The placementId of the ad.</param>
        public void Show(string placementId)
        {
            _rewardedAdsProxy.Show(placementId);
        }

        /// <summary>
        /// Shows a previously loaded rewarded ad for any placementId.
        ///
        /// If multiple ads with different placement ids were previously loaded, the sdk will try to present the best one available.
        /// </summary>
        /// <param name="adSpace">The space in your app from where the ad will be shown (eg: dashboard, settings). Used for tracking.</param>
        public void ShowFromAdSpace(string adSpace)
        {
            _rewardedAdsProxy.ShowFromAdSpace(adSpace);
        }
        
        /// <summary>
        /// Shows a previously loaded rewarded ad for the specified placementId.
        /// </summary>
        /// <param name="placementId">The placementId of the ad.</param>
        /// <param name="adSpace">The space in your app from where the ad will be shown (eg: dashboard, settings). Used for tracking.</param>
        public void ShowFromAdSpace(string placementId, string adSpace)
        {
            _rewardedAdsProxy.ShowFromAdSpace(placementId, adSpace);
        }

        private class DefaultRewardedAdsProxyListener : RewardedAdsProxyListener
        {
            private readonly RewardedAds _rewardedAds;

            public DefaultRewardedAdsProxyListener(RewardedAds rewardedAds)
            {
                _rewardedAds = rewardedAds;
            }

            public void OnLoaded(string placementId, LoadResult loadResult)
            {
                _rewardedAds.OnLoaded?.Invoke(placementId, loadResult);
            }

            public void OnShowed(string placementId)
            {
                _rewardedAds.OnShowed?.Invoke(placementId);
            }

            public void OnFailedToShow(string placementId, ShowError showError)
            {
                _rewardedAds.OnFailedToShow?.Invoke(placementId, showError);
            }

            public void OnImpression(string placementId, ImpressionData impressionData)
            {
                _rewardedAds.OnImpression?.Invoke(placementId, impressionData);
            }

            public void OnClicked(string placementId)
            {
                _rewardedAds.OnClicked?.Invoke(placementId);
            }

            public void OnDismissed(string placementId)
            {
                _rewardedAds.OnDismissed?.Invoke(placementId);
            }

            public void OnEarnedReward(string placementId)
            {
                _rewardedAds.OnEarnedReward?.Invoke(placementId);
            }
        }
    }
}