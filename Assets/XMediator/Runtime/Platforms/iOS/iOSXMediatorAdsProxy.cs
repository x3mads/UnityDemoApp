using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using AOT;
using JetBrains.Annotations;
using UnityEngine;
using XMediator.Api;

namespace XMediator.iOS
{
    public class iOSXMediatorAdsProxy : XMediatorAdsProxy
    {
        private delegate void NativeInitCallback(string result);

        private static Action<InitResult> _initCallback;
        
        public void StartWith(string appKey, string unityVersion, InitSettings initSettings, Action<InitResult> initCallback)
        {
            _initCallback = initCallback;
            var initSettingsDto = InitSettingsDto.FromInitSettings(initSettings, unityVersion);
            X3MStartWith(appKey,
                initSettingsDto.ToJson(),
                InitCallbackMethod
                );
        }

        public UserProperties GetUserProperties()
        {
            var userPropertiesString = X3MGetUserProperties();
            
            return JsonUtility.FromJson<UserPropertiesDto>(userPropertiesString).ToUserProperties();
        }

        public void SetUserProperties(UserProperties userProperties)
        {
            var userPropertiesDto = UserPropertiesDto.FromUserProperties(userProperties);
            X3MSetUserProperties(userPropertiesDto.ToJson());
        }

        public void SetUserId([CanBeNull] string userId)
        {
            X3MSetUserId(userId);
        }

        public void SetInstallDate(DateTimeOffset? installDate)
        {
            if (installDate == null)
            {
                X3MClearInstallDate();
                return;
            }

            X3MSetInstallDate(installDate.Value.ToUnixTimeSeconds());
        }

        public void SetPurchaseSummary([CanBeNull] InAppPurchaseSummary purchaseSummary)
        {
            if (purchaseSummary == null)
            {
                X3MSetPurchaseSummary(null);
                return;
            }

            var dto = InAppPurchaseSummaryDto.FromInAppPurchaseSummary(purchaseSummary);
            X3MSetPurchaseSummary(JsonUtility.ToJson(dto));
        }

        public void SetCustomProperty(string key, bool value)
        {
            X3MSetCustomPropertyBool(new CustomBoolPropertyDto(key, value).ToJson());
        }

        public void SetCustomProperty(string key, int value)
        {
            X3MSetCustomPropertyInt(new CustomIntPropertyDto(key, value).ToJson());
        }

        public void SetCustomProperty(string key, double value)
        {
            X3MSetCustomPropertyDouble(new CustomDoublePropertyDto(key, value).ToJson());
        }

        public void SetCustomProperty(string key, string value)
        {
            X3MSetCustomPropertyString(new CustomStringPropertyDto(key, value).ToJson());
        }

        public void SetCustomProperty(string key, IEnumerable<string> value)
        {
            X3MSetCustomPropertyStringArray(new CustomStringListPropertyDto(key, value.ToList()).ToJson());
        }

        public void RemoveCustomProperty(string key)
        {
            X3MRemoveCustomProperty(key);
        }

        public void ClearCustomProperties()
        {
            X3MClearCustomProperties();
        }

        public void ClearUserProperties()
        {
            X3MClearUserProperties();
        }

        public void SetConsentInformation(ConsentInformation consentInformation)
        {
            var consentInformationDto = ConsentInformationDto.FromConsentInformation(consentInformation);
            X3MSetConsentInformation(consentInformationDto.ToJson());
        }
        
        public void SetPauseOnAdPresentation(bool shouldPause)
        {
            X3MSetPauseOnAdPresentation(shouldPause);
        }

        public void OpenDebuggerSuite()
        {
            X3MShowMediationDebugger();
        }

        public bool IsInitialized()
        {
            return X3MIsInitialized();
        }

        [DllImport("__Internal")]
        private static extern bool X3MIsInitialized();

        [DllImport("__Internal")]
        private static extern void X3MStartWith(string appId,
            string initSettingsDto,
            NativeInitCallback initCallback);
        
        [DllImport("__Internal")]
        private static extern string X3MGetUserProperties();
        
        [DllImport("__Internal")]
        private static extern void X3MSetUserProperties(string userPropertiesDto);

        [DllImport("__Internal")]
        private static extern void X3MSetUserId(string userId);

        [DllImport("__Internal")]
        private static extern void X3MSetInstallDate(long timestamp);

        [DllImport("__Internal")]
        private static extern void X3MClearInstallDate();

        [DllImport("__Internal")]
        private static extern void X3MSetPurchaseSummary(string json);

        [DllImport("__Internal")]
        private static extern void X3MSetCustomPropertyBool(string json);

        [DllImport("__Internal")]
        private static extern void X3MSetCustomPropertyInt(string json);

        [DllImport("__Internal")]
        private static extern void X3MSetCustomPropertyDouble(string json);

        [DllImport("__Internal")]
        private static extern void X3MSetCustomPropertyString(string json);

        [DllImport("__Internal")]
        private static extern void X3MSetCustomPropertyStringArray(string json);

        [DllImport("__Internal")]
        private static extern void X3MRemoveCustomProperty(string key);

        [DllImport("__Internal")]
        private static extern void X3MClearCustomProperties();

        [DllImport("__Internal")]
        private static extern void X3MClearUserProperties();

        [DllImport("__Internal")]
        private static extern void X3MSetConsentInformation(string consentInformationDto);
        
        [DllImport("__Internal")]
        private static extern void X3MSetPauseOnAdPresentation(bool shouldPause);        
        
        [DllImport("__Internal")]
        private static extern void X3MShowMediationDebugger();
        
        [MonoPInvokeCallback(typeof(NativeInitCallback))]
        private static void InitCallbackMethod(string result)
        {
            var initResult = JsonUtility.FromJson<InitResultDto>(result).ToInitResult();
            _initCallback?.Invoke(initResult);
        }
    }
}