using System;

namespace XMediator.Api
{
    /// <summary>
    /// This class is the entry point for displaying banner ads.
    /// 
    /// To start, create a banner by providing a valid placement id in the <see cref="Create"/>
    /// method. Then call <see cref="SetPosition"/> and <see cref="Show(string)"/> or simply <see cref="Show(string,XMediator.Api.BannerAds.Position)"/>
    /// And that's it! You don't need to do anything else, as banners will refresh  automatically while visible.
    ///
    /// <remarks>
    /// Note: You can only create one banner per placement id. Subsequent calls with
    /// the same placement id will be ignored.
    /// </remarks>
    /// </summary>
    public class BannerAds
    {
        /// <summary>
        /// Notifies when a banner was loaded successfully, containing a <see cref="LoadResult"/> object.
        /// This callback can be called multiple times for each placement id, due to the banner autorefresh triggering more loads.
        /// </summary>
        public event Action<string, LoadResult> OnLoaded;

        /// <summary>
        /// Notifies when a banner impression was recorded, containing an <see cref="ImpressionData"/> object.
        /// It can be called multiple times for each placement id due to banner autorefreshing.
        /// </summary>
        public event Action<string, ImpressionData> OnImpression;

        /// <summary>
        /// Notifies when a banner was clicked.
        /// </summary>
        public event Action<string> OnClicked;

        private readonly BannerAdsProxy _bannerAdsProxy;

        /// <summary>
        /// Class representing the available sizes that a banner can be created with.
        /// </summary>
        public class Size
        {
            private const int PhoneIdentifier = 0;
            private const int TabletIdentifier = 1;
            private const int MrecIdentifier = 2;

            internal int Identifier { get; private set; }

            public int Width { get; private set; }

            public int Height { get; private set; }

            /// <summary>
            /// Phone size for banners, typically 320x50
            /// </summary>
            public static readonly Size Phone = new Size(PhoneIdentifier, 320, 50);

            /// <summary>
            /// Tablet size for banners, typically 728x90
            /// </summary>
            public static readonly Size Tablet = new Size(TabletIdentifier, 728, 90);

            /// <summary>
            /// MREC size for banners, typically 300x250
            /// </summary>
            public static readonly Size Mrec = new Size(MrecIdentifier, 300, 250);

            private Size(int identifier, int width, int height)
            {
                Identifier = identifier;
                Width = width;
                Height = height;
            }
        }

        /// <summary>
        /// Class representing the available positions where a banner can be placed.
        /// </summary>
        public class Position
        {
            private const int TopIdentifier = 0;
            private const int BottomIdentifier = 1;
            private const int CustomIdentifier = 42;

            internal int Identifier { get; private set; }
            internal int X { get; private set; }
            internal int Y { get; private set; }

            /// <summary>
            /// Top center position.
            /// </summary>
            public static readonly Position Top = new Position(TopIdentifier);

            /// <summary>
            /// Bottom center position.
            /// </summary>
            public static readonly Position Bottom = new Position(BottomIdentifier);

            /// <summary>
            /// Creates a custom banner position in x,y coordinates.
            /// </summary>
            /// <param name="x">X Coordinate from top-left corner</param>
            /// <param name="y">Y Coordinate from top-left corner</param>
            /// <returns>A <see cref="Position"/> object containing the specified x,y position.</returns>
            public static Position Custom(int x, int y)
            {
                return new Position(CustomIdentifier, x, y);
            }

            private Position(int identifier, int x = 0, int y = 0)
            {
                Identifier = identifier;
                X = x;
                Y = y;
            }

            internal bool IsCustom()
            {
                return Identifier == CustomIdentifier;
            }
        }

        internal BannerAds()
        {
            _bannerAdsProxy = ProxyFactory.CreateInstance<BannerAdsProxy>("BannerAdsProxy");
            _bannerAdsProxy.SetListener(new DefaultBannerAdsProxyListener(this));
        }

        /// <summary>
        /// Creates and loads a new banner.
        /// </summary>
        /// <param name="placementId">A string containing a valid placement id.</param>
        /// <param name="size">Size of the banner. See <see cref="Size"/> for the available sizes.</param>
        /// <param name="position">Position on the screen where the banner will appear after it has been loaded. See <see cref="Position"/>.</param>
        public void Create(string placementId, Size size, Position position)
        {
            _bannerAdsProxy.Create(placementId, size, position);
        }

        /// <summary>
        /// Manually refreshes a banner ad.
        /// </summary>
        /// <remarks>
        /// The usage of this method is optional, as banner ads refresh automatically.
        /// </remarks>
        /// <param name="placementId">The placement id of the ad to be refreshed.</param>
        public void Load(string placementId)
        {
            _bannerAdsProxy.Load(placementId);
        }

        /// <summary>
        /// Sets the position of a banner.
        /// </summary>
        /// <param name="placementId">The placement id of the banner to change position.</param>
        /// <param name="position">The new position for the banner.</param>
        public void SetPosition(string placementId, Position position)
        {
            _bannerAdsProxy.SetPosition(placementId, position);
        }

        /// <summary>
        /// Sets the ad space for the specified placementId.
        /// </summary>
        /// <param name="placementId">The placement id of the banner ad.</param>
        /// <param name="adSpace">The space in your app from where the ad will be shown (eg: dashboard, settings). Set null to reset adSpace. Used for tracking.</param>
        public void SetAdSpace(string placementId, string adSpace)
        {
            _bannerAdsProxy.SetAdSpace(placementId, adSpace);
        }

        /// <summary>
        /// Shows a banner at the given position. It doesn't trigger a new load.
        /// </summary>
        /// <param name="placementId">The placement id of the ad to show.</param>
        /// <param name="position">The new position for the banner.</param>
        public void Show(string placementId, Position position)
        {
            _bannerAdsProxy.Show(placementId, position);
        }

        /// <summary>
        /// Shows a banner at the last known position. It doesn't trigger a new load.
        /// </summary>
        /// <param name="placementId">The placement id of the ad to show.</param>
        public void Show(string placementId)
        {
            _bannerAdsProxy.Show(placementId);
        }

        /// <summary>
        /// Hides a banner ad.
        /// </summary>
        /// <param name="placementId">The placement id of the ad to hide.</param>
        public void Hide(string placementId)
        {
            _bannerAdsProxy.Hide(placementId);
        }

        private class DefaultBannerAdsProxyListener : BannerAdsProxyListener
        {
            private readonly BannerAds _bannerAds;

            public DefaultBannerAdsProxyListener(BannerAds bannerAds)
            {
                _bannerAds = bannerAds;
            }

            public void OnLoaded(string placementId, LoadResult loadResult)
            {
                _bannerAds.OnLoaded?.Invoke(placementId, loadResult);
            }

            public void OnImpression(string placementId, ImpressionData impressionData)
            {
                _bannerAds.OnImpression?.Invoke(placementId, impressionData);
            }

            public void OnClicked(string placementId)
            {
                _bannerAds.OnClicked?.Invoke(placementId);
            }
        }
    }
}