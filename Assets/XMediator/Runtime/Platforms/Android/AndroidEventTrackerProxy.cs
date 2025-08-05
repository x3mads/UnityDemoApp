using XMediator.Api;
using UnityEngine;

namespace XMediator.Android
{
    public class AndroidEventTrackerProxy : EventTrackerProxy
    {
        private const string PURCHASE_TRACKER_CLASS_NAME = "com.x3mads.android.xmediator.unityproxy.events.EventTrackerProxy";
        private const string TRACK_PURCHASE_METHOD_NAME = "trackPurchase";

        private static readonly AndroidJavaClass purchaseTrackerJavaClass = new AndroidJavaClass(PURCHASE_TRACKER_CLASS_NAME);

        public void Track(PurchaseEvent purchaseEvent)
        {
            purchaseTrackerJavaClass.CallStatic(
                TRACK_PURCHASE_METHOD_NAME,
                PurchaseEventDto.From(purchaseEvent)
            );
        }


    }
}