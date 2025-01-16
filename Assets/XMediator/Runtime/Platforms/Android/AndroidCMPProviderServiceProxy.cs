using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace XMediator.Android
{
    internal class AndroidCMPProviderServiceProxy : CMPProviderServiceProxy
    {
        private const string XMEDIATOR_CMP_PROVIDER_PROXY_CLASSNAME =
            "com.etermax.android.xmediator.unityproxy.cmp.CMPProviderProxy";

        private const string UNITY_CMP_PROVIDER_SHOW_CALLBACK_CLASSNAME =
            "com.etermax.android.xmediator.unityproxy.cmp.UnityShowPrivacyFormCallback";

        private const string IS_PRIVACY_FORM_AVAILABLE_METHOD_NAME = "isPrivacyFormAvailable";
        private const string SHOW_PRIVACY_FORM_METHOD_NAME = "showPrivacyForm";
        private const string RESET_METHOD_NAME = "reset";

        private AndroidJavaClass _xMediatorCMPProviderProxy = new AndroidJavaClass(XMEDIATOR_CMP_PROVIDER_PROXY_CLASSNAME);

        public bool IsPrivacyFormAvailable()
        {
            return _xMediatorCMPProviderProxy.CallStatic<bool>(
                IS_PRIVACY_FORM_AVAILABLE_METHOD_NAME,
                AndroidUtils.GetUnityActivity()
            );
        }

        public void ShowPrivacyForm(Action<Exception> onComplete)
        {
            _xMediatorCMPProviderProxy.CallStatic(
                SHOW_PRIVACY_FORM_METHOD_NAME,
                AndroidUtils.GetUnityActivity(),
                new XMediatorUnityShowPrivacyFormCallback(onComplete)
            );
        }

        public void Reset()
        {
            _xMediatorCMPProviderProxy.CallStatic(
                RESET_METHOD_NAME,
                AndroidUtils.GetUnityActivity()
            );
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private class XMediatorUnityShowPrivacyFormCallback : AndroidJavaProxy
        {
            private readonly Action<Exception> _action;

            internal XMediatorUnityShowPrivacyFormCallback(Action<Exception> action) : base(UNITY_CMP_PROVIDER_SHOW_CALLBACK_CLASSNAME)
            {
                _action = action;
            }

            public AndroidJavaObject onComplete(AndroidJavaObject reason)
            {
                if (reason == null)
                {
                    _action.Invoke(null);
                    return null;
                }
                _action.Invoke(new Exception(reason.Call<string>("toString")));
                return null;
            }
        }
    }
}
