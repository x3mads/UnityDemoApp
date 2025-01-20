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
        private Button _showInterstitialButton;
        private Button _showRewardedButton;
        private Button _showBannerButton;
        private Button _debuggingSuiteButton;
        private Button _showFormButton;

        private Button _resetButton;

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
            _viewModel.InterstitialLoaded += OnLoadInterstitial;
            _viewModel.RewardedLoaded += OnLoadRewarded;
            _viewModel.BannerLoaded += OnLoadBanner;
            _viewModel.OnResumeGame += OnResumeGame;
            _viewModel.OnWarning += OnWarning;
        }

        void Start()
        {
            // Find UI elements by name
            _mediatorDropdown = GameObject.Find("MediatorDropdown").GetComponent<TMP_Dropdown>();
            _automaticCmpCheckbox = GameObject.Find("AutoCmpToggle").GetComponent<Toggle>();
            _fakeEeaCheckbox = GameObject.Find("FakeRegionToggle").GetComponent<Toggle>();
            _initButton = GameObject.Find("InitButton").GetComponent<Button>();
            _showInterstitialButton = GameObject.Find("ShowIttButton").GetComponent<Button>();
            _showRewardedButton = GameObject.Find("ShowRewButton").GetComponent<Button>();
            _showBannerButton = GameObject.Find("ShowBannerButton").GetComponent<Button>();
            _debuggingSuiteButton = GameObject.Find("DebuggingSuiteButton").GetComponent<Button>();
            _showFormButton = GameObject.Find("ShowCmpFormButton").GetComponent<Button>();
            _resetButton = GameObject.Find("ResetCmpButton").GetComponent<Button>();

            // Hook up UI events to the ViewModel
            _mediatorDropdown.onValueChanged.AddListener(_viewModel.ChangeMediator);
            _automaticCmpCheckbox.onValueChanged.AddListener(_viewModel.ToggleAutomaticCmp);
            _fakeEeaCheckbox.onValueChanged.AddListener(_viewModel.ToggleFakeEea);
            _initButton.onClick.AddListener(_viewModel.InitSDK);
            _showInterstitialButton.onClick.AddListener(_viewModel.ShowInterstitial);
            _showRewardedButton.onClick.AddListener(_viewModel.ShowRewarded);
            _showBannerButton.onClick.AddListener(_viewModel.ShowBanner);
            _debuggingSuiteButton.onClick.AddListener(UIControllerViewModel.DebuggingSuite);
            _showFormButton.onClick.AddListener(UIControllerViewModel.ShowForm);
            _resetButton.onClick.AddListener(UIControllerViewModel.Reset);

            // Initialize UI states
            _fakeEeaCheckbox.interactable = _automaticCmpCheckbox.isOn; // Disable Fake EEA checkbox initially if Automatic CMP is off
            _showInterstitialButton.interactable = false;
            _showRewardedButton.interactable = false;
            _showBannerButton.interactable = false;
            _showFormButton.interactable = false;
            _resetButton.interactable = false;
        }

        // Cleanup event subscriptions to avoid memory leaks
        void OnDestroy()
        {
            _viewModel.MediatorChanged -= OnMediatorChanged;
            _viewModel.AutomaticCmpToggled -= OnAutomaticCMPChanged;
            _viewModel.FakeEeaToggled -= OnFakeEEAChanged;
            _viewModel.OnInitSDK -= OnInitSDK;
            _viewModel.InterstitialLoaded -= OnLoadInterstitial;
            _viewModel.RewardedLoaded -= OnLoadRewarded;
            _viewModel.BannerLoaded -= OnLoadBanner;
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
            if (_viewModel.IsPrivacyFormAvailable())
            {
                _showFormButton.interactable = true;
                _resetButton.interactable = true;
            }
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
                (appKey, bannerPlacement, interstitialPlacement, rewardedPlacement) =>
                {
                    _viewModel.ApplyCustomConfiguration(new AppConfiguration(appKey, bannerPlacement, interstitialPlacement, rewardedPlacement));
                }, () =>
                {
                    _mediatorDropdown.value = 0;
                    Debug.Log("Dialog cancelled");
                });
        }

        
        void Update()
        {
        }
    }
}