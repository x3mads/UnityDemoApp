using System;
using UnityEngine.Assertions;
using XMediator.Api;

namespace XMediator.Unity
{
    internal class UnityRewardedProxy : RewardedProxy
    {
        internal static Action<UnityRewardedProxy, string, RewardedProxyListener, bool, bool> OnCreate = DefaultOnCreate;
        internal static Action<UnityRewardedProxy, CustomProperties> OnLoad = DefaultOnLoad;
        internal static Action<UnityRewardedProxy> OnShow = DefaultOnShow;
        internal static Action<UnityRewardedProxy> OnDispose = DefaultOnDispose;
        internal static Func<UnityRewardedProxy, bool> OnIsReady = DefaultOnIsReady;
        internal RewardedProxyListener RewardedProxyListener { get; private set; }
        
        internal RewardedProxy Mock;
        
        public void Create(string placementId, RewardedProxyListener listener, bool test, bool verbose)
        {
            Assert.IsNotNull(placementId, "Rewarded Create error: placementId is null. Please provide a valid placementId.");
            Assert.IsFalse(placementId == "", "Rewarded Create error: placementId is empty. Please provide a valid placementId.");
            Assert.IsNotNull(listener, "Rewarded Create error: listener is null. Please provide a valid listener.");
            
            RewardedProxyListener = listener;
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

        private static void DefaultOnCreate(UnityRewardedProxy rewardedProxy, string placementId,
            RewardedProxyListener listener,
            bool test, bool verbose)
        {
            rewardedProxy.Mock = new MockRewardedProxy();
            rewardedProxy.Mock.Create(placementId, listener, test, verbose);
        }

        private static void DefaultOnLoad(UnityRewardedProxy rewardedProxy, CustomProperties customProperties)
        {
            rewardedProxy.Mock.Load(customProperties);
        }

        private static void DefaultOnShow(UnityRewardedProxy rewardedProxy)
        {
            rewardedProxy.Mock.Show();
        }

        private static void DefaultOnDispose(UnityRewardedProxy rewardedProxy)
        {
            rewardedProxy.Mock.Dispose();
        }

        private static bool DefaultOnIsReady(UnityRewardedProxy rewardedProxy)
        {
            return rewardedProxy.Mock.IsReady();
        }
    }
}