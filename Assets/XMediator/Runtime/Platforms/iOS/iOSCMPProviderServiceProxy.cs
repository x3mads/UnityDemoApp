using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

namespace XMediator.iOS
{
    internal class iOSCMPProviderServiceProxy : CMPProviderServiceProxy
    {
        private static readonly Dictionary<string, Action<Exception>> Callbacks = new Dictionary<string, Action<Exception>>();

        private delegate void NativeShowPrivacyFormCallback(string identifier, string result);

        public bool IsPrivacyFormAvailable() => X3MIsPrivacyFormAvailable();

        public void ShowPrivacyForm(Action<Exception> onComplete)
        {
            var identifier = Guid.NewGuid().ToString();
            Callbacks[identifier] = onComplete;
            X3MShowPrivacyForm(identifier, ShowPrivacyFormCallbackMethod);
        }

        public void Reset()
        {
            X3MResetCMP();
        }

        [DllImport("__Internal")]
        private static extern bool X3MIsPrivacyFormAvailable();

        [DllImport("__Internal")]
        private static extern void X3MShowPrivacyForm(string identifier, NativeShowPrivacyFormCallback callback);

        [DllImport("__Internal")]
        private static extern void X3MResetCMP();

        [MonoPInvokeCallback(typeof(NativeShowPrivacyFormCallback))]
        private static void ShowPrivacyFormCallbackMethod(string identifier, string result)
        {
            if (Callbacks.TryGetValue(identifier, out var callback))
            {
                var initResult = JsonUtility.FromJson<ShowPrivacyFormCallbackDto>(result).ToException();
                callback.Invoke(initResult);
            }

            Callbacks.Remove(identifier);
        }
    }
}