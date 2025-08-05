using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using UnityEngine;
using XMediator.Api;

namespace XMediator.Android
{
    public class AndroidBannerAdsProxy : BannerAdsProxy
    {
        private const string PROXY_CLASSNAME =
            "com.x3mads.android.xmediator.unityproxy.banner.BannerAdsProxy";

        private const string LISTENER_CLASSNAME =
            "com.x3mads.android.xmediator.unityproxy.banner.UnityBannerAdsListener";
        
        private const string ADD_LISTENER_METHOD_NAME = "addListener";
        private const string CREATE_METHOD_NAME = "create";
        private const string LOAD_METHOD_NAME = "load";
        private const string SET_POSITION_METHOD_NAME = "setPosition";
        private const string SHOW_METHOD_NAME = "show";
        private const string HIDE_METHOD_NAME = "hide";
        private const string SET_AD_SPACE_METHOD_NAME = "setAdSpace";
        private const string IS_READY_METHOD_NAME = "isReady";

        private static readonly AndroidJavaClass BannerProxy = new AndroidJavaClass(PROXY_CLASSNAME);
        private BannerAdsProxyListener proxyListener;  // Retain instance to avoid losing callbacks

        public void SetListener(BannerAdsProxyListener listener)
        {
            proxyListener = listener;
            BannerProxy.CallStatic(ADD_LISTENER_METHOD_NAME, new XMediatorBannerAdsListener(listener));
        }

        public void Create(string placementId, BannerAds.Size size, BannerAds.Position position)
        {
            if (position.IsCustom())
            {
                BannerProxy.CallStatic(CREATE_METHOD_NAME, AndroidUtils.GetUnityActivity(), placementId, size.Identifier, position.X, position.Y);
            }
            else
            {
                BannerProxy.CallStatic(CREATE_METHOD_NAME, AndroidUtils.GetUnityActivity(), placementId, size.Identifier, position.Identifier);
            }
        }

        public void Load(string placementId)
        {
            BannerProxy.CallStatic(LOAD_METHOD_NAME, placementId);
        }

        public bool IsReady(string placementId)
        {
            return BannerProxy.CallStatic<bool>(IS_READY_METHOD_NAME, placementId);
        }

        public void SetPosition(string placementId, BannerAds.Position position)
        {
            if (position.IsCustom())
            {
                BannerProxy.CallStatic(SET_POSITION_METHOD_NAME, placementId, position.X, position.Y);
            }
            else
            {
                BannerProxy.CallStatic(SET_POSITION_METHOD_NAME, placementId, position.Identifier);
            }
        }

        public void SetAdSpace(string placementId, string adSpace)
        {
            BannerProxy.CallStatic(SET_AD_SPACE_METHOD_NAME, placementId, adSpace);
        }

        public void Show(string placementId, BannerAds.Position position)
        {
            if (position.IsCustom())
            {
                BannerProxy.CallStatic(SHOW_METHOD_NAME, placementId, position.X, position.Y);
            }
            else
            {
                BannerProxy.CallStatic(SHOW_METHOD_NAME, placementId, position.Identifier);
            }
        }

        public void Show(string placementId)
        {
            BannerProxy.CallStatic(SHOW_METHOD_NAME, placementId);
        }

        public void Hide(string placementId)
        {
            BannerProxy.CallStatic(HIDE_METHOD_NAME, placementId);
        }
        
        [SuppressMessage("ReSharper", checkId: "UnusedMember.Local")]
        [SuppressMessage("ReSharper", checkId: "InconsistentNaming")]
        private class XMediatorBannerAdsListener : AndroidJavaProxy
        {
            private readonly WeakReference<BannerAdsProxyListener> _listener;

            [CanBeNull]
            private BannerAdsProxyListener safeListener
            {
                get
                {
                    _listener.TryGetTarget(out var listener);
                    return listener;
                }
            }

            internal XMediatorBannerAdsListener(BannerAdsProxyListener bannerAdsProxyListener) : base(LISTENER_CLASSNAME)
            {
                _listener = new WeakReference<BannerAdsProxyListener>(bannerAdsProxyListener);
            }

            public void onLoaded(string placementId, AndroidJavaObject loadResult)
            {
                safeListener?.OnLoaded(placementId, AndroidProxyMapper.MapLoadResult(loadResult));
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
