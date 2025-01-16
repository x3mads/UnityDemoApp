using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using UnityEngine;
using XMediator.Api;
using XMediator.Core.Util;

namespace XMediator.Android
{
    internal class AndroidInterstitialProxy : InterstitialProxy
    {
        private const string INTERSTITIAL_PROXY_CLASSNAME =
            "com.etermax.android.xmediator.unityproxy.interstitial.InterstitialProxy";

        private const string INTERSTITIAL_LISTENER_CLASSNAME =
            "com.etermax.android.xmediator.unityproxy.interstitial.UnityInterstitialListener";

        private const string CREATE_METHOD_NAME = "create";
        private const string LOAD_METHOD_NAME = "load";
        private const string SHOW_METHOD_NAME = "show";
        private const string IS_READY_METHOD_NAME = "isReady";
        private const string DESTROY_METHOD_NAME = "destroy";

        private static readonly AndroidJavaClass InterstitialProxy = new AndroidJavaClass(INTERSTITIAL_PROXY_CLASSNAME);

        private string instanceId;
        private InterstitialProxyListener proxyListener;
        private bool _disposed;
        private XMediatorDebouncer<bool> isReadyDebouncer = new XMediatorDebouncer<bool>();

        public void Create(string placementId, InterstitialProxyListener listener, bool test, bool verbose)
        {
            proxyListener = listener;
            instanceId = InterstitialProxy.CallStatic<string>(CREATE_METHOD_NAME,
                AndroidUtils.GetUnityActivity(),
                placementId,
                test,
                verbose,
                new XMediatorInterstitialListener(proxyListener, this)
            );
        }

        public bool IsReady()
        {
            return isReadyDebouncer.Invoke(
                () => InterstitialProxy.CallStatic<bool>(IS_READY_METHOD_NAME, instanceId)
            );
        }

        public void Load(CustomProperties customProperties)
        {
            var customPropertiesAndroidJavaObject = CustomPropertiesDto.From(customProperties).ToAndroidJavaObject();
            using (customPropertiesAndroidJavaObject)
            {
                InterstitialProxy.CallStatic(LOAD_METHOD_NAME,
                    instanceId,
                    customPropertiesAndroidJavaObject
                );
            }
        }

        public void Show()
        {
            InterstitialProxy.CallStatic(SHOW_METHOD_NAME,
                instanceId,
                AndroidUtils.GetUnityActivity()
            );
        }

        public void Show(string adSpace)
        {
            InterstitialProxy.CallStatic(SHOW_METHOD_NAME,
                instanceId,
                AndroidUtils.GetUnityActivity(),
                adSpace
            );
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            isReadyDebouncer.Dispose();
            InterstitialProxy.CallStatic(DESTROY_METHOD_NAME,
                instanceId
            );
        }

        ~AndroidInterstitialProxy()
        {
            AndroidJNI.AttachCurrentThread();
            Dispose();
            AndroidJNI.DetachCurrentThread();
        }

        [SuppressMessage("ReSharper", checkId: "UnusedMember.Local")]
        [SuppressMessage("ReSharper", checkId: "InconsistentNaming")]
        private class XMediatorInterstitialListener : AndroidJavaProxy
        {
            private readonly WeakReference<InterstitialProxyListener> _listener;
            private readonly WeakReference<AndroidInterstitialProxy> _interstitial;

            [CanBeNull]
            private InterstitialProxyListener safeListener
            {
                get
                {
                    _listener.TryGetTarget(out var listener);
                    return listener;
                }
            }

            internal XMediatorInterstitialListener(
                InterstitialProxyListener interstitialProxyListener,
                AndroidInterstitialProxy androidInterstitialProxy
            ) : base(INTERSTITIAL_LISTENER_CLASSNAME)
            {
                _listener = new WeakReference<InterstitialProxyListener>(interstitialProxyListener);
                _interstitial = new WeakReference<AndroidInterstitialProxy>(androidInterstitialProxy);
            }

            public void onPrebiddingFinished(AndroidJavaObject[] result)
            {
                safeListener?.OnPrebiddingFinished(AndroidProxyMapper.MapToPrebiddingResults(result));
            }

            public void onLoaded(AndroidJavaObject loadResult, AndroidJavaObject extras)
            {
                extras.Dispose();
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

            public void onImpression(AndroidJavaObject impressionData)
            {
                safeListener?.OnImpression(AndroidProxyMapper.MapImpressionData(impressionData));
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