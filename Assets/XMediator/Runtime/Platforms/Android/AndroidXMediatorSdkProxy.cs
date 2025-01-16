using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using XMediator.Api;

namespace XMediator.Android
{
    internal class AndroidXMediatorSdkProxy : XMediatorSdkProxy
    {
        private const string XMEDIATOR_SDK_PROXY_CLASSNAME =
            "com.etermax.android.xmediator.unityproxy.initialize.XMediatorSdkProxy";

        private const string UNITY_MEDIATION_CALLBACK_CLASSNAME =
            "com.etermax.android.xmediator.unityproxy.initialize.UnityMediationCallback";

        private const string INITIALIZE_METHOD_NAME = "initialize";
        private const string SET_USER_PROPERTIES_METHOD_NAME = "setUserProperties";
        private const string SET_CONSENT_INFORMATION_METHOD_NAME = "setConsentInformation";
        private const string OPEN_DEBUGGER_SUITE_METHOD_NAME = "openDebuggerSuite";

        private AndroidJavaClass _xMediatorSdkProxy = new AndroidJavaClass(XMEDIATOR_SDK_PROXY_CLASSNAME);

        public void Initialize(
            string appKey,
            string unityVersion,
            InitSettings initSettings,
            Action<InitResult> initCallback,
            Action<MediationResult> mediationCallback
        )
        {
            var androidJavaObject = initSettings.ToInitSettingsDto().ToAndroidJavaObject();
            using (androidJavaObject)
            {
                _xMediatorSdkProxy.CallStatic(
                    INITIALIZE_METHOD_NAME,
                    AndroidUtils.GetUnityActivity(),
                    unityVersion,
                    appKey,
                    androidJavaObject,
                    new XMediatorUnityInitCallback(initCallback),
                    new XMediatorUnityMediationCallback(mediationCallback)
                );
            }
        }

        public void SetUserProperties(UserProperties userProperties)
        {
            var androidJavaObject = UserPropertiesDto.From(userProperties).ToAndroidJavaObject();
            using (androidJavaObject)
            {
                _xMediatorSdkProxy.CallStatic
                (
                    SET_USER_PROPERTIES_METHOD_NAME,
                    androidJavaObject
                );
            }
        }

        public void SetConsentInformation(ConsentInformation consentInformation)
        {
            var androidJavaObject = ConsentInformationDto.From(consentInformation).ToAndroidJavaObject();
            using (androidJavaObject)
            {
                _xMediatorSdkProxy.CallStatic(
                    SET_CONSENT_INFORMATION_METHOD_NAME,
                    androidJavaObject
                );
            }
        }

        public void SetPauseOnAdPresentation(bool shouldPause)
        {
            // Do nothing. Android pauses the app by default on ad presentation.
        }

        public void OpenDebuggerSuite()
        {
            _xMediatorSdkProxy.CallStatic(
                OPEN_DEBUGGER_SUITE_METHOD_NAME,
                AndroidUtils.GetUnityActivity()
            );
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private class XMediatorUnityMediationCallback : AndroidJavaProxy
        {
            private readonly Action<MediationResult> _action;

            internal XMediatorUnityMediationCallback(Action<MediationResult> action) : base(UNITY_MEDIATION_CALLBACK_CLASSNAME)
            {
                _action = action;
            }

            public AndroidJavaObject onMediationInitCompleted(AndroidJavaObject mediationResult)
            {
                _action.Invoke(AndroidProxyMapper.ParseOnMediationInitCompleted(mediationResult));
                return null;
            }
        }
    }
}