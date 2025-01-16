using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;

namespace XMediator.iOS
{
    internal class iOSCallbackServiceProxy
    {
        private delegate void Callback(string identifier, string eventName, string resultJsonString);

        private static readonly Dictionary<string, WeakReference<iOSNativeEventCallback>> callbacks = new Dictionary<string, WeakReference<iOSNativeEventCallback>>();

        private iOSCallbackServiceProxy()
        {
        }

        internal static void AddCallback(string identifier, iOSNativeEventCallback eventCallback)
        {
            setCallback(identifier, ExecuteCallback);
            var weakReference = new WeakReference<iOSNativeEventCallback>(eventCallback);
            callbacks.Add(identifier, weakReference);
        }

        internal static void RemoveCallback(string identifier)
        {
            callbacks.Remove(identifier);
        }

        [DllImport("__Internal")]
        private static extern void setCallback(string identifier, Callback callback);

        [MonoPInvokeCallback(typeof(Callback))]
        private static void ExecuteCallback(string identifier, string eventName, string resultJsonString)
        {
            if (!callbacks.TryGetValue(identifier, out var weakCallback)) return;
            if (!weakCallback.TryGetTarget(out var callback)) return;
            
            Enum.TryParse(eventName, out IOSNativeEvent nativeAdEvent);
            callback.OnIOSNativeEvent(nativeAdEvent, resultJsonString);
        }
    }

    internal enum IOSNativeEvent
    {
        DidCompletePrebidding,
        DidLoad,
        FailedToLoad,
        DidPresent,
        FailedToPresent,
        DidRecordImpression,
        WillDismiss,
        DidDismiss,
        DidClick,
        DidEarnReward
    }
}