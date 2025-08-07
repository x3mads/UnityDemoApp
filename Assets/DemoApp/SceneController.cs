using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DemoApp
{
    public class UIInitHandler : MonoBehaviour
    {
        private TMP_Dropdown _mediatorDropdown;
        private Toggle _automaticCmpCheckbox;
        private Toggle _fakeEeaCheckbox;
        private Button _initButton;
        private Button _showAppOpenButton;
        private Button _showInterstitialButton;
        private Button _showRewardedButton;
        private Button _showBannerButton;
        private Button _debuggingSuiteButton;
        private Button _showFormButton;
        private GameObject _reopenWarningLabel;
        private GameObject _mediatorsPanel;

        private Button _resetButton;
        
        private bool _isFromShowFullScreenAd = false;

        // Start is called before the first frame update
        private UIControllerViewModel _viewModel;

        void Awake()
        {
            // Initialize the ViewModel
            _viewModel = new UIControllerViewModel();

            // Subscribe ViewModel events to handle UI logic
            _viewModel.MediatorChanged += OnMediatorChanged;
            _viewModel.AutomaticCmpToggled += OnAutomaticCMPChanged;
            _viewModel.FakeEeaToggled += OnFakeEEAChanged;
            _viewModel.OnInitSDK += OnInitSDK;
            _viewModel.AppOpenLoaded += OnLoadAppOpen;
            _viewModel.InterstitialLoaded += OnLoadInterstitial;
            _viewModel.RewardedLoaded += OnLoadRewarded;
            _viewModel.BannerLoaded += OnLoadBanner;
            _viewModel.OnResumeGame += OnResumeGame;
            _viewModel.OnWarning += OnWarning;
            _viewModel.FromShowFullScreenAd += OnFromShowFullScreenAd;
        }

        void Start()
        {
            // Find UI elements by name
            _mediatorDropdown = GameObject.Find("MediatorDropdown").GetComponent<TMP_Dropdown>();
            _automaticCmpCheckbox = GameObject.Find("AutoCmpToggle").GetComponent<Toggle>();
            _fakeEeaCheckbox = GameObject.Find("FakeRegionToggle").GetComponent<Toggle>();
            _initButton = GameObject.Find("InitButton").GetComponent<Button>();
            _showAppOpenButton = GameObject.Find("ShowApoButton").GetComponent<Button>();
            _showInterstitialButton = GameObject.Find("ShowIttButton").GetComponent<Button>();
            _showRewardedButton = GameObject.Find("ShowRewButton").GetComponent<Button>();
            _showBannerButton = GameObject.Find("ShowBannerButton").GetComponent<Button>();
            _debuggingSuiteButton = GameObject.Find("DebuggingSuiteButton").GetComponent<Button>();
            _showFormButton = GameObject.Find("ShowCmpFormButton").GetComponent<Button>();
            _resetButton = GameObject.Find("ResetCmpButton").GetComponent<Button>();
            _reopenWarningLabel = GameObject.Find("ReopenWarningLabel");
            _mediatorsPanel = GameObject.Find("MediatorsPanel");

            // Hook up UI events to the ViewModel
            _mediatorDropdown.onValueChanged.AddListener(_viewModel.ChangeMediator);
            _automaticCmpCheckbox.onValueChanged.AddListener(_viewModel.ToggleAutomaticCmp);
            _fakeEeaCheckbox.onValueChanged.AddListener(_viewModel.ToggleFakeEea);
            _initButton.onClick.AddListener(_viewModel.InitSDK);
            _showAppOpenButton.onClick.AddListener(_viewModel.ShowAppOpen);
            _showInterstitialButton.onClick.AddListener(_viewModel.ShowInterstitial);
            _showRewardedButton.onClick.AddListener(_viewModel.ShowRewarded);
            _showBannerButton.onClick.AddListener(_viewModel.ShowBanner);
            _debuggingSuiteButton.onClick.AddListener(UIControllerViewModel.DebuggingSuite);
            _showFormButton.onClick.AddListener(_viewModel.ShowForm);
            _resetButton.onClick.AddListener(UIControllerViewModel.Reset);

            // Initialize UI states
            _fakeEeaCheckbox.interactable = _automaticCmpCheckbox.isOn; // Disable Fake EEA checkbox initially if Automatic CMP is off
            _showAppOpenButton.interactable = false;
            _showInterstitialButton.interactable = false;
            _showRewardedButton.interactable = false;
            _showBannerButton.interactable = false;
            _showFormButton.interactable = false;
            _resetButton.interactable = false;
            _reopenWarningLabel.SetActive(false);
        }

        // Cleanup event subscriptions to avoid memory leaks
        void OnDestroy()
        {
            _viewModel.MediatorChanged -= OnMediatorChanged;
            _viewModel.AutomaticCmpToggled -= OnAutomaticCMPChanged;
            _viewModel.FakeEeaToggled -= OnFakeEEAChanged;
            _viewModel.OnInitSDK -= OnInitSDK;
            _viewModel.AppOpenLoaded -= OnLoadAppOpen;
            _viewModel.InterstitialLoaded -= OnLoadInterstitial;
            _viewModel.RewardedLoaded -= OnLoadRewarded;
            _viewModel.BannerLoaded -= OnLoadBanner;
        }

        private void OnFromShowFullScreenAd()
        {
            _isFromShowFullScreenAd = true;
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus && !_isFromShowFullScreenAd)
            {
                _viewModel.ShowAppOpen();
            }
            _isFromShowFullScreenAd = false;
        }

        private void OnResumeGame()
        {
            ToastManager.Instance.ShowToast("Game resume!");
        }

        private void OnWarning(string message)
        {
            ToastManager.Instance.ShowToast(message);
        }

        // ViewModel event handlers
        private void OnMediatorChanged(int selectedIndex)
        {
            if (selectedIndex == 4)
            {
                ShowConfigurationDialog();
            }
        }

        private void OnAutomaticCMPChanged(bool isOn)
        {
            _fakeEeaCheckbox.interactable = isOn;
            if (!isOn)
            {
                _fakeEeaCheckbox.isOn = false;
            }
        }

        private void OnFakeEEAChanged(bool isOn) { }

        private void OnInitSDK()
        {
            _initButton.interactable = false;
            _automaticCmpCheckbox.interactable = false;
            _fakeEeaCheckbox.interactable = false;
            _mediatorDropdown.interactable = false;
            _mediatorsPanel.SetActive(false);
            _reopenWarningLabel.SetActive(true);
            if (!_viewModel.IsPrivacyFormAvailable()) return;
            _showFormButton.interactable = true;
            _resetButton.interactable = true;
        }

        private void OnLoadAppOpen()
        {
            _showAppOpenButton.interactable = true;
        }

        private void OnLoadInterstitial()
        {
            _showInterstitialButton.interactable = true;
        }

        private void OnLoadRewarded()
        {
            _showRewardedButton.interactable = true;
        }

        private void OnLoadBanner()
        {
            _showBannerButton.interactable = true;
        }

        private void ShowConfigurationDialog()
        {
            ConfigurationDialog dialog = ConfigurationDialog.Instance;
            dialog.Show(
                (appKey, bannerPlacement, interstitialPlacement, rewardedPlacement, appOpenPlacement) =>
                {
                    _viewModel.ApplyCustomConfiguration(appOpenPlacement == ""
                        ? new AppConfiguration(appKey, bannerPlacement, interstitialPlacement, rewardedPlacement)
                        : new AppConfiguration(appKey, bannerPlacement, interstitialPlacement, rewardedPlacement, appOpenPlacement));
                }, () =>
                {
                    _mediatorDropdown.value = 0;
                    Debug.Log("Dialog cancelled");
                });
        }


        void Update() { }
    }
}