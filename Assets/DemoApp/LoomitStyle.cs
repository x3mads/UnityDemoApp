using System;
using UnityEngine;
using UnityEngine.UI;

namespace DemoApp
{
    public class LoomitStyle : MonoBehaviour
    {
        [Serializable]
        public class Entry
        {
            public Graphic graphic;
            public Color enabledColor = Color.white;
            public Color disabledColor = Color.white;
        }

        public Selectable target;
        public Entry[] entries;

        private int _lastState = -1;

        private void OnEnable() => Apply();

        private void Update() => Apply();

        private void Apply()
        {
            if (target == null) return;
            var state = target.IsInteractable() ? 1 : 0;
            if (state == _lastState) return;
            _lastState = state;
            foreach (var e in entries)
            {
                if (e.graphic != null) e.graphic.color = state == 1 ? e.enabledColor : e.disabledColor;
            }
        }
    }
}
