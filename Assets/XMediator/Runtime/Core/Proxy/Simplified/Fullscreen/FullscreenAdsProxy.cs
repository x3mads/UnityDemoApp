namespace XMediator
{
    internal interface FullscreenAdsProxy<FullscreenListener>
    {
        void SetListener(FullscreenListener listener);

        void Load(string placementId);

        bool IsReady();

        bool IsReady(string placementId);
        
        bool IsAdSpaceCapped(string adSpace);
        
        void Show();
        
        void Show(string placementId);
        
        void ShowFromAdSpace(string adSpace);
        
        void ShowFromAdSpace(string placementId, string adSpace);
    }
}