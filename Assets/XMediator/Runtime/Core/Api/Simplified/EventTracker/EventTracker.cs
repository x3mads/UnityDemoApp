using System;

namespace XMediator.Api
{

    /// <summary>
    /// Tracks events such as purchases for analytics and segmentation purposes.
    ///
    /// - Note: The <see cref="Track"> method should only be called after the SDK is properly initialized. If <see cref="Track"> is called before SDK initialization, events will be ignored.
    /// </summary>
    public class EventTracker
    {

        private static readonly EventTrackerProxy _eventTracker = ProxyFactory.CreateInstance<EventTrackerProxy>("EventTrackerProxy");

        internal EventTracker()
        {
            
        }

        /// <summary>
        /// Tracks a purchase event.
        /// 
        /// - Note: This method should only be called after the SDK is properly initialized. If called before SDK initialization, events will be ignored.
        /// </summary>
        /// <param name="purchaseEvent">The purchase event to track.</param>
        public void Track(PurchaseEvent purchaseEvent)
        {
            _eventTracker.Track(purchaseEvent);
        }

        /// <summary>
        /// Tracks an ad opportunity event.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method should be called each time there is an opportunity to show an ad to the user, <strong>before</strong> checking if the ad is ready or capped.
        /// Track this event only <strong>once per opportunity</strong> to accurately measure potential ad impressions.
        /// </para>
        /// <para>
        /// <strong>Important:</strong> This method should be called only after the SDK has been initialized. If called before initialization, events will be ignored.
        /// </para>
        /// <para>
        /// <strong>Note:</strong> For rewarded ads, track the opportunity before showing a popup or button with the reward to the user, not when they click to watch the ad.
        /// </para>
        /// </remarks>
        /// <param name="adOpportunity">The ad opportunity event to track. Use the factory methods on <see cref="AdOpportunityEvent"/> to create instances
        /// (e.g., <c>AdOpportunityEvent.Interstitial(adSpace)</c>, <c>AdOpportunityEvent.Rewarded(adSpace)</c>, <c>AdOpportunityEvent.AppOpen(adSpace)</c>).</param>
        public void Track(AdOpportunityEvent adOpportunity)
        {
            _eventTracker.Track(adOpportunity);
        }
    }

}
