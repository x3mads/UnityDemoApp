using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace XMediator.Unity.Views
{
    public class UnityFullScreenView : MonoBehaviour
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private Button skipButton;
        [SerializeField] private Animator animator;
        [SerializeField] private Button button;

        private static readonly int FadeInState = Animator.StringToHash("fadeIn");
        private static readonly int FadeOutState = Animator.StringToHash("fadeOut");

        public event Action OnClicked;
        public event Action OnEarnedReward;
        public event Action OnDismissed;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Deactivate();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            animator.SetTrigger(FadeInState);
            button.onClick.AddListener(() => { OnClicked?.Invoke(); });
            closeButton.onClick.AddListener(() =>
            {
                Hide();
                OnEarnedReward?.Invoke();
                OnDismissed?.Invoke();
            });
            if (skipButton != null)
            {
                skipButton.onClick.AddListener(() =>
                {
                    Hide();
                    OnDismissed?.Invoke();
                });
            }
        }

        private async void Hide()
        {
            animator.SetTrigger(FadeOutState);
            await Task.Delay(500);
            Destroy(gameObject);
        }

        private void Deactivate()
        {
            gameObject.SetActive(false);
        }

        public static UnityFullScreenView Instantiate(string type)
        {
            return Object.Instantiate(GetPrefab(type));
        }

        private static UnityFullScreenView GetPrefab(string type)
        {
            if (type == "itt") return Resources.Load<UnityFullScreenView>("com.x3mads.xmediator/Prefabs/Interstitial");
            if (type == "rew") return Resources.Load<UnityFullScreenView>("com.x3mads.xmediator/Prefabs/RewardedVideo");
            if (type == "apo") return Resources.Load<UnityFullScreenView>("com.x3mads.xmediator/Prefabs/AppOpen");
            return null;
        }
    }
}