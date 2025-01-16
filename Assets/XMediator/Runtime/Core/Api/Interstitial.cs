using System;

namespace XMediator.Api
{
    /// <summary>
    /// This class represents an Interstitial Ad format.
    /// </summary>
    public class Interstitial : IDisposable
    {
        private readonly InterstitialProxy _interstitialProxy;

        /// <summary>
        /// Notifies when the prebidding call finished, containing a <see cref="PrebiddingResults"/> object.
        /// </summary>
        public event Action<PrebiddingResults> OnPrebiddingFinished;

        /// <summary>
        /// Notifies when the interstitial was loaded successfully, containing a <see cref="LoadResult"/> object.
        /// </summary>
        public event Action<LoadResult> OnLoaded;

        /// <summary>
        /// Notifies when the interstitial failed to load, containing a <see cref="LoadError"/> object describing the error
        /// and optionally a <see cref="LoadResult"/> object.
        /// </summary>
        public event Action<LoadError, LoadResult> OnFailedToLoad;

        /// <summary>
        /// Notifies when the interstitial failed to show, containing a <see cref="ShowError"/> object describing the error.
        /// </summary>
        public event Action<ShowError> OnFailedToShow;

        /// <summary>
        /// Notifies when the interstitial was shown.
        /// </summary>
        public event Action OnShowed;

        /// <summary>
        /// Notifies when the interstitial impression was recorded, containing an <see cref="ImpressionData"/> object.
        /// </summary>
        public event Action<ImpressionData> OnImpression;

        /// <summary>
        /// Notifies when the interstitial was clicked.
        /// </summary>
        public event Action OnClicked;

        /// <summary>
        /// Notifies when the interstitial was dismissed.
        /// </summary>
        public event Action OnDismissed;

        /// <summary>
        /// Creates a new instance of a <see cref="Interstitial"/>.
        /// </summary>
        /// <param name="placementId">A string containing a valid placement id.</param>
        /// <param name="test">Whether a test interstitial should be loaded. This is only for testing purposes, should be false on production builds.</param>
        /// <param name="verbose">Enable logging for this interstitial instance. This is only for debugging purposes, should be false on production builds.</param>
        /// <returns>An <see cref="Interstitial"/> instance ready to load.</returns>
        public static Interstitial Create(string placementId, bool test = false, bool verbose = false)
        {
            var interstitialProxy = ProxyFactory.CreateInstance<InterstitialProxy>("InterstitialProxy");
            return new Interstitial(interstitialProxy, placementId, test, verbose);
        }

        private Interstitial(InterstitialProxy interstitialProxy, string placementId, bool test, bool verbose)
        {
            _interstitialProxy = interstitialProxy;
            interstitialProxy.Create(placementId, new DefaultInterstitialProxyListener(this), verbose, test);
        }

        /// <summary>
        /// Starts a new load call.
        /// </summary>
        /// <param name="customProperties">A <see cref="CustomProperties"/> object containing custom properties, useful for tracking.</param>
        public void Load(CustomProperties customProperties = null)
        {
            _interstitialProxy.Load(customProperties ?? new CustomProperties.Builder().Build());
        }

        /// <summary>
        /// Indicates if the interstitial is ready to show.
        /// </summary>
        /// <returns><see langword="true"/> if the interstitial is ready to show, <see langword="false"/> otherwise.</returns>
        public bool IsReady() => _interstitialProxy.IsReady();

        /// <summary>
        /// Shows a previously loaded interstitial.
        /// </summary>
        public void Show() => _interstitialProxy.Show();
        
        /// <summary>
        /// Shows a previously loaded interstitial.
        /// </summary>
        /// <param name="adSpace">The space in your app from where the ad will be shown (eg: dashboard, settings). Used for tracking.</param>
        public void Show(string adSpace) => _interstitialProxy.Show(adSpace);

        /// <summary>
        /// Disposes this interstitial object and can no longer be used for loading or showing.
        /// </summary>
        public void Dispose() => _interstitialProxy.Dispose();

        class DefaultInterstitialProxyListener : InterstitialProxyListener
        {
            private readonly Interstitial _interstitial;

            public DefaultInterstitialProxyListener(Interstitial interstitial)
            {
                _interstitial = interstitial;
            }

            public void OnPrebiddingFinished(PrebiddingResults result)
            {
                _interstitial.OnPrebiddingFinished?.Invoke(result);
            }

            public void OnLoaded(LoadResult loadResult)
            {
                _interstitial.OnLoaded?.Invoke(loadResult);
            }

            public void OnFailedToLoad(LoadError loadError, LoadResult loadResult)
            {
                _interstitial.OnFailedToLoad?.Invoke(loadError, loadResult);
            }

            public void OnFailedToShow(ShowError showError)
            {
                _interstitial.OnFailedToShow?.Invoke(showError);
            }

            public void OnShowed()
            {
                _interstitial.OnShowed?.Invoke();
            }

            public void OnImpression(ImpressionData impressionData)
            {
                _interstitial.OnImpression?.Invoke(impressionData);
            }

            public void OnClicked()
            {
                _interstitial.OnClicked?.Invoke();
            }

            public void OnDismissed()
            {
                _interstitial.OnDismissed?.Invoke();
            }
        }
    }
}