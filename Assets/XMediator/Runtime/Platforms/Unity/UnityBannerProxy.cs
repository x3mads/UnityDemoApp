using System;
using UnityEngine.Assertions;
using XMediator.Api;

namespace XMediator.Unity
{
    internal class UnityBannerProxy : BannerProxy
    {
        internal static Action<UnityBannerProxy, string, BannerProxyListener, Banner.Position, Banner.Size, bool, bool> OnCreate =
            DefaultOnCreate;
        internal static Action<UnityBannerProxy, string, BannerProxyListener, int, int, Banner.Size, bool, bool> OnCreatePositioned =
            DefaultOnCreatePositioned;
        internal static Action<UnityBannerProxy, CustomProperties> OnLoad = DefaultOnLoad;
        internal static Action<UnityBannerProxy, Banner.Position> OnShow = DefaultOnShow;
        internal static Action<UnityBannerProxy> OnShowNoArguments = DefaultOnShowNoArguments;
        internal static Action<UnityBannerProxy, int, int> OnSetPosition = DefaultOnSetPosition;
        internal static Action<UnityBannerProxy> OnHide = DefaultOnHide;
        internal static Action<UnityBannerProxy> OnDispose = DefaultOnDispose;
        internal BannerProxyListener BannerProxyListener { get; private set; }
        
        private MockBannerProxy _mock;
        
        public void Create(string placementId, BannerProxyListener listener, Banner.Position position, Banner.Size size,
            bool test,
            bool verbose)
        {
            Assert.IsNotNull(placementId, "Banner Create error: placementId is null. Please provide a valid placementId.");
            Assert.IsFalse(placementId == "", "Banner Create error: placementId is empty. Please provide a valid placementId.");
            Assert.IsNotNull(listener, "Banner Create error: listener is null. Please provide a valid listener.");
            
            BannerProxyListener = listener;
            OnCreate(this, placementId, listener, position, size, test, verbose);
        }

        public void Create(string placementId, BannerProxyListener listener, Banner.Size size, int x, int y, bool test, bool verbose)
        {
            Assert.IsNotNull(placementId, "Banner Create error: placementId is null. Please provide a valid placementId.");
            Assert.IsFalse(placementId == "", "Banner Create error: placementId is empty. Please provide a valid placementId.");
            Assert.IsNotNull(listener, "Banner Create error: listener is null. Please provide a valid listener.");
            
            BannerProxyListener = listener;
            OnCreatePositioned(this, placementId, listener, x, y, size, test, verbose);
        }

        public void Load(CustomProperties customProperties)
        {
            OnLoad(this, customProperties);
        }

        public void SetPosition(int x, int y)
        {
            OnSetPosition(this, x, y);
        }

        public void SetAdSpace(string adSpace)
        {
            _mock.SetAdSpace(adSpace);
        }

        public void Show()
        {
            OnShowNoArguments(this);
        }

        public void Show(Banner.Position position)
        {
            OnShow(this, position);
        }

        public void Hide()
        {
            OnHide(this);
        }

        public void Dispose()
        {
            OnDispose(this);
        }

        private static void DefaultOnCreate(UnityBannerProxy bannerProxy, string placementId, BannerProxyListener listener,
            Banner.Position position, Banner.Size size, bool test, bool verbose)
        {
            bannerProxy._mock = new MockBannerProxy();
            bannerProxy._mock.Create(placementId, listener, position, size, test, verbose);
        }
        
        private static void DefaultOnCreatePositioned(UnityBannerProxy bannerProxy, string placementId, BannerProxyListener listener,
            int x, int y, Banner.Size size, bool test, bool verbose)
        {
            bannerProxy._mock = new MockBannerProxy();
            bannerProxy._mock.Create(placementId, listener, size, x, y, test, verbose);
        }

        private static void DefaultOnLoad(UnityBannerProxy bannerProxy, CustomProperties customProperties)
        {
            bannerProxy._mock.Load(customProperties);
        }

        private static void DefaultOnShow(UnityBannerProxy bannerProxy, Banner.Position position)
        {
            bannerProxy._mock.Show(position);
        }
        
        private static void DefaultOnShowNoArguments(UnityBannerProxy bannerProxy)
        {
            bannerProxy._mock.Show();
        }

        private static void DefaultOnHide(UnityBannerProxy bannerProxy)
        {
            bannerProxy._mock.Hide();
        }

        private static void DefaultOnSetPosition(UnityBannerProxy bannerProxy, int x, int y)
        {
            bannerProxy._mock.SetPosition(x, y);
        }

        private static void DefaultOnDispose(UnityBannerProxy bannerProxy)
        {
            bannerProxy._mock.Dispose();
        }
    }
}