using XMediator.Api;

namespace XMediator
{
    internal interface FullscreenShowProxyListener
    {
        void OnShowed(string placementId);
        
        void OnFailedToShow(string placementId, ShowError showError);

        void OnImpression(string placementId, ImpressionData impressionData);

        void OnClicked(string placementId);

        void OnDismissed(string placementId);
    }
}