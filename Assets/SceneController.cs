using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInitHandler : MonoBehaviour
{
    private TMP_Dropdown mediatorDropdown;
    private Toggle automaticCMPCheckbox;
    private Toggle fakeEEACheckbox;
    private Button initButton;
    private Button showInterstitialButton;
    private Button showRewardedButton;
    private Button showBannerButton;
    private Button debuggingSuiteButton;
    private Button showFormButton;

    private Button resetButton;

    // Start is called before the first frame update
    private UIControllerViewModel viewModel;

    void Awake()
    {
        // Initialize the ViewModel
        viewModel = new UIControllerViewModel();

        // Subscribe ViewModel events to handle UI logic
        viewModel.MediatorChanged += OnMediatorChanged;
        viewModel.AutomaticCMPToggled += OnAutomaticCMPChanged;
        viewModel.FakeEEAToggled += OnFakeEEAChanged;
        viewModel.InitSDKClicked += OnInitSDK;
        viewModel.ShowInterstitialClicked += OnShowInterstitial;
        viewModel.ShowRewardedClicked += OnShowRewarded;
        viewModel.ShowBannerClicked += OnShowBanner;
        viewModel.DebuggingSuiteClicked += OnDebuggingSuite;
        viewModel.ShowFormClicked += OnShowForm;
        viewModel.ResetClicked += OnReset;
    }

    void Start()
    {
        // Find UI elements by name
        mediatorDropdown = GameObject.Find("MediatorDropdown").GetComponent<TMP_Dropdown>();
        automaticCMPCheckbox = GameObject.Find("AutoCmpToggle").GetComponent<Toggle>();
        fakeEEACheckbox = GameObject.Find("FakeRegionToggle").GetComponent<Toggle>();
        initButton = GameObject.Find("InitButton").GetComponent<Button>();
        showInterstitialButton = GameObject.Find("ShowIttButton").GetComponent<Button>();
        showRewardedButton = GameObject.Find("ShowRewButton").GetComponent<Button>();
        showBannerButton = GameObject.Find("ShowBannerButton").GetComponent<Button>();
        debuggingSuiteButton = GameObject.Find("DebuggingSuiteButton").GetComponent<Button>();
        showFormButton = GameObject.Find("ShowCmpFormButton").GetComponent<Button>();
        resetButton = GameObject.Find("ResetCmpButton").GetComponent<Button>();

        // Hook up UI events to the ViewModel
        mediatorDropdown.onValueChanged.AddListener(viewModel.ChangeMediator);
        automaticCMPCheckbox.onValueChanged.AddListener(viewModel.ToggleAutomaticCMP);
        fakeEEACheckbox.onValueChanged.AddListener(viewModel.ToggleFakeEEA);
        initButton.onClick.AddListener(viewModel.InitSDK);
        showInterstitialButton.onClick.AddListener(viewModel.ShowInterstitial);
        showRewardedButton.onClick.AddListener(viewModel.ShowRewarded);
        showBannerButton.onClick.AddListener(viewModel.ShowBanner);
        debuggingSuiteButton.onClick.AddListener(viewModel.DebuggingSuite);
        showFormButton.onClick.AddListener(viewModel.ShowForm);
        resetButton.onClick.AddListener(viewModel.Reset);

        // Initialize UI states
        fakeEEACheckbox.interactable = automaticCMPCheckbox.isOn; // Disable Fake EEA checkbox initially if Automatic CMP is off
        showInterstitialButton.interactable = false;
        showRewardedButton.interactable = false;
        showBannerButton.interactable = false;
        showFormButton.interactable = false;
        resetButton.interactable = false;
    }

    // Cleanup event subscriptions to avoid memory leaks
    void OnDestroy()
    {
        viewModel.MediatorChanged -= OnMediatorChanged;
        viewModel.AutomaticCMPToggled -= OnAutomaticCMPChanged;
        viewModel.FakeEEAToggled -= OnFakeEEAChanged;
        viewModel.InitSDKClicked -= OnInitSDK;
        viewModel.ShowInterstitialClicked -= OnShowInterstitial;
        viewModel.ShowRewardedClicked -= OnShowRewarded;
        viewModel.ShowBannerClicked -= OnShowBanner;
        viewModel.DebuggingSuiteClicked -= OnDebuggingSuite;
        viewModel.ShowFormClicked -= OnShowForm;
        viewModel.ResetClicked -= OnReset;
    }

    // ViewModel event handlers
    private void OnMediatorChanged(int selectedIndex)
    {
        Debug.Log($"Mediator dropdown updated in UI: {mediatorDropdown.options[selectedIndex].text}");
    }

    private void OnAutomaticCMPChanged(bool isOn)
    {
        fakeEEACheckbox.interactable = isOn;
        if (!isOn)
        {
            fakeEEACheckbox.isOn = false;
        }
    }

    private void OnFakeEEAChanged(bool isOn)
    {
        Debug.Log($"Fake EEA checkbox updated in UI: {isOn}");
    }

    private void OnInitSDK()
    {
        Debug.Log("INIT SDK logic executed.");
    }

    private void OnShowInterstitial()
    {
        Debug.Log("SHOW INTERSTITIAL logic executed.");
    }

    private void OnShowRewarded()
    {
        Debug.Log("SHOW REWARDED logic executed.");
    }

    private void OnShowBanner()
    {
        Debug.Log("SHOW BANNER logic executed.");
    }

    private void OnDebuggingSuite()
    {
        Debug.Log("DEBUGGING SUITE logic executed.");
    }

    private void OnShowForm()
    {
        Debug.Log("SHOW FORM logic executed.");
    }

    private void OnReset()
    {
        Debug.Log("RESET logic executed.");
    }

    // Update is called once per frame
    void Update()
    {
    }
}