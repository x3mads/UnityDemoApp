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
        
        public event Action AppOpenLoaded;
        public event Action InterstitialLoaded;
        public event Action RewardedLoaded;
        public event Action BannerLoaded;
        public event Action<string> OnWarning;
        public event Action OnResumeGame;
        public event Action FromShowFullScreenAd;

        private bool _isAutomaticCmp;
        private bool _isFakeEea;
        private AppConfiguration _appConfiguration;
#if UNITY_IOS
        private const string X3MAppKey = "3-15";
        private const string X3MBannerPlacementId = "3-15/28";
        private const string X3MInterstitialPlacementId = "3-15/26";
        private const string X3MRewardedPlacementId = "3-15/27";

        private const string MaxAppKey = "V148L42DRG";
        private const string MaxBannerPlacementId = "V142XB3LRNZCM7";
        private const string MaxInterstitialPlacementId = "V142XBGL601BCD";
        private const string MaxRewardedPlacementId = "V142DRKLYD2CVX";
        private const string MaxAppOpenPlacementId = "V14JHR48HLHAQ304";

        private const string AdmobAppKey = "V148L48DB9";
        private const string AdmobBannerPlacementId = "V14JHR4VKLPYKX70";
        private const string AdmobInterstitialPlacementId = "V14JHR4V2LCPBMMY";
        private const string AdmobRewardedPlacementId = "V14JHR4V3L78QZQ2";
        private const string AdmobAppOpenPlacementId = "V14JHR4VHLG8A741";

        private const string LPAppKey = "V148L42DR3";
        private const string LPBannerPlacementId = "V142YBZL36TNH7";
        private const string LPInterstitialPlacementId = "V142YB3L1J9DKT";
        private const string LPRewardedPlacementId = "V142YBGLYF22ST";
#elif UNITY_ANDROID
        private const string X3MAppKey = "3-15";
        private const string X3MBannerPlacementId = "3-15/28";
        private const string X3MInterstitialPlacementId = "3-15/26";
        private const string X3MRewardedPlacementId = "3-15/27";


        private const string MaxAppKey = "V148L42DB1";
        private const string MaxBannerPlacementId = "V142DR9L2247MG";
        private const string MaxInterstitialPlacementId = "V142DRJLE1G5XX";
        private const string MaxRewardedPlacementId = "V142DR4LW2MT4B";
        private const string MaxAppOpenPlacementId = "V14JHR4ZKL86RTC2";

        private const string AdmobAppKey = "V148L48DBJ";
        private const string AdmobBannerPlacementId = "V14JHR4Z2LKRFYNP";
        private const string AdmobInterstitialPlacementId = "V14JHR28KLVMZGXJ";
        private const string AdmobRewardedPlacementId = "V14JHR282L889BY3";
        private const string AdmobAppOpenPlacementId = "V14JHR283LMV0WYT";

        private const string LPAppKey = "V148L42DB8";
        private const string LPBannerPlacementId = "V142DR2LD0QYR1";
        private const string LPInterstitialPlacementId = "V142DR1L7WJN07";
        private const string LPRewardedPlacementId = "V142DR8L1DP5ND";
#endif
        
        private static readonly AppConfiguration MaxConfiguration = new(
            MaxAppKey, MaxBannerPlacementId, MaxInterstitialPlacementId, MaxRewardedPlacementId, MaxAppOpenPlacementId);

        private static readonly AppConfiguration LPConfiguration = new(
            LPAppKey, LPBannerPlacementId, LPInterstitialPlacementId, LPRewardedPlacementId, null);

        private static readonly AppConfiguration X3MConfiguration = new(
            X3MAppKey, X3MBannerPlacementId, X3MInterstitialPlacementId, X3MRewardedPlacementId, null);

        private static readonly AppConfiguration AdmobConfiguration = new(
            AdmobAppKey, AdmobBannerPlacementId, AdmobInterstitialPlacementId, AdmobRewardedPlacementId, AdmobAppOpenPlacementId);


        private const string ADSpace = "MAIN-SCREEN";

        public void ChangeMediator(int selectedIndex)
        {
            _appConfiguration = selectedIndex switch
            {
                1 => X3MConfiguration,
                2 => MaxConfiguration,
                3 => LPConfiguration,
                4 => AdmobConfiguration,
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
                    verbose: true, // Enable verbose logging, do not use in production  
                    test: true, // Enable test mode, do not use in production  
                    userProperties: new UserProperties(
                        userId: "test_user"
                    ),
                    consentInformation: consentInformation
                ),
                initCallback: result =>
                {
                    if (result.IsSuccess)
                    {
                        Debug.Log("Initialization complete! You can start loading your placements!");
                        LoadAppOpen();
                        LoadBanner();
                        LoadInterstitial();
                        LoadRewarded();
                        OnInitSDK?.Invoke();
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

        public void ShowAppOpen()
        {
            if (_appConfiguration.AppOpenPlacementId == null)
            {
                Debug.Log("AppOpen placement not supported by this Mediator");
                return;
            }
            if (XMediatorAds.AppOpen.IsReady(_appConfiguration.AppOpenPlacementId))
            {
                XMediatorAds.AppOpen.ShowFromAdSpace(_appConfiguration.AppOpenPlacementId, ADSpace);
            }
        }
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
            XMediatorAds.Rewarded.OnShowed += placementId =>
            {
                FromShowFullScreenAd?.Invoke();
                Debug.Log($"Rewarded is being shown! placementId: {placementId}");
            };

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
            XMediatorAds.Interstitial.OnShowed += placementId =>
            {
                FromShowFullScreenAd?.Invoke();
                Debug.Log($"Interstitial is being shown! placementId: {placementId}");
            };

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
        
        private void LoadAppOpen()
        {
            if (_appConfiguration.AppOpenPlacementId == null)
            {
                Debug.Log($"AppOpen placement not supported by this Mediator");
                return;
            }
            
            // Impression Callback
            XMediatorAds.AppOpen.OnImpression += (placementId, impressionData) => { Debug.Log($"AppOpen impression! placementId: {placementId}"); };

            // Click Callback
            XMediatorAds.AppOpen.OnClicked += placementId => { Debug.Log($"AppOpen clicked! placementId: {placementId}"); };

            // Load Callback
            XMediatorAds.AppOpen.OnLoaded += (placementId, result) =>
            {
                AppOpenLoaded?.Invoke();
                Debug.Log($"AppOpen loaded! placementId: {placementId}");
            };

            // Showed Callback
            XMediatorAds.AppOpen.OnShowed += placementId => { Debug.Log($"AppOpen is being shown! placementId: {placementId}"); };

            // Failed to show callback
            XMediatorAds.AppOpen.OnFailedToShow += (placementId, error) =>
            {
                // If you need to resume your app's flow, make sure to do it here and in the OnDismissed callback
                OnResumeGame?.Invoke();
                Debug.Log($"AppOpen failed to show. placementId: {placementId}, Reason: {error.Message}");
            };

            // Dismissed callback
            XMediatorAds.AppOpen.OnDismissed += placementId =>
            {
                // If you need to resume your app's flow, make sure to do it here and in the OnFailedToShow callback
                OnResumeGame?.Invoke();
                Debug.Log($"AppOpen dismissed! placementId: {placementId}, Resume gameplay");
            };
            
            XMediatorAds.AppOpen.Load(placementId: _appConfiguration.AppOpenPlacementId);
        }
    }
}