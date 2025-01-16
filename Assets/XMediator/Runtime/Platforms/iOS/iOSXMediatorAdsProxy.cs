using System;
using System.Runtime.InteropServices;
using AOT;
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
        
        public void SetUserProperties(UserProperties userProperties)
        {
            var userPropertiesDto = UserPropertiesDto.FromUserProperties(userProperties);
            X3MSetUserProperties(userPropertiesDto.ToJson());
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

        [DllImport("__Internal")]
        private static extern void X3MStartWith(string appId,
            string initSettingsDto,
            NativeInitCallback initCallback);
        
        [DllImport("__Internal")]
        private static extern void X3MSetUserProperties(string userPropertiesDto);

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