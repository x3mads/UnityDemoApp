using XMediator.Api;
using UnityEngine;

namespace XMediator.Android
{
    public class AndroidEventTrackerProxy : EventTrackerProxy
    {
        private const string EVENT_TRACKER_CLASS_NAME = "com.x3mads.android.xmediator.unityproxy.events.EventTrackerProxy";
        private const string TRACK_PURCHASE_METHOD_NAME = "trackPurchase";

        private const string TRACK_OPPORTUNITY_EVENT_NAME = "trackAdOpportunity";

        private static readonly AndroidJavaClass eventTrackerJavaClass = new AndroidJavaClass(EVENT_TRACKER_CLASS_NAME);

        public void Track(PurchaseEvent purchaseEvent)
        {
            eventTrackerJavaClass.CallStatic(
                TRACK_PURCHASE_METHOD_NAME,
                PurchaseEventDto.From(purchaseEvent)
            );
        }

        public void Track(AdOpportunityEvent adOpportunity)
        {
            eventTrackerJavaClass.CallStatic(
                TRACK_OPPORTUNITY_EVENT_NAME,
                AdOpportunityEventDto.From(adOpportunity)
            );
        }

    }
}