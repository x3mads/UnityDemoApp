using System;
using UnityEngine;
using UnityEngine.UI;
using XMediator.Api;

namespace XMediator.Unity.Views
{
    public class UnityEmbeddedScreenView : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Button button;

        public event Action OnClicked;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            gameObject.SetActive(false);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }

        public static UnityEmbeddedScreenView Instantiate(Banner.Position position, Banner.Size size)
        {
            var view = UnityEngine.Object.Instantiate(GetPrefab(size));
            view.SetPosition(position);
            view.gameObject.SetActive(true);
            view.button.onClick.AddListener(() => view.OnClicked?.Invoke());
            return view;
        }

        public static UnityEmbeddedScreenView Instantiate(int x, int y, Banner.Size size)
        {
            var view = UnityEngine.Object.Instantiate(GetPrefab(size));
            view.SetPosition(x, y);
            view.gameObject.SetActive(true);
            view.button.onClick.AddListener(() => view.OnClicked?.Invoke());
            return view;
        }

        private static UnityEmbeddedScreenView GetPrefab(Banner.Size size)
        {
            if (size == Banner.Size.MREC)
                return Resources.Load<UnityEmbeddedScreenView>("com.x3mads.xmediator/Prefabs/BannerMREC");
            return Resources.Load<UnityEmbeddedScreenView>("com.x3mads.xmediator/Prefabs/Banner");
        }

        private void SetPosition(Banner.Position position)
        {
            if (position == Banner.Position.Top)
                ConfigTransform(new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1));
            else
                ConfigTransform(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0));
        }

        private void SetPosition(int x, int y)
        {
            rectTransform.anchoredPosition = new Vector2(x, -y);
        }

        private void ConfigTransform(Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot)
        {
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.pivot = pivot;
            rectTransform.anchoredPosition = Vector2.zero;
        }
    }
}