using UnityEngine;
using XMediator.Api;

namespace XMediator.Unity
{
    internal class UnityEventTrackerProxy : EventTrackerProxy
    {

        public void Track(PurchaseEvent purchaseEvent)
        {
            Debug.Log("[XMed] Track purchase event: " + purchaseEvent);
        }

        public void Track(AdOpportunityEvent adOpportunity)
        {
            Debug.Log("[XMed] Track ad opportunity event: " + adOpportunity);
        }
    }


}