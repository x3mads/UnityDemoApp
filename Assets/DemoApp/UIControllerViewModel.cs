using System;
using UnityEngine;
using XMediator.Api;

namespace DemoApp
{
    internal class UIControllerViewModel
    {
        // Public events to notify the UIController about logic changes
        public event Action<int> MediatorChanged;
        public event Action<bool> AutomaticCmpToggled;
        public event Action<bool> FakeEeaToggled;
        public event Action OnInitSDK;
        public event Action InterstitialLoaded;
        public event Action RewardedLoaded;
        public event Action BannerLoaded;
        public event Action<string> OnWarning;
        public event Action OnResumeGame;

        private bool _isAutomaticCmp;
        private bool _isFakeEea;
        private AppConfiguration _appConfiguration;


        private static readonly AppConfiguration MaxConfiguration = new(
            "3-180", "3-180/1150", "3-180/1151", "3-180/1152");
    
        private static readonly AppConfiguration LPConfiguration = new(
            "3-181", "3-181/1153", "3-181/1154", "3-181/1155");
    
        private static readonly AppConfiguration X3MConfiguration = new(
            "3-15", "3-15/28", "3-15/26", "3-15/27");

        private const string ADSpace = "MAIN-SCREEN";

        public void ChangeMediator(int selectedIndex)
        {
            _appConfiguration = selectedIndex switch
            {
                1 => X3MConfiguration,
                2 => MaxConfiguration,
                3 => LPConfiguration,
                _ => _appConfiguration
            };

            Debug.Log($"Mediator changed to index: {selectedIndex}");
            MediatorChanged?.Invoke(selectedIndex); // Notify observers
        }

        public void ToggleAutomaticCmp(bool isOn)
        {
            _isAutomaticCmp = isOn;
            Debug.Log($"Automatic CMP toggled: {isOn}");
            AutomaticCmpToggled?.Invoke(isOn); // Notify observers
        }

        public void ToggleFakeEea(bool isOn)
        {
            _isFakeEea = isOn;
            Debug.Log($"Fake EEA toggled: {isOn}");
            FakeEeaToggled?.Invoke(isOn); // Notify observers
        }

        public void InitSDK()
        {
            if (_appConfiguration == null)
            {
                OnWarning?.Invoke("Select a Mediator first");
                return;
            }

            Debug.Log("Initializing SDK...");
            CMPDebugSettings cmpDebugSettings = null;
            if (_isFakeEea)
            {
                cmpDebugSettings = new CMPDebugSettings(
                    debugGeography: CMPDebugGeography.EEA // Only for test proposes, do not use in production
                );
            }

            ConsentInformation consentInformation = null;
            if (_isAutomaticCmp)
            {
                consentInformation = new ConsentInformation(
                    isCMPAutomationEnabled: true,
                    cmpDebugSettings: cmpDebugSettings);
            }

            XMediatorAds.StartWith(
                appKey: _appConfiguration.AppKey,
                initSettings: new InitSettings(
                    verbose: true, // Enable verbose logging
                    test: true, // Enable test mode, do not use in production  
                    userProperties: new UserProperties(
                        userId: "test_user"
                    ),
                    consentInformation: consentInformation
                ),
                initCallback: result =>
                {
                    LoadBanner();
                    LoadInterstitial();
                    LoadRewarded();
                    OnInitSDK?.Invoke();
                    if (result.IsSuccess)
                    {
                        Debug.Log("Initialization complete! You can start loading your placements!");
                    }
                    else
                    {
                        result.TryGetFailure(out var failure);
                        Debug.Log("Initialization failed: " + failure.ErrorDescription);
                        OnWarning?.Invoke("Initialization failed: " + failure.ErrorDescription);
                    }
                }
            );
        }

        public bool IsPrivacyFormAvailable() => XMediatorAds.CMPProvider.IsPrivacyFormAvailable();

        public void ShowInterstitial()
        {
            if (XMediatorAds.Interstitial.IsReady(_appConfiguration.InterstitialPlacementId))
            {
                XMediatorAds.Interstitial.ShowFromAdSpace(_appConfiguration.InterstitialPlacementId, ADSpace);
            }
        }

        public void ShowRewarded()
        {
            if (XMediatorAds.Rewarded.IsReady(_appConfiguration.RewardedPlacementId))
            {
                XMediatorAds.Rewarded.ShowFromAdSpace(_appConfiguration.RewardedPlacementId, ADSpace);
            }
        }

        public void ShowBanner()
        {
            XMediatorAds.Banner.SetAdSpace(_appConfiguration.BannerPlacementId, ADSpace);
            XMediatorAds.Banner.Show(_appConfiguration.BannerPlacementId);
        }

