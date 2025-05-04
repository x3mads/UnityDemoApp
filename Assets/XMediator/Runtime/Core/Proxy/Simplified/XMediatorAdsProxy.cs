using System;
using XMediator.Api;

namespace XMediator
{
    internal interface XMediatorAdsProxy
    {
        void StartWith(
            string appKey,
            string unityVersion,
            InitSettings initSettings,
            Action<InitResult> initCallback
        );

        void SetConsentInformation(ConsentInformation consentInformation);
        UserProperties GetUserProperties();
        void SetUserProperties(UserProperties userProperties);
        
        void SetPauseOnAdPresentation(bool shouldPause);
        void OpenDebuggerSuite();
    }
}