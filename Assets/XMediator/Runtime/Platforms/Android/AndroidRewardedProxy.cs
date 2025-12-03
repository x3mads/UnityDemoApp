using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using UnityEngine;
using XMediator.Api;
using XMediator.Core.Util;

namespace XMediator.Android
{
    internal class AndroidRewardedProxy : RewardedProxy
    {
        private const string REWARDED_PROXY_CLASSNAME = "com.etermax.android.xmediator.unityproxy.rewarded.RewardedProxy";

        private const string REWARDED_LISTENER_CLASSNAME =
            "com.etermax.android.xmediator.unityproxy.rewarded.UnityRewardedListener";

        private const string CREATE_METHOD_NAME = "create";
        private const string LOAD_METHOD_NAME = "load";
        private const string SHOW_METHOD_NAME = "show";
        private const string IS_READY_METHOD_NAME = "isReady";
        private const string IS_AD_SPACE_CAPPED_METHOD_NAME = "isAdSpaceCapped";
        private const string DESTROY_METHOD_NAME = "destroy";

        private static readonly AndroidJavaClass RewardedProxy = new AndroidJavaClass(REWARDED_PROXY_CLASSNAME);

        private string instanceId;
        private RewardedProxyListener _proxyListener;
        private bool _disposed;
        private XMediatorDebouncer<bool> isReadyDebouncer = new XMediatorDebouncer<bool>();
        private XMediatorDebouncer<bool> isAdSpaceCappedDebouncer = new XMediatorDebouncer<bool>();

        public void Create(string placementId, RewardedProxyListener listener, bool test, bool verbose)
        {
            _proxyListener = listener;
            instanceId = RewardedProxy.CallStatic<string>(CREATE_METHOD_NAME,
                AndroidUtils.GetUnityActivity(),
                placementId,
                test,
                verbose,
                new XMediatorRewardedListener(_proxyListener, this)
            );
        }

        public void Load(CustomProperties customProperties)
        {
            var customPropertiesAndroidJavaObject = CustomPropertiesDto.From(customProperties).ToAndroidJavaObject();
            using (customPropertiesAndroidJavaObject)
            {
                RewardedProxy.CallStatic(
                    LOAD_METHOD_NAME,
                    instanceId,
                    customPropertiesAndroidJavaObject
                );
            }
        }

        public bool IsReady()
        {
            return isReadyDebouncer.Invoke(
                () => RewardedProxy.CallStatic<bool>(IS_READY_METHOD_NAME, instanceId)
            );
        }

        public bool IsAdSpaceCapped(string adSpace)
        {
            return isAdSpaceCappedDebouncer.Invoke(
                () => RewardedProxy.CallStatic<bool>(IS_AD_SPACE_CAPPED_METHOD_NAME, instanceId, adSpace)
            );
        }

        public void Show()
        {
            RewardedProxy.CallStatic(SHOW_METHOD_NAME,
                instanceId,
                AndroidUtils.GetUnityActivity()
            );
        }

        public void Show(string adSpace)
        {
            RewardedProxy.CallStatic(SHOW_METHOD_NAME,
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
            RewardedProxy.CallStatic(DESTROY_METHOD_NAME,
                instanceId
            );
        }

        ~AndroidRewardedProxy()
        {
            AndroidJNI.AttachCurrentThread();
            Dispose();
            AndroidJNI.DetachCurrentThread();
        }

        [SuppressMessage("ReSharper", checkId: "UnusedMember.Local")]
        [SuppressMessage("ReSharper", checkId: "InconsistentNaming")]
        private class XMediatorRewardedListener : AndroidJavaProxy
        {
            private readonly WeakReference<RewardedProxyListener> _listener;
            private readonly WeakReference<AndroidRewardedProxy> _rewarded;

            [CanBeNull]
            private RewardedProxyListener safeListener
            {
                get
                {
                    _listener.TryGetTarget(out var listener);
                    return listener;
                }
            }

            internal XMediatorRewardedListener(
                RewardedProxyListener listener,
                AndroidRewardedProxy androidRewardedProxy
            ) : base(REWARDED_LISTENER_CLASSNAME)
            {
                _listener = new WeakReference<RewardedProxyListener>(listener);
                _rewarded = new WeakReference<AndroidRewardedProxy>(androidRewardedProxy);
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

            public void onEarnedReward()
            {
                safeListener?.OnEarnedReward();
            }
        }
    }
}