using System;
using UnityEngine;

internal class UIControllerViewModel
{
    // Public events to notify the UIController about logic changes
    public event Action<int> MediatorChanged;
    public event Action<bool> AutomaticCMPToggled;
    public event Action<bool> FakeEEAToggled;
    public event Action InitSDKClicked;
    public event Action ShowInterstitialClicked;
    public event Action ShowRewardedClicked;
    public event Action ShowBannerClicked;
    public event Action DebuggingSuiteClicked;
    public event Action ShowFormClicked;
    public event Action ResetClicked;

    // Logic for Mediator dropdown selection
    public void ChangeMediator(int selectedIndex)
    {
        Debug.Log($"Mediator changed to index: {selectedIndex}");
        MediatorChanged?.Invoke(selectedIndex); // Notify observers
    }

    // Logic for Automatic CMP checkbox toggle
    public void ToggleAutomaticCMP(bool isOn)
    {
        Debug.Log($"Automatic CMP toggled: {isOn}");
        AutomaticCMPToggled?.Invoke(isOn); // Notify observers
    }

    // Logic for Fake EEA checkbox toggle
    public void ToggleFakeEEA(bool isOn)
    {
        Debug.Log($"Fake EEA toggled: {isOn}");
        FakeEEAToggled?.Invoke(isOn); // Notify observers
    }

    // Button click handlers
    public void InitSDK()
    {
        InitSDKClicked?.Invoke();
    }
    public void ShowInterstitial() => ShowInterstitialClicked?.Invoke();
    public void ShowRewarded() => ShowRewardedClicked?.Invoke();
    public void ShowBanner() => ShowBannerClicked?.Invoke();
    public void DebuggingSuite() => DebuggingSuiteClicked?.Invoke();
    public void ShowForm() => ShowFormClicked?.Invoke();
    public void Reset() => ResetClicked?.Invoke();
}