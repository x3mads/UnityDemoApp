using System;
using XMediator.Api;

namespace XMediator
{
    internal interface ShowProxyListener
    {
        void OnFailedToShow(ShowError showError);
        void OnShowed();
        void OnImpression(ImpressionData impressionData);
        void OnClicked();
        void OnDismissed();
    }
}