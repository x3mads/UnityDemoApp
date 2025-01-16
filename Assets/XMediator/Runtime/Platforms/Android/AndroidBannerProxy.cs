using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using UnityEngine;
using XMediator.Api;

namespace XMediator.Android
{
    internal class AndroidBannerProxy : BannerProxy
    {
        private const string BANNER_PROXY_CLASSNAME = "com.etermax.android.xmediator.unityproxy.banner.BannerProxy";
        private const string BANNER_LISTENER_CLASSNAME = "com.etermax.android.xmediator.unityproxy.banner.UnityBannerListener";
        private const string CREATE_METHOD_NAME = "create";
        private const string LOAD_METHOD_NAME = "load";
        private const string SET_POSITION_METHOD_NAME = "setPosition";
        private const string SET_AD_SPACE_METHOD_NAME = "setAdSpace";
        private const string SHOW_METHOD_NAME = "show";
        private const string HIDE_METHOD_NAME = "hide";
        private const string DESTROY_METHOD_NAME = "destroy";

        private const int BANNER_SIZE_PHONE = 0;
        private const int BANNER_SIZE_TABLET = 1;
        private const int BANNER_SIZE_MREC = 2;
        private const int BANNER_POSITION_TOP = 0;
        private const int BANNER_POSITION_BOTTOM = 1;

        private static readonly AndroidJavaClass BannerProxy = new AndroidJavaClass(BANNER_PROXY_CLASSNAME);

        private string instanceId;
        private BannerProxyListener proxyListener;
        private bool _disposed;

        public void Create(string placementId, BannerProxyListener listener, Banner.Position position, Banner.Size size,
            bool test,
            bool verbose)
        {
            proxyListener = listener;
            instanceId = BannerProxy.CallStatic<string>(CREATE_METHOD_NAME,
                AndroidUtils.GetUnityActivity(),
                placementId,
                MapSizeToInt(size),
                ToInteger(position),
                test,
                verbose,
                new XMediatorBannerListener(proxyListener)
            );
        }

        public void Create(string placementId, BannerProxyListener listener, Banner.Size size, int x, int y, bool test, bool verbose)
        {
            proxyListener = listener;
            instanceId = BannerProxy.CallStatic<string>(CREATE_METHOD_NAME,
                AndroidUtils.GetUnityActivity(),
                placementId,
                MapSizeToInt(size),
                x,
                y,
                test,
                verbose,
                new XMediatorBannerListener(proxyListener)
            );
        }

        public void Load(CustomProperties customProperties)
        {
            var customPropertiesAndroidJavaObject = CustomPropertiesDto.From(customProperties).ToAndroidJavaObject();
            using (customPropertiesAndroidJavaObject)
            {
                BannerProxy.CallStatic(LOAD_METHOD_NAME,
                    instanceId,
                    customPropertiesAndroidJavaObject
                );
            }
        }

        public void SetPosition(int x, int y)
        {
            BannerProxy.CallStatic(
                SET_POSITION_METHOD_NAME,
                instanceId,
                x,
                y
            );
        }

        public void SetAdSpace(string adSpace)
        {
            BannerProxy.CallStatic(SET_AD_SPACE_METHOD_NAME, instanceId, adSpace);
        }

        public void Show()
        {
            BannerProxy.CallStatic(
                SHOW_METHOD_NAME,
                instanceId
            );
        }

        public void Show(Banner.Position position)
        {
            BannerProxy.CallStatic(SHOW_METHOD_NAME,
                instanceId,
                ToInteger(position)
            );
        }

        public void Hide()
        {
            BannerProxy.CallStatic(HIDE_METHOD_NAME,
                instanceId
            );
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            BannerProxy.CallStatic(DESTROY_METHOD_NAME,
                instanceId
            );
        }

        ~AndroidBannerProxy()
        {
            AndroidJNI.AttachCurrentThread();
            Dispose();
            AndroidJNI.DetachCurrentThread();
        }

        private static int ToInteger(Banner.Position position)
        {
            return position.Equals(Banner.Position.Top) ? BANNER_POSITION_TOP : BANNER_POSITION_BOTTOM;
        }

        private static int MapSizeToInt(Banner.Size size)
        {
            switch (size)
            {
                case Banner.Size.Phone:
                    return BANNER_SIZE_PHONE;
                case Banner.Size.Tablet:
                    return BANNER_SIZE_TABLET;
                case Banner.Size.MREC:
                    return BANNER_SIZE_MREC;
                default:
                    return BANNER_SIZE_PHONE;
            }
        }

        [SuppressMessage("ReSharper", checkId: "UnusedMember.Local")]
        [SuppressMessage("ReSharper", checkId: "InconsistentNaming")]
        private class XMediatorBannerListener : AndroidJavaProxy
        {
            private readonly WeakReference<BannerProxyListener> _listener;

            [CanBeNull]
            private BannerProxyListener safeListener
            {
                get
                {
                    _listener.TryGetTarget(out var listener);
                    return listener;
                }
            }

            internal XMediatorBannerListener(BannerProxyListener bannerProxyListener) : base(BANNER_LISTENER_CLASSNAME)
            {
                _listener = new WeakReference<BannerProxyListener>(bannerProxyListener);
            }

            public void onNetworkImpression(AndroidJavaObject[] instances, AndroidJavaObject extras)
            {
                foreach (var androidJavaObject in instances)
                {
                    androidJavaObject.Dispose();
                }

                extras.Dispose();
                // Not implemented
            }

            public void onImpression(AndroidJavaObject impressionData)
            {
                safeListener?.OnImpression(AndroidProxyMapper.MapImpressionData(impressionData));
            }

            public void onPrebiddingFinished(AndroidJavaObject[] result)
            {
                safeListener?.OnPrebiddingFinished(AndroidProxyMapper.MapToPrebiddingResults(result));
            }

            public void onLoaded(AndroidJavaObject loadResult, AndroidJavaObject extras)
            {
                safeListener?.OnLoaded(AndroidProxyMapper.MapLoadResult(loadResult));
            }

            public void onFailedToLoad(AndroidJavaObject loadError, [CanBeNull] AndroidJavaObject loadResult)
            {
                safeListener?.OnFailedToLoad(AndroidProxyMapper.MapLoadError(loadError),
                    AndroidProxyMapper.MapLoadResult(loadResult));
            }

            public void onFailedToShow(AndroidJavaObject showError)
            {
                safeListener?.OnFailedToShow(AndroidProxyMapper.MapShowError(showError));
            }

            public void onShowed()
            {
                safeListener?.OnShowed();
            }

            public void onClicked()
            {
                safeListener?.OnClicked();
            }

            public void onDismissed()
            {
                safeListener?.OnDismissed();
            }
        }
    }
}