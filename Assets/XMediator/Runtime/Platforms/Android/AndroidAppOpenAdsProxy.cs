using System;
using UnityEngine;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using UnityEngine;
using XMediator.Core.Util;

namespace XMediator.Android
{
    internal class AndroidAppOpenAdsProxy : AppOpenAdsProxy
    {
        private const string PROXY_CLASSNAME =
            "com.x3mads.android.xmediator.unityproxy.fullscreen.AppOpenAdsProxy";

        private const string LISTENER_CLASSNAME =
            "com.x3mads.android.xmediator.unityproxy.fullscreen.UnityFullScreenListener";
        
        private const string ADD_LISTENER_METHOD_NAME = "addListener";
        private const string LOAD_METHOD_NAME = "load";
        private const string IS_READY_METHOD_NAME = "isReady";
        private const string IS_AD_SPACE_CAPPED_METHOD_NAME = "isAdSpaceCapped";
        private const string SHOW_METHOD_NAME = "show";
        private const string SHOW_FROM_AD_SPACE_METHOD_NAME = "showFromAdSpace";

        private static readonly AndroidJavaClass AppOpenAdsProxy = new AndroidJavaClass(PROXY_CLASSNAME);
        private AppOpenAdsProxyListener proxyListener; // Retain instance to avoid losing callbacks
        private XMediatorMultipleDebouncer<bool> isReadyDebouncer = new XMediatorMultipleDebouncer<bool>();
        private XMediatorDebouncer<bool> isAnyReadyDebouncer = new XMediatorDebouncer<bool>();
        private XMediatorMultipleDebouncer<bool> IsAdSpaceCappedDebouncer = new XMediatorMultipleDebouncer<bool>();

        public void SetListener(AppOpenAdsProxyListener listener)
        {
            proxyListener = listener;
            AppOpenAdsProxy.CallStatic(ADD_LISTENER_METHOD_NAME, new SafeAppOpenAdsProxyListener(listener));
        }

        public void Load(string placementId)
        {
            AppOpenAdsProxy.CallStatic(LOAD_METHOD_NAME, placementId);
        }

        public bool IsReady()
        {
            return isAnyReadyDebouncer.Invoke(
                () => AppOpenAdsProxy.CallStatic<bool>(IS_READY_METHOD_NAME)
            );
        }

        public bool IsReady(string placementId)
        {
            return isReadyDebouncer.Invoke(placementId,
                () => AppOpenAdsProxy.CallStatic<bool>(IS_READY_METHOD_NAME, placementId)
            );
        }

        public bool IsAdSpaceCapped(string adSpace)
        {
            return IsAdSpaceCappedDebouncer.Invoke(adSpace,
                () => AppOpenAdsProxy.CallStatic<bool>(IS_AD_SPACE_CAPPED_METHOD_NAME, adSpace)
            );
        }

        public void Show()
        {
            AppOpenAdsProxy.CallStatic(SHOW_METHOD_NAME, AndroidUtils.GetUnityActivity());
        }

        public void Show(string placementId)
        {
            AppOpenAdsProxy.CallStatic(SHOW_METHOD_NAME, placementId, AndroidUtils.GetUnityActivity());
        }

        public void ShowFromAdSpace(string adSpace)
        {
            AppOpenAdsProxy.CallStatic(SHOW_FROM_AD_SPACE_METHOD_NAME, AndroidUtils.GetUnityActivity(), adSpace);
        }

        public void ShowFromAdSpace(string placementId, string adSpace)
        {
            AppOpenAdsProxy.CallStatic(SHOW_FROM_AD_SPACE_METHOD_NAME, placementId, AndroidUtils.GetUnityActivity(), adSpace);
        }

        [SuppressMessage("ReSharper", checkId: "UnusedMember.Local")]
        [SuppressMessage("ReSharper", checkId: "InconsistentNaming")]
        private class SafeAppOpenAdsProxyListener : AndroidJavaProxy
        {
            private readonly WeakReference<AppOpenAdsProxyListener> _listener;

            [CanBeNull]
            private AppOpenAdsProxyListener safeListener
            {
                get
                {
                    _listener.TryGetTarget(out var listener);
                    return listener;
                }
            }

            internal SafeAppOpenAdsProxyListener(AppOpenAdsProxyListener appOpenAdsProxyListener) : base(LISTENER_CLASSNAME)
            {
                _listener = new WeakReference<AppOpenAdsProxyListener>(appOpenAdsProxyListener);
            }
            
            public void onLoaded(string placementId, AndroidJavaObject loadResult)
            {
                safeListener?.OnLoaded(placementId, AndroidProxyMapper.MapLoadResult(loadResult));
            }

            public void onShowed(string placementId)
            {
                safeListener?.OnShowed(placementId);
            }

            public void onFailedToShow(string placementId, AndroidJavaObject showError)
            {
                safeListener?.OnFailedToShow(placementId, AndroidProxyMapper.MapShowError(showError));
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
