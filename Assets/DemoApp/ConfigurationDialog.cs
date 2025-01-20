namespace DemoApp
{
    using UnityEngine;
    using UnityEngine.UI;
    using System;

    public class ConfigurationDialog : MonoBehaviour
    {
        public static ConfigurationDialog Instance { get; private set; }

        [SerializeField] private InputField appKeyInput;
        [SerializeField] private InputField bannerPlacementInput;
        [SerializeField] private InputField interstitialPlacementInput;
        [SerializeField] private InputField rewardedPlacementInput;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private GameObject dialogPanel;

        private Action<string, string, string, string> _onConfirm;
        private Action _onCancel;

        private void Awake()
        {
            dialogPanel.SetActive(false);
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            confirmButton.onClick.AddListener(OnConfirmClicked);
            cancelButton.onClick.AddListener(OnCancelClicked);
        }

        public void Show(Action<string, string, string, string> confirmCallback, Action cancelCallback = null)
        {
            _onConfirm = confirmCallback;
            _onCancel = cancelCallback;
            dialogPanel.SetActive(true);

            var rectTransform = GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.sizeDelta = Vector2.zero;
                rectTransform.anchoredPosition = Vector2.zero;
            }

            transform.SetAsLastSibling();
        }

        private void OnConfirmClicked()
        {
            _onConfirm?.Invoke(
                appKeyInput.text,
                bannerPlacementInput.text,
                interstitialPlacementInput.text,
                rewardedPlacementInput.text
            );
            gameObject.SetActive(false);
        }

        private void OnCancelClicked()
        {
            _onCancel?.Invoke();
            gameObject.SetActive(false);
        }
    }
}