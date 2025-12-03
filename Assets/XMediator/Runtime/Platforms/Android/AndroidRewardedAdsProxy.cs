using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using UnityEngine;
using XMediator.Core.Util;

namespace XMediator.Android
{
    internal class AndroidRewardedAdsProxy : RewardedAdsProxy
    {
        private const string PROXY_CLASSNAME =
            "com.x3mads.android.xmediator.unityproxy.fullscreen.RewardedAdsProxy";

        private const string LISTENER_CLASSNAME =
            "com.x3mads.android.xmediator.unityproxy.fullscreen.UnityRewardedAdsListener";
        
        private const string ADD_LISTENER_METHOD_NAME = "addListener";
        private const string LOAD_METHOD_NAME = "load";
        private const string IS_READY_METHOD_NAME = "isReady";
        private const string IS_AD_SPACE_CAPPED_METHOD_NAME = "isAdSpaceCapped";
        private const string SHOW_METHOD_NAME = "show";
        private const string SHOW_FROM_AD_SPACE_METHOD_NAME = "showFromAdSpace";
        
        private static readonly AndroidJavaClass RewardedProxy = new AndroidJavaClass(PROXY_CLASSNAME);
        private RewardedAdsProxyListener proxyListener; // Retain instance to avoid losing callbacks
        private XMediatorMultipleDebouncer<bool> isReadyDebouncer = new XMediatorMultipleDebouncer<bool>();
        private XMediatorDebouncer<bool> isAnyReadyDebouncer = new XMediatorDebouncer<bool>();
        private XMediatorMultipleDebouncer<bool> IsAdSpaceCappedDebouncer = new XMediatorMultipleDebouncer<bool>();

        public void SetListener(RewardedAdsProxyListener listener)
        {
            proxyListener = listener;
            RewardedProxy.CallStatic(ADD_LISTENER_METHOD_NAME, new XMediatorRewardedAdsListener(listener));
        }

        public void Load(string placementId)
        {
            RewardedProxy.CallStatic(LOAD_METHOD_NAME, placementId);
        }

        public bool IsReady(string placementId)
        {
            return isReadyDebouncer.Invoke(placementId, 
                () => RewardedProxy.CallStatic<bool>(IS_READY_METHOD_NAME, placementId)
            );
        }

        public bool IsAdSpaceCapped(string adSpace)
        {
            return IsAdSpaceCappedDebouncer.Invoke(adSpace,
                () => RewardedProxy.CallStatic<bool>(IS_AD_SPACE_CAPPED_METHOD_NAME, adSpace)
            );
        }

        public void Show(string placementId)
        {
            RewardedProxy.CallStatic(SHOW_METHOD_NAME, placementId, AndroidUtils.GetUnityActivity());
        }

        public void ShowFromAdSpace(string adSpace)
        {
            RewardedProxy.CallStatic(SHOW_FROM_AD_SPACE_METHOD_NAME, AndroidUtils.GetUnityActivity(), adSpace);
        }

        public void ShowFromAdSpace(string placementId, string adSpace)
        {
            RewardedProxy.CallStatic(SHOW_FROM_AD_SPACE_METHOD_NAME, placementId, AndroidUtils.GetUnityActivity(), adSpace);
        }

        public bool IsReady()
        {
            return isAnyReadyDebouncer.Invoke(
                () => RewardedProxy.CallStatic<bool>(IS_READY_METHOD_NAME)
            );
        }

        public void Show()
        {
            RewardedProxy.CallStatic(SHOW_METHOD_NAME, AndroidUtils.GetUnityActivity());
        }
        
        [SuppressMessage("ReSharper", checkId: "UnusedMember.Local")]
        [SuppressMessage("ReSharper", checkId: "InconsistentNaming")]
        private class XMediatorRewardedAdsListener : AndroidJavaProxy
        {
            private readonly WeakReference<RewardedAdsProxyListener> _listener;

            [CanBeNull]
            private RewardedAdsProxyListener safeListener
            {
                get
                {
                    _listener.TryGetTarget(out var listener);
                    return listener;
                }
            }

            internal XMediatorRewardedAdsListener(RewardedAdsProxyListener interstitialProxyListener) : base(LISTENER_CLASSNAME)
            {
                _listener = new WeakReference<RewardedAdsProxyListener>(interstitialProxyListener);
            }

            public void onLoaded(string placementId, AndroidJavaObject loadResult)
            {
                safeListener?.OnLoaded(placementId, AndroidProxyMapper.MapLoadResult(loadResult));
            }
            
            public void onFailedToShow(string placementId, AndroidJavaObject showError)
            {
                safeListener?.OnFailedToShow(placementId, AndroidProxyMapper.MapShowError(showError));
            }
            
            public void onShowed(string placementId)
            {
                safeListener?.OnShowed(placementId);
            }
            
            public void onDismissed(string placementId)
            {
                safeListener?.OnDismissed(placementId);
            }
            
            public void onImpression(string placementId, AndroidJavaObject impressionData)
            {
                safeListener?.OnImpression(placementId, AndroidProxyMapper.MapImpressionData(impressionData));
            }
            
            public void onClicked(string placementId)
            {
                safeListener?.OnClicked(placementId);
            }

            public void onEarnedReward(string placementId)
            {
                safeListener?.OnEarnedReward(placementId);
            }
        }
    }
}
