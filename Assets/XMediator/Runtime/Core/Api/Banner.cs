using System;

namespace XMediator.Api
{
    /// <summary>
    /// This class represents a Banner Ad format.
    /// </summary>
    public class Banner : IDisposable
    {
        private readonly BannerProxy _bannerProxy;

        /// <summary>
        /// Creates a new instance of a <see cref="Banner"/>.
        /// </summary>
        /// <param name="placementId">A string containing a valid placement id.</param>
        /// <param name="position">Position on the screen where the banner will appear after it has been loaded. See <see cref="Position"/>.</param>
        /// <param name="size">Size of the banner. See <see cref="Size"/> for the available sizes.</param>
        /// <param name="test">Whether a test banner should be loaded. This is only for testing purposes, should be false on production builds.</param>
        /// <param name="verbose">Enable logging for this banner instance. This is only for debugging purposes, should be false on production builds.</param>
        /// <returns>A <see cref="Banner"/> instance ready to load.</returns>
        public static Banner Create(string placementId, Position position = Position.Bottom, Size size = Size.Phone,
            bool test = false, bool verbose = false)
        {
            var bannerProxy = ProxyFactory.CreateInstance<BannerProxy>("BannerProxy");
            return new Banner(bannerProxy, placementId, position, size, test, verbose);
        }

        /// <summary>
        /// Creates a new instance of a <see cref="Banner"/>.
        /// </summary>
        /// <param name="placementId">A string containing a valid placement id.</param>
        /// <param name="x">X Coordinate from top-left corner.</param>
        /// <param name="y">Y Coordinate from top-left corner.</param>
        /// <param name="size">Size of the banner. See <see cref="Size"/> for the available sizes.</param>
        /// <param name="test">Whether a test banner should be loaded. This is only for testing purposes, should be false on production builds.</param>
        /// <param name="verbose">Enable logging for this banner instance. This is only for debugging purposes, should be false on production builds.</param>
        /// <returns>A <see cref="Banner"/> instance ready to load.</returns>
        public static Banner Create(string placementId, int x, int y, Size size, bool test = false, bool verbose = false)
        {
            var bannerProxy = ProxyFactory.CreateInstance<BannerProxy>("BannerProxy");
            return new Banner(bannerProxy, placementId, size, x, y, test, verbose);
        }

        /// <summary>
        /// Notifies when the prebidding call finished, containing a <see cref="PrebiddingResults"/> object.
        /// </summary>
        public event Action<PrebiddingResults> OnPrebiddingFinished;

        /// <summary>
        /// Notifies when the banner was loaded successfully, containing a <see cref="LoadResult"/> object.
        /// This callback can be called multiple times, due to the banner autorefresh triggering more loads.
        /// </summary>
        public event Action<LoadResult> OnLoaded;

        /// <summary>
        /// Notifies when the banner failed to load, containing a <see cref="LoadError"/> object describing the error
        /// and optionally a <see cref="LoadResult"/> object.
        /// </summary>
        public event Action<LoadError, LoadResult> OnFailedToLoad;

        /// <summary>
        /// Notifies when the banner failed to show, containing a <see cref="ShowError"/> object describing the error.
        /// </summary>
        public event Action<ShowError> OnFailedToShow;

        /// <summary>
        /// Notifies when the banner was shown. It can be called multiple times due to banner autorefreshing.
        /// </summary>
        public event Action OnShowed;

        /// <summary>
        /// Notifies when the banner impression was recorded, containing an <see cref="ImpressionData"/> object.
        /// It can be called multiple times due to banner autorefreshing.
        /// </summary>
        public event Action<ImpressionData> OnImpression;

        /// <summary>
        /// Notifies when the banner was clicked.
        /// </summary>
        public event Action OnClicked;

        /// <summary>
        /// Notifies when the banner was dismissed.
        /// </summary>
        public event Action OnDismissed;

        private Banner(BannerProxy bannerProxy, string placementId, Position position, Size size, bool test,
            bool verbose)
        {
            _bannerProxy = bannerProxy;
            bannerProxy.Create(placementId, new DefaultBannerProxyListener(this), position, size, test, verbose);
        }
        
        private Banner(BannerProxy bannerProxy, string placementId, Size size, int x, int y, bool test, bool verbose)
        {
            _bannerProxy = bannerProxy;
            bannerProxy.Create(placementId, new DefaultBannerProxyListener(this), size, x, y, test, verbose);
        }
        
