using System;
using System.Collections.Generic;
using JetBrains.Annotations;
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
        void SetUserId([CanBeNull] string userId);
        void SetInstallDate(DateTimeOffset? installDate);
        void SetPurchaseSummary([CanBeNull] InAppPurchaseSummary purchaseSummary);
        void SetCustomProperty(string key, bool value);
        void SetCustomProperty(string key, int value);
        void SetCustomProperty(string key, double value);
        void SetCustomProperty(string key, string value);
        void SetCustomProperty(string key, IEnumerable<string> value);
        void RemoveCustomProperty(string key);
        void ClearCustomProperties();
        void ClearUserProperties();
        
        void SetPauseOnAdPresentation(bool shouldPause);
        void OpenDebuggerSuite();
        bool IsInitialized();
    }
}