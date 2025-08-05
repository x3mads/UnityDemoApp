using System.Runtime.InteropServices;
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
        
        [DllImport("__Internal")]
        private static extern void X3MTrackPurchase(string purchase);
    }
}