        public static void DebuggingSuite()
        {
            XMediatorAds.OpenDebuggingSuite();
        }

        public void ShowForm()
        {
            XMediatorAds.CMPProvider.ShowPrivacyForm((error) =>
            {
                if (error != null)
                {
                    OnWarning?.Invoke(($"CMP Error: {error.Message}"));
                    Debug.Log($"Error: {error}");
                }

                Debug.Log($"ShowPrivacyForm complete!");
            });
        }

        public static void Reset()
        {
            XMediatorAds.CMPProvider.Reset();
        }

        public void ApplyCustomConfiguration(AppConfiguration appConfiguration)
        {
            _appConfiguration = appConfiguration;
        }

        private void LoadBanner()
        {
            // Load Callback
            XMediatorAds.Banner.OnLoaded += (placementId, result) =>
            {
                BannerLoaded?.Invoke();
                Debug.Log($"Banner loaded! placementId: {placementId}");
            };

            // Impression Callback
            XMediatorAds.Banner.OnImpression += (placementId, impressionData) => { Debug.Log($"Banner impression! placementId: {placementId}"); };

            // Click Callback
            XMediatorAds.Banner.OnClicked += placementId => { Debug.Log($"Banner clicked! placementId: {placementId}"); };
            XMediatorAds.Banner.Create(placementId: _appConfiguration.BannerPlacementId, size: BannerAds.Size.Phone, position: BannerAds.Position.Bottom);
        }

        private void LoadRewarded()
        {
            // Reward Callback
            XMediatorAds.Rewarded.OnEarnedReward += placementId => { Debug.Log($"{placementId} Rewarded ad earned a reward!"); };

            // Impression Callback
            XMediatorAds.Rewarded.OnImpression += (placementId, impressionData) => { Debug.Log($"Rewarded impression! placementId: {placementId}"); };

            // Click Callback
            XMediatorAds.Rewarded.OnClicked += placementId => { Debug.Log($"Rewarded clicked! placementId: {placementId}"); };

            // Load Callback
            XMediatorAds.Rewarded.OnLoaded += (placementId, result) =>
            {
                RewardedLoaded?.Invoke();
                Debug.Log($"Rewarded loaded! placementId: {placementId}");
            };

            // Showed Callback
            XMediatorAds.Rewarded.OnShowed += placementId => { Debug.Log($"Rewarded is being shown! placementId: {placementId}"); };

            // Failed to show callback
            XMediatorAds.Rewarded.OnFailedToShow += (placementId, error) =>
            {
                // If you need to resume your app's flow, make sure to do it here and in the OnDismissed callback
                OnResumeGame?.Invoke();
                Debug.Log($"Rewarded failed to show. placementId: {placementId}, Reason: {error.Message}");
            };

            // Dismissed callback
            XMediatorAds.Rewarded.OnDismissed += placementId =>
            {
                // If you need to resume your app's flow, make sure to do it here and in the OnFailedToShow callback
                OnResumeGame?.Invoke();
                Debug.Log($"Rewarded dismissed! placementId: {placementId}, Resume gameplay");
            };
            XMediatorAds.Rewarded.Load(placementId: _appConfiguration.RewardedPlacementId);
        }

        private void LoadInterstitial()
        {
            // Impression Callback
            XMediatorAds.Interstitial.OnImpression += (placementId, impressionData) => { Debug.Log($"Interstitial impression! placementId: {placementId}"); };

            // Click Callback
            XMediatorAds.Interstitial.OnClicked += placementId => { Debug.Log($"Interstitial clicked! placementId: {placementId}"); };

            // Load Callback
            XMediatorAds.Interstitial.OnLoaded += (placementId, result) =>
            {
                InterstitialLoaded?.Invoke();
                Debug.Log($"Interstitial loaded! placementId: {placementId}");
            };

            // Showed Callback
            XMediatorAds.Interstitial.OnShowed += placementId => { Debug.Log($"Interstitial is being shown! placementId: {placementId}"); };

            // Failed to show callback
            XMediatorAds.Interstitial.OnFailedToShow += (placementId, error) =>
            {
                // If you need to resume your app's flow, make sure to do it here and in the OnDismissed callback
                OnResumeGame?.Invoke();
                Debug.Log($"Interstitial failed to show. placementId: {placementId}, Reason: {error.Message}");
            };

            // Dismissed callback
            XMediatorAds.Interstitial.OnDismissed += placementId =>
            {
                // If you need to resume your app's flow, make sure to do it here and in the OnFailedToShow callback
                OnResumeGame?.Invoke();
                Debug.Log($"Interstitial dismissed! placementId: {placementId}, Resume gameplay");
            };

            XMediatorAds.Interstitial.Load(placementId: _appConfiguration.InterstitialPlacementId);
        }
    }
}