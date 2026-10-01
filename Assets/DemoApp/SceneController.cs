using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XMediator.Core.Util;

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

        private InputField _userIdInput;
        private TMP_Text _userPropertiesText;
        private Button _userPropertiesToggle;
        private GameObject _userPropertiesCard;
        private RectTransform _userPropertiesChevron;
        private Button _setUserIdButton;
        private Button _setInstallDateButton;
        private Button _setPurchaseSummaryButton;
        private Button _setCustomPropertiesButton;
        private Button _removeCustomPropertyButton;
        private Button _getUserPropertiesButton;
        private Button _clearCustomPropertiesButton;
        private Button _clearUserPropertiesButton;
        private Button _trackPurchaseButton;
        private Button _trackAppEventButton;
        private bool _userPropertiesExpanded;
        private readonly Dictionary<Graphic, (Coroutine routine, Color baseColor)> _flashes = new Dictionary<Graphic, (Coroutine routine, Color baseColor)>();

        private bool _isFromShowFullScreenAd = false;

        // Start is called before the first frame update
        private UIControllerViewModel _viewModel;

        void Awake()
        {
            Debug.developerConsoleVisible = false;
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
            _userIdInput = GameObject.Find("UserIdInput").GetComponent<InputField>();
            _userPropertiesText = GameObject.Find("UserPropertiesText").GetComponent<TMP_Text>();
            _userPropertiesToggle = GameObject.Find("UserPropertiesToggle").GetComponent<Button>();
            _userPropertiesCard = GameObject.Find("UserPropertiesCard");
            _userPropertiesChevron = GameObject.Find("UserPropertiesChevron").GetComponent<RectTransform>();
            _setUserIdButton = GameObject.Find("SetUserIdButton").GetComponent<Button>();
            _setInstallDateButton = GameObject.Find("SetInstallDateButton").GetComponent<Button>();
            _setPurchaseSummaryButton = GameObject.Find("SetPurchaseSummaryButton").GetComponent<Button>();
            _setCustomPropertiesButton = GameObject.Find("SetCustomPropertiesButton").GetComponent<Button>();
            _removeCustomPropertyButton = GameObject.Find("RemoveCustomPropertyButton").GetComponent<Button>();
            _getUserPropertiesButton = GameObject.Find("GetUserPropertiesButton").GetComponent<Button>();
            _clearCustomPropertiesButton = GameObject.Find("ClearCustomPropertiesButton").GetComponent<Button>();
            _clearUserPropertiesButton = GameObject.Find("ClearUserPropertiesButton").GetComponent<Button>();
            _trackPurchaseButton = GameObject.Find("TrackPurchaseButton").GetComponent<Button>();
            _trackAppEventButton = GameObject.Find("TrackAppEventButton").GetComponent<Button>();

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
            _userPropertiesToggle.onClick.AddListener(() => SetUserPropertiesExpanded(!_userPropertiesExpanded));
            BindUserPropertiesAction(_setUserIdButton, () => _viewModel.SetUserId(_userIdInput.text));
            BindUserPropertiesAction(_setInstallDateButton, _viewModel.SetInstallDateNow);
            BindUserPropertiesAction(_setPurchaseSummaryButton, _viewModel.SetSamplePurchaseSummary);
            BindUserPropertiesAction(_setCustomPropertiesButton, _viewModel.SetSampleCustomProperties);
            BindUserPropertiesAction(_removeCustomPropertyButton, _viewModel.RemoveSampleCustomProperty);
            BindUserPropertiesAction(_getUserPropertiesButton, () => true);
            BindUserPropertiesAction(_clearCustomPropertiesButton, _viewModel.ClearCustomProperties);
            BindUserPropertiesAction(_clearUserPropertiesButton, _viewModel.ClearUserProperties);
            BindFlashAction(_trackPurchaseButton, _viewModel.TrackTestPurchase);
            BindFlashAction(_trackAppEventButton, _viewModel.TrackTestAppEvent);

            // Initialize UI states
            _fakeEeaCheckbox.interactable = _automaticCmpCheckbox.isOn; // Disable Fake EEA checkbox initially if Automatic CMP is off
            _showAppOpenButton.interactable = false;
            _showInterstitialButton.interactable = false;
            _showRewardedButton.interactable = false;
            _showBannerButton.interactable = false;
            _showFormButton.interactable = false;
            _resetButton.interactable = false;
            _reopenWarningLabel.SetActive(false);
            SetUserPropertiesExpanded(false);
        }

        private bool RefreshUserProperties()
        {
            _userPropertiesText.text = _viewModel.GetUserPropertiesText();
            return !_userPropertiesText.text.StartsWith("Error:");
        }

        private void BindUserPropertiesAction(Button button, Func<bool> action)
        {
            BindFlashAction(button, () =>
            {
                var success = action();
                return RefreshUserProperties() && success;
            });
        }

        private void BindFlashAction(Button button, Func<bool> action)
        {
            button.onClick.AddListener(() => Flash(button, action()));
        }

        private void Flash(Button button, bool success)
        {
            if (!(button.targetGraphic is Graphic graphic)) return;
            if (_flashes.TryGetValue(graphic, out var running))
            {
                StopCoroutine(running.routine);
                graphic.color = running.baseColor;
            }
            var baseColor = graphic.color;
            var flashColor = success
                ? (baseColor.a > 0.5f ? Color.white : new Color(0f, 1f, 0.71f, 0.45f))
                : new Color(0.9f, 0.28f, 0.3f, 0.85f);
            _flashes[graphic] = (StartCoroutine(FlashRoutine(graphic, baseColor, flashColor)), baseColor);
        }

        private IEnumerator FlashRoutine(Graphic graphic, Color baseColor, Color flashColor)
        {
            const float duration = 0.45f;
            for (var t = 0f; t < duration; t += Time.unscaledDeltaTime)
            {
                graphic.color = Color.Lerp(flashColor, baseColor, t / duration);
                yield return null;
            }
            graphic.color = baseColor;
            _flashes.Remove(graphic);
        }

        private void SetUserPropertiesExpanded(bool expanded)
        {
            _userPropertiesExpanded = expanded;
            _userPropertiesCard.SetActive(expanded);
            _userPropertiesChevron.localEulerAngles = new Vector3(0, 0, expanded ? 180 : 0);
            if (!expanded && _userIdInput.isFocused)
            {
                _userIdInput.DeactivateInputField();
            }
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
                _viewModel.ShowAppOpenOnResume();
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
            if (selectedIndex == 5)
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
            XMediatorMainThreadDispatcher.Enqueue(() =>
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
            });
        }

        private void OnLoadAppOpen()
        {
            XMediatorMainThreadDispatcher.Enqueue(() => 
                    _showAppOpenButton.interactable = true
            );
        }

        private void OnLoadInterstitial()
        {
            XMediatorMainThreadDispatcher.Enqueue(() => 
                    _showInterstitialButton.interactable = true
            );
        }

        private void OnLoadRewarded()
        {
            XMediatorMainThreadDispatcher.Enqueue(() => 
                    _showRewardedButton.interactable = true
            );
        }

        private void OnLoadBanner()
        {
            XMediatorMainThreadDispatcher.Enqueue(() => 
                    _showBannerButton.interactable = true
            );
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