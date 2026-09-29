using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
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
        private const string SET_USER_ID_METHOD_NAME = "setUserId";
        private const string SET_INSTALL_DATE_METHOD_NAME = "setInstallDate";
        private const string SET_PURCHASE_SUMMARY_METHOD_NAME = "setPurchaseSummary";
        private const string SET_CUSTOM_PROPERTY_INT_METHOD_NAME = "setCustomPropertyInt";
        private const string SET_CUSTOM_PROPERTY_DOUBLE_METHOD_NAME = "setCustomPropertyDouble";
        private const string SET_CUSTOM_PROPERTY_STRING_METHOD_NAME = "setCustomPropertyString";
        private const string SET_CUSTOM_PROPERTY_BOOLEAN_METHOD_NAME = "setCustomPropertyBoolean";
        private const string SET_CUSTOM_PROPERTY_STRING_SET_METHOD_NAME = "setCustomPropertyStringSet";
        private const string REMOVE_CUSTOM_PROPERTY_METHOD_NAME = "removeCustomProperty";
        private const string CLEAR_CUSTOM_PROPERTIES_METHOD_NAME = "clearCustomProperties";
        private const string CLEAR_USER_PROPERTIES_METHOD_NAME = "clearUserProperties";
        private const string OPEN_DEBUGGER_SUITE_METHOD_NAME = "openDebuggerSuite";
        private const string IS_INITIALIZED_METHOD_NAME = "isInitialized";

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

        public void SetUserId([CanBeNull] string userId)
        {
            _xMediatorAdsProxy.CallStatic(SET_USER_ID_METHOD_NAME, userId);
        }

        public void SetInstallDate(DateTimeOffset? installDate)
        {
            using (var installDateJavaObject = Utils.ToAndroidLong(installDate?.ToUnixTimeMilliseconds()))
            {
                _xMediatorAdsProxy.CallStatic(SET_INSTALL_DATE_METHOD_NAME, installDateJavaObject);
            }
        }

        public void SetPurchaseSummary([CanBeNull] InAppPurchaseSummary purchaseSummary)
        {
            var purchaseSummaryJavaObject = purchaseSummary == null ? null : InAppPurchaseSummaryDto.From(purchaseSummary).ToAndroidJavaObject();
            using (purchaseSummaryJavaObject)
            {
                _xMediatorAdsProxy.CallStatic(SET_PURCHASE_SUMMARY_METHOD_NAME, purchaseSummaryJavaObject);
            }
        }

        public void SetCustomProperty(string key, bool value)
        {
            _xMediatorAdsProxy.CallStatic(SET_CUSTOM_PROPERTY_BOOLEAN_METHOD_NAME, key, value);
        }

        public void SetCustomProperty(string key, int value)
        {
            _xMediatorAdsProxy.CallStatic(SET_CUSTOM_PROPERTY_INT_METHOD_NAME, key, value);
        }

        public void SetCustomProperty(string key, double value)
        {
            _xMediatorAdsProxy.CallStatic(SET_CUSTOM_PROPERTY_DOUBLE_METHOD_NAME, key, value);
        }

        public void SetCustomProperty(string key, string value)
        {
            _xMediatorAdsProxy.CallStatic(SET_CUSTOM_PROPERTY_STRING_METHOD_NAME, key, value);
        }

        public void SetCustomProperty(string key, IEnumerable<string> value)
        {
            _xMediatorAdsProxy.CallStatic(SET_CUSTOM_PROPERTY_STRING_SET_METHOD_NAME, key, value.ToArray());
        }

        public void RemoveCustomProperty(string key)
        {
            _xMediatorAdsProxy.CallStatic(REMOVE_CUSTOM_PROPERTY_METHOD_NAME, key);
        }

        public void ClearCustomProperties()
        {
            _xMediatorAdsProxy.CallStatic(CLEAR_CUSTOM_PROPERTIES_METHOD_NAME);
        }

        public void ClearUserProperties()
        {
            _xMediatorAdsProxy.CallStatic(CLEAR_USER_PROPERTIES_METHOD_NAME);
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

        public bool IsInitialized()
        {
            return _xMediatorAdsProxy.CallStatic<bool>(IS_INITIALIZED_METHOD_NAME);
        }
    }
}