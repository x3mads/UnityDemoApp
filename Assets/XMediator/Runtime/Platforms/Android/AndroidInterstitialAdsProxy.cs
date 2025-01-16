using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using UnityEngine;
using XMediator.Core.Util;

namespace XMediator.Android
{
    internal class AndroidInterstitialAdsProxy : InterstitialAdsProxy
    {
        private const string PROXY_CLASSNAME =
            "com.x3mads.android.xmediator.unityproxy.fullscreen.InterstitialAdsProxy";

        private const string LISTENER_CLASSNAME =
            "com.x3mads.android.xmediator.unityproxy.fullscreen.UnityFullScreenListener";
        
        private const string ADD_LISTENER_METHOD_NAME = "addListener";
        private const string LOAD_METHOD_NAME = "load";
        private const string IS_READY_METHOD_NAME = "isReady";
        private const string SHOW_METHOD_NAME = "show";
        private const string SHOW_FROM_AD_SPACE_METHOD_NAME = "showFromAdSpace";
        
        private static readonly AndroidJavaClass InterstitialProxy = new AndroidJavaClass(PROXY_CLASSNAME);
        private InterstitialAdsProxyListener proxyListener; // Retain instance to avoid losing callbacks
        private XMediatorMultipleDebouncer<bool> isReadyDebouncer = new XMediatorMultipleDebouncer<bool>();
        private XMediatorDebouncer<bool> isAnyReadyDebouncer = new XMediatorDebouncer<bool>();

        public void SetListener(InterstitialAdsProxyListener listener)
        {
            proxyListener = listener;
            InterstitialProxy.CallStatic(ADD_LISTENER_METHOD_NAME, new XMediatorInterstitialAdsListener(listener));
        }

        public void Load(string placementId)
        {
            InterstitialProxy.CallStatic(LOAD_METHOD_NAME, placementId);
        }

        public bool IsReady(string placementId)
        {
            return isReadyDebouncer.Invoke(placementId,
                () => InterstitialProxy.CallStatic<bool>(IS_READY_METHOD_NAME, placementId)
            );
        }

        public void Show(string placementId)
        {
            InterstitialProxy.CallStatic(SHOW_METHOD_NAME, placementId, AndroidUtils.GetUnityActivity());
        }

        public void ShowFromAdSpace(string adSpace)
        {
            InterstitialProxy.CallStatic(SHOW_FROM_AD_SPACE_METHOD_NAME, AndroidUtils.GetUnityActivity(), adSpace);
        }

        public void ShowFromAdSpace(string placementId, string adSpace)
        {
            InterstitialProxy.CallStatic(SHOW_FROM_AD_SPACE_METHOD_NAME, placementId, AndroidUtils.GetUnityActivity(), adSpace);
        }

        public bool IsReady()
        {
            return isAnyReadyDebouncer.Invoke(
                () => InterstitialProxy.CallStatic<bool>(IS_READY_METHOD_NAME)
            );
        }

        public void Show()
        {
            InterstitialProxy.CallStatic(SHOW_METHOD_NAME, AndroidUtils.GetUnityActivity());
        }
        
        [SuppressMessage("ReSharper", checkId: "UnusedMember.Local")]
        [SuppressMessage("ReSharper", checkId: "InconsistentNaming")]
        private class XMediatorInterstitialAdsListener : AndroidJavaProxy
        {
            private readonly WeakReference<InterstitialAdsProxyListener> _listener;

            [CanBeNull]
            private InterstitialAdsProxyListener safeListener
            {
                get
                {
                    _listener.TryGetTarget(out var listener);
                    return listener;
                }
            }

            internal XMediatorInterstitialAdsListener(InterstitialAdsProxyListener interstitialProxyListener) : base(LISTENER_CLASSNAME)
            {
                _listener = new WeakReference<InterstitialAdsProxyListener>(interstitialProxyListener);
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
        }
    }
}
