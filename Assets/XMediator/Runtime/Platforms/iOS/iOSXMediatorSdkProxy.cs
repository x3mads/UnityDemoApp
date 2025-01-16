using System;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;
using XMediator.Api;

namespace XMediator.iOS
{
    internal class iOSXMediatorSdkProxy : XMediatorSdkProxy
    {
        private delegate void NativeInitCallback(string result);

        private static Action<InitResult> _initCallback;
        private static Action<MediationResult> _mediationCallback;

        public void Initialize(string appKey,
            string unityVersion,
            InitSettings initSettings,
            Action<InitResult> initCallback,
            Action<MediationResult> mediationCallback
        )
        {
            _initCallback = initCallback;
            _mediationCallback = mediationCallback;
            var initSettingsDto = InitSettingsDto.FromInitSettings(initSettings, unityVersion);
            initialize(appKey, initSettingsDto.ToJson(), InitCallbackMethod, MediationCallbackMethod);
        }

        public void SetUserProperties(UserProperties userProperties)
        {
            var userPropertiesDto = UserPropertiesDto.FromUserProperties(userProperties);
            setUserProperties(userPropertiesDto.ToJson());
        }

        public void SetConsentInformation(ConsentInformation consentInformation)
        {
            var consentInformationDto = ConsentInformationDto.FromConsentInformation(consentInformation);
            setConsentInformation(consentInformationDto.ToJson());
        }

        public void SetPauseOnAdPresentation(bool shouldPause)
        {
            setPauseOnAdPresentation(shouldPause);
        }

        public void OpenDebuggerSuite()
        {
            ShowMediationDebugger();
        }

        [DllImport("__Internal")]
        private static extern void initialize(string appId,
            string initSettingsDto,
            NativeInitCallback initCallback,
            NativeInitCallback mediationCallback);
        
        [DllImport("__Internal")]
        private static extern void setUserProperties(string userPropertiesDto);

        [DllImport("__Internal")]
        private static extern void setConsentInformation(string consentInformationDto);
        
        [DllImport("__Internal")]
        private static extern void setPauseOnAdPresentation(bool shouldPause);

        [MonoPInvokeCallback(typeof(NativeInitCallback))]
        private static void InitCallbackMethod(string result)
        {
            var initResult = JsonUtility.FromJson<InitResultDto>(result).ToInitResult();
            _initCallback?.Invoke(initResult);
        }
        
        [DllImport("__Internal")]
        private static extern void ShowMediationDebugger();

        [MonoPInvokeCallback(typeof(NativeInitCallback))]
        private static void MediationCallbackMethod(string result)
        {
            var mediationResult = JsonUtility.FromJson<MediationResultDto>(result).ToMediationResult();
            _mediationCallback?.Invoke(mediationResult);
        }
    }
}