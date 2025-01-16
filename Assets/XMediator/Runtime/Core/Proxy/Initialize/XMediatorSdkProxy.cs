using System;
using XMediator.Api;

namespace XMediator
{
    internal interface XMediatorSdkProxy
    {
        void Initialize(
            string appKey,
            string unityVersion,
            InitSettings initSettings,
            Action<InitResult> initCallback,
            Action<MediationResult> mediationCallback
        );

        void SetUserProperties(UserProperties userProperties);

        void SetConsentInformation(ConsentInformation consentInformation);
        
        void SetPauseOnAdPresentation(bool shouldPause);
        void OpenDebuggerSuite();
    }
}