        /// <summary>
        /// Returns the width of the specified banner size.
        /// </summary>
        /// <param name="size">The banner size</param>
        /// <returns></returns>
        public static int GetWidthFor(Size size)
        {
            switch (size)
            {
                case Size.Tablet:
                    return 728;
                case Size.MREC:
                    return 300;
                case Size.Phone:
                default:
                    return 320;
            }
        }
        
        /// <summary>
        /// Returns the height of the specified banner size.
        /// </summary>
        /// <param name="size">The banner size</param>
        /// <returns></returns>
        public static int GetHeightFor(Size size)
        {
            switch (size)
            {
                case Size.Tablet:
                    return 90;
                case Size.MREC:
                    return 250;
                case Size.Phone:
                default:
                    return 50;
            }
        }

        /// <summary>
        /// Starts a new load call.
        /// After it loads successfully, it is automatically shown at the position specified when <see cref="Create"/> was called.  
        /// </summary>
        /// <param name="customProperties">A <see cref="CustomProperties"/> object containing custom properties, useful for tracking.</param>
        public void Load(CustomProperties customProperties = null)
        {
            _bannerProxy.Load(customProperties ?? new CustomProperties.Builder().Build());
        }

        /// <summary>
        /// Sets a custom position for the banner.
        /// </summary>
        /// <param name="x">X Coordinate from top-left corner</param>
        /// <param name="y">Y Coordinate from top-left corner</param>
        public void SetPosition(int x, int y)
        {
            _bannerProxy.SetPosition(x, y);
        }

        /// <summary>
        /// Sets the ad space where the ad will be shown.
        /// </summary>
        /// <param name="adSpace">The space in your app from where the ad will be shown (eg: dashboard, settings). Used for tracking.</param>
        public void SetAdSpace(string adSpace)
        {
            _bannerProxy.SetAdSpace(adSpace);
        }

        /// <summary>
        /// Shows the banner at the last known position. It doesn't trigger a new load.
        /// </summary>
        public void Show()
        {
            _bannerProxy.Show();
        }

        /// <summary>
        /// Shows the banner at the indicated position. It doesn't trigger a new load.
        /// </summary>
        /// <param name="position">The <see cref="Position"/> where the banner will be placed.</param>
        public void Show(Position position)
        {
            _bannerProxy.Show(position);
        }

        /// <summary>
        /// Disposes this banner object and can no longer be used for loading or showing.
        /// </summary>
        public void Dispose()
        {
            _bannerProxy.Dispose();
        }

        /// <summary>
        /// Hides the banner.
        /// </summary>
        public void Hide()
        {
            _bannerProxy.Hide();
        }

        /// <summary>
        /// Enum class representing the available positions where a banner can be placed.
        /// </summary>
        public enum Position
        {
            Top = 0,
            Bottom
        }

        /// <summary>
        /// Enum class representing the available sizes that a banner can be created with.
        /// </summary>
        public enum Size
        {
            Phone = 0,
            Tablet = 1,
            MREC = 2
        }

        class DefaultBannerProxyListener : BannerProxyListener
        {
            private readonly Banner _banner;

            public DefaultBannerProxyListener(Banner banner)
            {
                _banner = banner;
            }

            public void OnPrebiddingFinished(PrebiddingResults result)
            {
                _banner.OnPrebiddingFinished?.Invoke(result);
            }

            public void OnLoaded(LoadResult loadResult)
            {
                _banner.OnLoaded?.Invoke(loadResult);
            }

            public void OnFailedToLoad(LoadError loadError, LoadResult loadResult)
            {
                _banner.OnFailedToLoad?.Invoke(loadError, loadResult);
            }

            public void OnFailedToShow(ShowError showError)
            {
                _banner.OnFailedToShow?.Invoke(showError);
            }

            public void OnShowed()
            {
                _banner.OnShowed?.Invoke();
            }

            public void OnImpression(ImpressionData impressionData)
            {
                _banner.OnImpression?.Invoke(impressionData);
            }

            public void OnClicked()
            {
                _banner.OnClicked?.Invoke();
            }

            public void OnDismissed()
            {
                _banner.OnDismissed?.Invoke();
            }
        }
    }
}