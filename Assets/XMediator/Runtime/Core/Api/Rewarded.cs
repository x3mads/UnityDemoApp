using System;

namespace XMediator.Api
{
    public class Rewarded : IDisposable
    {
        private readonly RewardedProxy _rewardedProxy;

        /// <summary>
        /// Notifies when the prebidding call finished, containing a <see cref="PrebiddingResults"/> object.
        /// </summary>
        public event Action<PrebiddingResults> OnPrebiddingFinished;

        /// <summary>
        /// Notifies when the rewarded was loaded successfully, containing a <see cref="LoadResult"/> object.
        /// </summary>
        public event Action<LoadResult> OnLoaded;

        /// <summary>
        /// Notifies when the rewarded failed to load, containing a <see cref="LoadError"/> object describing the error
        /// and optionally a <see cref="LoadResult"/> object.
        /// </summary>
        public event Action<LoadError, LoadResult> OnFailedToLoad;

        /// <summary>
        /// Notifies when the rewarded failed to show, containing a <see cref="ShowError"/> object describing the error.
        /// </summary>
        public event Action<ShowError> OnFailedToShow;

        /// <summary>
        /// Notifies when the rewarded was shown.
        /// </summary>
        public event Action OnShowed;

        /// <summary>
        /// Notifies when the rewarded impression was recorded, containing an <see cref="ImpressionData"/> object.
        /// </summary>
        public event Action<ImpressionData> OnImpression;

        /// <summary>
        /// Notifies when the rewarded was clicked.
        /// </summary>
        public event Action OnClicked;

        /// <summary>
        /// Notifies when the rewarded was dismissed.
        /// </summary>
        public event Action OnDismissed;

        /// <summary>
        /// Notifies when the user should be granted a reward for watching the ad.
        /// This callback won't be called if the user skipped the ad.
        /// </summary>
        public event Action OnEarnedReward;

        /// <summary>
        /// Creates a new instance of a <see cref="Rewarded"/>.
        /// </summary>
        /// <param name="placementId">A string containing a valid placement id.</param>
        /// <param name="test">Whether a test rewarded should be loaded. This is only for testing purposes, should be false on production builds.</param>
        /// <param name="verbose">Enable logging for this rewarded instance. This is only for debugging purposes, should be false on production builds.</param>
        /// <returns>A <see cref="Rewarded"/> instance ready to load.</returns>
        public static Rewarded Create(string placementId, bool test = false, bool verbose = false)
        {
            var rewardedProxy = ProxyFactory.CreateInstance<RewardedProxy>("RewardedProxy");
            return new Rewarded(rewardedProxy, placementId, test, verbose);
        }

        private Rewarded(RewardedProxy rewardedProxy, string placementId, bool test, bool verbose)
        {
            _rewardedProxy = rewardedProxy;
            rewardedProxy.Create(placementId, new DefaultRewardedProxyListener(this), test, verbose);
        }

        /// <summary>
        /// Starts a new load call.
        /// </summary>
        /// <param name="customProperties">A <see cref="CustomProperties"/> object containing custom properties, useful for tracking.</param>
        public void Load(CustomProperties customProperties = null)
        {
            _rewardedProxy.Load(customProperties ?? new CustomProperties.Builder().Build());
        }

        /// <summary>
        /// Indicates if the rewarded is ready to show.
        /// </summary>
        /// <returns><see langword="true"/> if the rewarded is ready to show, <see langword="false"/> otherwise.</returns>
        public bool IsReady() => _rewardedProxy.IsReady();

        /// <summary>
        /// Checks whether the given ad space is currently capped by the capping rules.
        /// </summary>
        /// <param name="adSpace">The space in your app from where the ad would be shown (eg: dashboard, settings).</param>
        /// <returns>true if the ad space is capped (show is not allowed by rules); false if it is allowed.</returns>
        public bool IsAdSpaceCapped(string adSpace)
        {
            return _rewardedProxy.IsAdSpaceCapped(adSpace);
        }

        /// <summary>
        /// Shows a previously loaded rewarded.
        /// </summary>
        public void Show() => _rewardedProxy.Show();
        
        /// <summary>
        /// Shows a previously loaded rewarded.
        /// </summary>
        /// <param name="adSpace">The space in your app from where the ad will be shown (eg: dashboard, settings). Used for tracking.</param>
        public void Show(string adSpace) => _rewardedProxy.Show(adSpace);

        /// <summary>
        /// Disposes this interstitial object and can no longer be used for loading or showing.
        /// </summary>
        public void Dispose() => _rewardedProxy.Dispose();

        class DefaultRewardedProxyListener : RewardedProxyListener
        {
            private readonly Rewarded _rewarded;

            public DefaultRewardedProxyListener(Rewarded rewarded)
            {
                _rewarded = rewarded;
            }

            public void OnPrebiddingFinished(PrebiddingResults result)
            {
                _rewarded.OnPrebiddingFinished?.Invoke(result);
            }

            public void OnLoaded(LoadResult loadResult)
            {
                _rewarded.OnLoaded?.Invoke(loadResult);
            }

            public void OnFailedToLoad(LoadError loadError, LoadResult loadResult)
            {
                _rewarded.OnFailedToLoad?.Invoke(loadError, loadResult);
            }

            public void OnFailedToShow(ShowError showError)
            {
                _rewarded.OnFailedToShow?.Invoke(showError);
            }

            public void OnShowed()
            {
                _rewarded.OnShowed?.Invoke();
            }

            public void OnImpression(ImpressionData impressionData)
            {
                _rewarded.OnImpression?.Invoke(impressionData);
            }

            public void OnClicked()
            {
                _rewarded.OnClicked?.Invoke();
            }

            public void OnDismissed()
            {
                _rewarded.OnDismissed?.Invoke();
            }

            public void OnEarnedReward()
            {
                _rewarded.OnEarnedReward?.Invoke();
            }
        }
    }
}