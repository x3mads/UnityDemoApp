using System;
using UnityEngine;
using XMediator.Api;

namespace XMediator.Android
{
    internal class AndroidXMediatorAdsProxy : XMediatorAdsProxy
    {
        private const string XMEDIATOR_ADS_PROXY_CLASSNAME = "com.x3mads.android.xmediator.unityproxy.XMediatorAdsProxy";

        private const string INITIALIZE_METHOD_NAME = "startWith";
        private const string SET_CONSENT_METHOD_NAME = "setConsentInformation";
        private const string GET_USER_PROPERTIES_METHOD_NAME = "getUserProperties";
        private const string SET_USER_PROPERTIES_METHOD_NAME = "setUserProperties";
        private const string OPEN_DEBUGGER_SUITE_METHOD_NAME = "openDebuggerSuite";

        private AndroidJavaClass _xMediatorAdsProxy = new AndroidJavaClass(XMEDIATOR_ADS_PROXY_CLASSNAME);

        public void StartWith(string appKey, string unityVersion, InitSettings initSettings, Action<InitResult> initCallback)
        {
            var androidJavaObject = initSettings.ToInitSettingsDto().ToAndroidJavaObject();
            using (androidJavaObject)
            {
                _xMediatorAdsProxy.CallStatic(
                    INITIALIZE_METHOD_NAME,
                    AndroidUtils.GetUnityActivity(),
                    unityVersion,
                    appKey,
                    androidJavaObject,
                    new XMediatorUnityInitCallback(initCallback)
                );
            }
        }

        public void SetConsentInformation(ConsentInformation consentInformation)
        {
            var androidJavaObject = ConsentInformationDto.From(consentInformation).ToAndroidJavaObject();
            using (androidJavaObject)
            {
                _xMediatorAdsProxy.CallStatic(
                    SET_CONSENT_METHOD_NAME,
                    androidJavaObject
                );
            }
        }

        public UserProperties GetUserProperties()
        {
            return UserPropertiesDto.From(_xMediatorAdsProxy.CallStatic<AndroidJavaObject>(GET_USER_PROPERTIES_METHOD_NAME)).ToUserProperties();
        }

        public void SetUserProperties(UserProperties userProperties)
        {
            var androidJavaObject = UserPropertiesDto.From(userProperties).ToAndroidJavaObject();
            using (androidJavaObject)
            {
                _xMediatorAdsProxy.CallStatic(
                    SET_USER_PROPERTIES_METHOD_NAME,
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
            _xMediatorAdsProxy.CallStatic(
                OPEN_DEBUGGER_SUITE_METHOD_NAME,
                AndroidUtils.GetUnityActivity()
            );
        }
    }
}