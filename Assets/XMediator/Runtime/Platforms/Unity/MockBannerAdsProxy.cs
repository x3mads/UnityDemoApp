using System.Collections.Generic;
using UnityEngine;
using XMediator.Api;

namespace XMediator.Unity
{
    internal class MockBannerAdsProxy : BannerAdsProxy
    {
        private BannerAdsProxyListener _listener;
        private Dictionary<string, MockBannerProxy> _banners = new Dictionary<string, MockBannerProxy>();

        public void SetListener(BannerAdsProxyListener listener)
        {
            _listener = listener;
        }

        public void Create(string placementId, BannerAds.Size size, BannerAds.Position position)
        {
            if (_banners.ContainsKey(placementId))
            {
                Log($"Banner {placementId} already created!");
                return;
            }

            Log($"Banner created for placement Id: {placementId}");
            var banner = new MockBannerProxy();
            (Banner.Position? bannerPosition, int x, int y) = MapPosition(position);
            Banner.Size bannerSize = MapSize(size);
            if (bannerPosition.HasValue)
            {
                banner.Create(
                    placementId,
                    new InternalBannerProyListener(placementId, _listener),
                    bannerPosition.Value,
                    bannerSize,
                    true,
                    true
                );
            }
            else
            {
                banner.Create(
                    placementId,
                    new InternalBannerProyListener(placementId, _listener),
                    bannerSize,
                    x,
                    y,
                    true,
                    true
                );
            }

            _banners[placementId] = banner;
        }

        public void Load(string placementId)
        {
            if (!_banners.ContainsKey(placementId))
            {
                Log($"Can't load before creating banner for placement Id: {placementId}");
                return;
            }

            _banners[placementId].Load(new CustomProperties.Builder().Build());
        }

        public void SetPosition(string placementId, BannerAds.Position position)
        {
            if (!_banners.ContainsKey(placementId))
            {
                Log($"Can't set position before creating banner for placement Id: {placementId}");
                return;
            }

            Log($"Changed position for {placementId}");

            (Banner.Position? bannerPosition, int x, int y) = MapPosition(position);
            if (bannerPosition.HasValue)
            {
                _banners[placementId].SetPosition(bannerPosition.Value);
            }
            else
            {
                _banners[placementId].SetPosition(x, y);
            }
        }

        public void SetAdSpace(string placementId, string adSpace)
        {
            if (!_banners.ContainsKey(placementId))
            {
                Log($"Can't set adSpace before creating banner for placement Id: {placementId}");
                return;
            }

            _banners[placementId].SetAdSpace(adSpace);
        }

        public void Show(string placementId, BannerAds.Position position)
        {
            if (!_banners.ContainsKey(placementId))
            {
                Log($"Can't show before creating banner for placement Id: {placementId}");
                return;
            }

            if (position == BannerAds.Position.Top)
            {
                _banners[placementId].Show(Banner.Position.Top);
            }
            else if (position == BannerAds.Position.Bottom)
            {
                _banners[placementId].Show(Banner.Position.Bottom);
            }
            else
            {
                _banners[placementId].SetPosition(position.X, position.Y);
                _banners[placementId].Show();
            }
        }

        public void Show(string placementId)
        {
            if (!_banners.ContainsKey(placementId))
            {
                Log($"Can't show before creating banner for placement Id: {placementId}");
                return;
            }

            _banners[placementId].Show();
        }

        public void Hide(string placementId)
        {
            if (!_banners.ContainsKey(placementId))
            {
                Log($"Can't hide before creating banner for placement Id: {placementId}");
                return;
            }

            _banners[placementId].Hide();
        }

        private (Banner.Position? bannerPosition, int x, int y) MapPosition(BannerAds.Position position)
        {
            if (position == BannerAds.Position.Top)
            {
                return (Banner.Position.Top, 0, 0);
            }

            if (position == BannerAds.Position.Bottom)
            {
                return (Banner.Position.Bottom, 0, 0);
            }

            return (null, position.X, position.Y);
        }

        private Banner.Size MapSize(BannerAds.Size size)
        {
            if (size == BannerAds.Size.Phone)
            {
                return Banner.Size.Phone;
            }

            if (size == BannerAds.Size.Tablet)
            {
                return Banner.Size.Tablet;
            }

            if (size == BannerAds.Size.Mrec)
            {
                return Banner.Size.MREC;
            }

            return Banner.Size.Phone;
        }

        private void Log(string message)
        {
            Debug.Log($"[XMed] (Banner) {message}");
        }

        private class InternalBannerProyListener : BannerProxyListener
        {
            private readonly string _placementId;
            private readonly BannerAdsProxyListener _listener;

            public InternalBannerProyListener(string placementId, BannerAdsProxyListener listener)
            {
                _placementId = placementId;
                _listener = listener;
            }

            public void OnPrebiddingFinished(PrebiddingResults result)
            {
            }

            public void OnLoaded(LoadResult loadResult)
            {
                _listener.OnLoaded(_placementId, loadResult);
            }

            public void OnFailedToLoad(LoadError loadError, LoadResult loadResult)
            {
            }

            public void OnFailedToShow(ShowError showError)
            {
            }

            public void OnShowed()
            {
            }

            public void OnImpression(ImpressionData impressionData)
            {
                _listener.OnImpression(_placementId, impressionData);
            }

            public void OnClicked()
            {
                _listener.OnClicked(_placementId);
            }

            public void OnDismissed()
            {
            }
        }
    }
}