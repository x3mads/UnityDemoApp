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
    }

}
