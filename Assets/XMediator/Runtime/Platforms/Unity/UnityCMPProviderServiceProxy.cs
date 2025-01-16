using System;
using UnityEngine;
using XMediator.Core.Util;

namespace XMediator.Unity
{
    internal class UnityCMPProviderServiceProxy : CMPProviderServiceProxy
    {
        public bool IsPrivacyFormAvailable()
        {
            const bool isPrivacyFormAvailable = true;
            Log($"IsPrivacyFormAvailable() -> {isPrivacyFormAvailable}");
            return isPrivacyFormAvailable;
        }

        public void ShowPrivacyForm(Action<Exception> onComplete)
        {
            Log($"ShowPrivacyForm() called.");
            XMediatorMainThreadDispatcher.Enqueue(() =>
            {
                onComplete(null);
            });
        }

        public void Reset()
        {
            Log($"Reset() called.");
        }

        private static void Log(string message)
        {
            Debug.Log($"[XMed] {message}");
        }
    }
}