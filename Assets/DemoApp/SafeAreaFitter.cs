using UnityEngine;

namespace DemoApp
{
    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaFitter : MonoBehaviour
    {
        private Rect _lastSafeArea;
        private Vector2Int _lastScreen;

        private void OnEnable() => Apply();

        private void Update() => Apply();

        private void Apply()
        {
            var safeArea = Screen.safeArea;
            var screen = new Vector2Int(Screen.width, Screen.height);
            if (safeArea == _lastSafeArea && screen == _lastScreen) return;
            _lastSafeArea = safeArea;
            _lastScreen = screen;
            if (screen.x <= 0 || screen.y <= 0) return;

            var rt = (RectTransform)transform;
            rt.anchorMin = new Vector2(safeArea.xMin / screen.x, safeArea.yMin / screen.y);
            rt.anchorMax = new Vector2(safeArea.xMax / screen.x, safeArea.yMax / screen.y);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
    }
}
