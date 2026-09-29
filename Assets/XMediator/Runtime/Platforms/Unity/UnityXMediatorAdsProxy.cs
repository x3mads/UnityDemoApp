using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;
using XMediator.Api;

namespace XMediator.Unity
{
    internal class UnityXMediatorAdsProxy : XMediatorAdsProxy
    {
        internal static Action<string, InitSettings, Action<InitResult>> OnInit = DefaultOnInit;
        internal static Action<ConsentInformation> OnSetConsentInformation = DefaultOnSetConsentInformation;
        internal static Action<UserProperties> OnSetUserProperties = DefaultOnSetUserProperties;

        private static UserProperties _userProperties; 

        public void StartWith(string appKey, string unityVersion, InitSettings initSettings, Action<InitResult> initCallback)
        {
            Assert.IsNotNull(appKey, "Initialize error: appKey is null. Please provide a valid appKey.");
            Assert.IsFalse(appKey == "", "Initialize error: appKey is empty. Please provide a valid appKey.");
            
            if (initSettings?.UserProperties != null)
            {
                _userProperties = initSettings.UserProperties;
            }

            var xmediatorVersion = "1.74.0"; // TODO Get XMediator Unity Version
            Debug.Log($"Running XMediator {xmediatorVersion} | AppKey: {appKey} | Client version: {initSettings.ClientVersion} | Unity version: {unityVersion}");
            OnInit.Invoke(appKey, initSettings, initCallback);
        }

        public void SetConsentInformation(ConsentInformation consentInformation)
        {
            Assert.IsNotNull(consentInformation, "SetConsentInformation error: consentInformation is null. If you want to clear user consent, please provide an empty ConsentInformation object.");
            OnSetConsentInformation.Invoke(consentInformation);
        }

        public UserProperties GetUserProperties()
        {
            return _userProperties ?? new UserProperties();
        }

        public void SetUserProperties(UserProperties userProperties)
        {
            Assert.IsNotNull(userProperties, "SetUserProperties error: userProperties is null. If you want to clear user properties, please provide an empty UserProperties object.");
            
            _userProperties = userProperties;
            OnSetUserProperties.Invoke(userProperties);
        }

        public void SetUserId([CanBeNull] string userId)
        {
            ReplaceUserProperties(userId: userId, keepUserId: false);
        }

        public void SetInstallDate(DateTimeOffset? installDate)
        {
            ReplaceUserProperties(installDate: installDate, keepInstallDate: false);
        }

        public void SetPurchaseSummary([CanBeNull] InAppPurchaseSummary purchaseSummary)
        {
            ReplaceUserProperties(inAppPurchaseSummary: purchaseSummary, keepPurchaseSummary: false);
        }

        public void SetCustomProperty(string key, bool value)
        {
            UpdateCustomProperties(builder => builder.AddBoolean(key, value));
        }

        public void SetCustomProperty(string key, int value)
        {
            UpdateCustomProperties(builder => builder.AddInt(key, value));
        }

        public void SetCustomProperty(string key, double value)
        {
            UpdateCustomProperties(builder => builder.AddDouble(key, value));
        }

        public void SetCustomProperty(string key, string value)
        {
            UpdateCustomProperties(builder => builder.AddString(key, value));
        }

        public void SetCustomProperty(string key, IEnumerable<string> value)
        {
            UpdateCustomProperties(builder => builder.AddStringSet(key, value));
        }

        public void RemoveCustomProperty(string key)
        {
            UpdateCustomProperties(builder => builder.Remove(key));
        }

        public void ClearCustomProperties()
        {
            ReplaceUserProperties(customProperties: new CustomProperties.Builder().Build(), keepCustomProperties: false);
        }

        public void ClearUserProperties()
        {
            _userProperties = new UserProperties();
            OnSetUserProperties.Invoke(_userProperties);
        }

        public void SetPauseOnAdPresentation(bool shouldPause)
        {
            // Do nothing, only needed for iOS
        }

        public void OpenDebuggerSuite()
        {
            Log("OpenDebuggingSuite called. This feature is available only on native platforms, please Build and Run the project from an Android or iOS device to open X3M's Debugging Suite.");
        }

        public bool IsInitialized()
        {
            Log("IsInitialized called. This feature is available only on native platforms, please Build and Run the project from an Android or iOS device to check if XMediator is initialized.");
            return true;
        }

        private static async void DefaultOnInit(
            string appKey,
            InitSettings initSettings,
            Action<InitResult> initCallback
        )
        {
            await Task.Delay(1000);
            Log("Initialize complete!");
            await Task.Run(() => initCallback.Invoke(new InitResult.Success(Guid.NewGuid().ToString())));
        }

        private static void DefaultOnSetConsentInformation(ConsentInformation consentInformation)
        {
            Log($"Setting consent information: {consentInformation}");
        }

        private static void DefaultOnSetUserProperties(UserProperties userProperties)
        {
            Log($"Setting User Properties: {userProperties}");
        }

        private void UpdateCustomProperties(Action<CustomProperties.Builder> editAction)
        {
            var current = GetUserProperties();
            var builder = current.CustomProperties.NewBuilder();
            editAction(builder);
            ReplaceUserProperties(customProperties: builder.Build(), keepCustomProperties: false);
        }

        private void ReplaceUserProperties(
            [CanBeNull] string userId = null,
            bool keepUserId = true,
            DateTimeOffset? installDate = null,
            bool keepInstallDate = true,
            [CanBeNull] InAppPurchaseSummary inAppPurchaseSummary = null,
            bool keepPurchaseSummary = true,
            [CanBeNull] CustomProperties customProperties = null,
            bool keepCustomProperties = true)
        {
            var current = GetUserProperties();
            _userProperties = new UserProperties(
                userId: keepUserId ? current.UserId : userId,
                customProperties: keepCustomProperties ? current.CustomProperties : customProperties,
                installDate: keepInstallDate ? current.InstallDate : installDate,
                inAppPurchaseSummary: keepPurchaseSummary ? current.InAppPurchaseSummary : inAppPurchaseSummary
            );
            OnSetUserProperties.Invoke(_userProperties);
        }

        private static void Log(string message)
        {
            Debug.Log($"[XMed] {message}");
        }
    }
}