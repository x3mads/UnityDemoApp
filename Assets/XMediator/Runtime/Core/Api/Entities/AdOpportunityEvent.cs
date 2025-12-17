using JetBrains.Annotations;

namespace XMediator.Api
{
    /// <summary>
    /// Represents an ad opportunity event that should be tracked each time there's an opportunity to show an ad to the user.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This event should be tracked <strong>before</strong> checking if the ad is ready or capped, and should only be tracked <strong>once per opportunity</strong>.
    /// The purpose is to measure potential ad impressions regardless of whether the ad is actually shown.
    /// </para>
    /// <para>
    /// <strong>Important:</strong> Track this event before any ad readiness or capping checks to accurately measure all ad opportunities.
    /// </para>
    /// <para>
    /// <strong>Note:</strong> For rewarded ads, the best practice is to track the opportunity before showing a popup or button with the reward to the user.
    /// </para>
    /// </remarks>
    public class AdOpportunityEvent
    {
        /// <summary>
        /// The type of ad.
        /// </summary>
        internal AdType AdType { get; }

        /// <summary>
        /// An optional identifier indicating where in the application the ad was attempting to be shown.
        /// </summary>
        [CanBeNull] internal string AdSpace { get; }

        /// <summary>
        /// Creates a new <see cref="AdOpportunityEvent"/> instance.
        /// </summary>
        /// <param name="adType">The type of ad.</param>
        /// <param name="adSpace">An optional identifier indicating where in the application the ad was attempting to be shown.</param>
        private AdOpportunityEvent(AdType adType, [CanBeNull] string adSpace)
        {
            AdType = adType;
            AdSpace = adSpace;
        }

        /// <summary>
        /// Creates an interstitial ad opportunity event.
        /// </summary>
        /// <param name="adSpace">An optional identifier indicating where in the application the ad was attempting to be shown.
        /// While optional, it is <strong>recommended</strong> to provide a meaningful value (e.g., "main_menu", "level_complete").</param>
        /// <returns>An <see cref="AdOpportunityEvent"/> configured for an interstitial ad.</returns>
        public static AdOpportunityEvent Interstitial([CanBeNull] string adSpace)
        {
            return new AdOpportunityEvent(AdType.Interstitial, adSpace);
        }

        /// <summary>
        /// Creates a rewarded ad opportunity event.
        /// </summary>
        /// <param name="adSpace">An optional identifier indicating where in the application the ad was attempting to be shown.
        /// While optional, it is <strong>recommended</strong> to provide a meaningful value (e.g., "extra_lives", "bonus_coins").</param>
        /// <returns>An <see cref="AdOpportunityEvent"/> configured for a rewarded ad.</returns>
        /// <remarks>
        /// For rewarded ads, track this opportunity <strong>before</strong> showing a popup or button with the reward to the user,
        /// not when the user actually clicks to watch the ad.
        /// </remarks>
        public static AdOpportunityEvent Rewarded([CanBeNull] string adSpace)
        {
            return new AdOpportunityEvent(AdType.Rewarded, adSpace);
        }

        /// <summary>
        /// Creates an app open ad opportunity event.
        /// </summary>
        /// <param name="adSpace">An optional identifier indicating where in the application the ad was attempting to be shown.
        /// While optional, it is <strong>recommended</strong> to provide a meaningful value (e.g., "app_launch", "app_resume").</param>
        /// <returns>An <see cref="AdOpportunityEvent"/> configured for an app open ad.</returns>
        public static AdOpportunityEvent AppOpen([CanBeNull] string adSpace)
        {
            return new AdOpportunityEvent(AdType.AppOpen, adSpace);
        }

        public override string ToString()
        {
            return $"{nameof(AdType)}: {AdType}, {nameof(AdSpace)}: {AdSpace}";
        }
    }
}
