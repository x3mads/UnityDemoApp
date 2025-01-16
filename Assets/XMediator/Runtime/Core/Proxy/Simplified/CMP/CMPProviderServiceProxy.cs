using System;

namespace XMediator
{
    internal interface CMPProviderServiceProxy
    {
        public bool IsPrivacyFormAvailable();

        public void ShowPrivacyForm(Action<Exception> onComplete);

        public void Reset();
    }
}