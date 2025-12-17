using System.Runtime.InteropServices;
using JetBrains.Annotations;
using XMediator.Api;

namespace XMediator.iOS
{
    internal class iOSEventTrackerProxy : EventTrackerProxy
    {
        public void Track(PurchaseEvent purchaseEvent)
        {
            var purchaseEventDto = PurchaseEventDto.FromPurchaseEvent(purchaseEvent);
            X3MTrackPurchase(purchaseEventDto.ToJson());
        }

        public void Track(AdOpportunityEvent adOpportunity)
        {
            switch (adOpportunity.AdType)
            {
                case AdType.Interstitial:
                    X3MTrackInterstitialAdOpportunity(adOpportunity.AdSpace);
                    break;
                case AdType.Rewarded:
                    X3MTrackRewardedAdOpportunity(adOpportunity.AdSpace);
                    break;
                case AdType.AppOpen:
                    X3MTrackAppOpenAdOpportunity(adOpportunity.AdSpace);
                    break;
                case AdType.Banner:
                    // Banner doesn't support ad opportunity events
                    break;
            }
        }
        
        [DllImport("__Internal")]
        private static extern void X3MTrackPurchase(string purchase);
        
        [DllImport("__Internal")]
        private static extern void X3MTrackInterstitialAdOpportunity([CanBeNull] string adSpace);
        
        [DllImport("__Internal")]
        private static extern void X3MTrackRewardedAdOpportunity([CanBeNull] string adSpace);
        
        [DllImport("__Internal")]
        private static extern void X3MTrackAppOpenAdOpportunity([CanBeNull] string adSpace);
    }
}