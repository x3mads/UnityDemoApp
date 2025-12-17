using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using XMediator.Api;

namespace XMediator.Unity
{
    internal class UnityXMediatorSdkProxy : XMediatorSdkProxy
    {
        internal static Action<string, InitSettings, Action<InitResult>, Action<MediationResult>> OnInit = DefaultOnInit;
        internal static Action<ConsentInformation> OnSetConsentInformation = DefaultOnSetConsentInformation;

        public void Initialize(
            string appKey,
            string unityVersion,
            InitSettings initSettings,
            Action<InitResult> initCallback,
            Action<MediationResult> mediationCallback
        )
        {
            Assert.IsNotNull(appKey, "Initialize error: appKey is null. Please provide a valid appKey.");
            Assert.IsFalse(appKey == "", "Initialize error: appKey is empty. Please provide a valid appKey.");
            
            var xmediatorVersion = "1.63.0"; // TODO Get XMediator Unity Version
            Log($"Running XMediator {xmediatorVersion} | AppKey: {appKey} | Client version: {initSettings.ClientVersion} | Unity version: {unityVersion}");
            OnInit.Invoke(appKey, initSettings, initCallback, mediationCallback);
        }

        public void SetUserProperties(UserProperties userProperties)
        {
            Assert.IsNotNull(userProperties, "SetUserProperties error: userProperties is null. If you want to clear user properties, please provide an empty UserProperties object.");
            
            Log($"Setting user properties: UserId: {userProperties.UserId}, Custom Properties: {userProperties.CustomProperties.GetAll()}");
        }

        public void SetConsentInformation(ConsentInformation consentInformation)
        {
            Assert.IsNotNull(consentInformation, "SetConsentInformation error: consentInformation is null. If you want to clear user consent, please provide an empty ConsentInformation object.");
            OnSetConsentInformation.Invoke(consentInformation);
        }

        public void SetPauseOnAdPresentation(bool shouldPause)
        {
            // Do nothing, only needed for iOS
        }

        public void OpenDebuggerSuite()
        {
            Log("OpenDebuggingSuite called. This feature is available only on native platforms, please Build and Run the project from an Android or iOS device to open X3M's Debugging Suite.");
        }

        private static async void DefaultOnInit(
            string appKey,
            InitSettings initSettings,
            Action<InitResult> initCallback,
            Action<MediationResult> mediationCallback
        )
        {
            await Task.Delay(1000);
            Log("Initialize complete!");
            await Task.Run(() => initCallback.Invoke(new InitResult.Success(Guid.NewGuid().ToString())));
            await Task.Delay(1000);
            await Task.Run(() => mediationCallback.Invoke(new MediationResult(Enumerable.Empty<NetworkInitResult>())));
        }

        private static void DefaultOnSetConsentInformation(ConsentInformation consentInformation)
        {
            Log($"Setting consent information: {consentInformation}");
        }

        private static void Log(string message)
        {
            Debug.Log($"[XMed] {message}");
        }
    }
}