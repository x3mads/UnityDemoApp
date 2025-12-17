using XMediator.Api;

namespace XMediator
{
    internal interface EventTrackerProxy
    {
        void Track(PurchaseEvent purchaseEvent);
        void Track(AdOpportunityEvent adOpportunity);
    }
}