namespace DemoApp
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;

    public class ToastManager : MonoBehaviour
    {
        public static ToastManager Instance;

        [SerializeField] private GameObject toastPanel;
        [SerializeField] private Text toastText;
        [SerializeField] private float displayDuration = 2f;
        [SerializeField] private float fadeDuration = 0.5f;

        private void Awake()
        {
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

        public void ShowToast(string message)
        {
            StopAllCoroutines();
            StartCoroutine(ShowToastCoroutine(message));
        }

        private IEnumerator ShowToastCoroutine(string message)
        {
            toastText.text = message;
            toastPanel.SetActive(true);

            // Fade in
            yield return FadePanel(0, 1);

            // Wait
            yield return new WaitForSeconds(displayDuration);

            // Fade out
            yield return FadePanel(1, 0);

            toastPanel.SetActive(false);
        }

        private IEnumerator FadePanel(float start, float end)
        {
            float elapsedTime = 0f;
            CanvasGroup canvasGroup = toastPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = toastPanel.AddComponent<CanvasGroup>();

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(start, end, elapsedTime / fadeDuration);
                yield return null;
            }
        }
    }
}