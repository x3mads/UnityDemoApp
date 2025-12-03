using System;
using UnityEngine.Assertions;
using XMediator.Api;

namespace XMediator.Unity
{
    internal class UnityInterstitialProxy : InterstitialProxy
    {
        internal static Action<UnityInterstitialProxy, string, InterstitialProxyListener, bool, bool> OnCreate = DefaultOnCreate;
        internal static Action<UnityInterstitialProxy, CustomProperties> OnLoad = DefaultOnLoad;
        internal static Action<UnityInterstitialProxy> OnShow = DefaultOnShow;
        internal static Action<UnityInterstitialProxy> OnDispose = DefaultOnDispose;
        internal static Func<UnityInterstitialProxy, bool> OnIsReady = DefaultOnIsReady;
        internal static Func<UnityInterstitialProxy, string, bool> OnIsAdSpaceCapped = DefaultOnIsAdSpaceCapped;
        internal InterstitialProxyListener InterstitialProxyListener;
        internal InterstitialProxy Mock;

        public void Create(string placementId, InterstitialProxyListener listener, bool test, bool verbose)
        {
            Assert.IsNotNull(placementId, "Interstitial Create error: placementId is null. Please provide a valid placementId.");
            Assert.IsFalse(placementId == "", "Interstitial Create error: placementId is empty. Please provide a valid placementId.");
            Assert.IsNotNull(listener, "Interstitial Create error: listener is null. Please provide a valid listener.");
            
            InterstitialProxyListener = listener;
            OnCreate(this, placementId, listener, test, verbose);
        }

        public void Load(CustomProperties customProperties)
        {
            OnLoad(this, customProperties);
        }

        public bool IsReady()
        {
            return OnIsReady(this);
        }

        public bool IsAdSpaceCapped(string adSpace)
        {
            return OnIsAdSpaceCapped(this, adSpace);
        }

        public void Show()
        {
            OnShow(this);
        }

        public void Show(string adSpace)
        {
            Mock.Show(adSpace);
        }

        public void Dispose()
        {
            OnDispose(this);
        }

        private static void DefaultOnCreate(UnityInterstitialProxy interstitialProxy, string placementId,
            InterstitialProxyListener listener, bool test, bool verbose)
        {
            interstitialProxy.Mock = new MockInterstitialProxy();
            interstitialProxy.Mock.Create(placementId, listener, test, verbose);
        }

        private static void DefaultOnLoad(UnityInterstitialProxy interstitialProxy, CustomProperties customProperties)
        {
            interstitialProxy.Mock.Load(customProperties);
        }

        private static void DefaultOnShow(UnityInterstitialProxy interstitialProxy)
        {
            interstitialProxy.Mock.Show();
        }

        private static void DefaultOnDispose(UnityInterstitialProxy interstitialProxy)
        {
            interstitialProxy.Mock.Dispose();
        }

        private static bool DefaultOnIsReady(UnityInterstitialProxy interstitialProxy)
        {
            return interstitialProxy.Mock.IsReady();
        }

        private static bool DefaultOnIsAdSpaceCapped(UnityInterstitialProxy interstitialProxy, string adSpace)
        {
            return interstitialProxy.Mock.IsAdSpaceCapped(adSpace);
        }
    }
}