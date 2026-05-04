namespace XMediator.Api
{
    /// <summary>
    /// A catalog of predefined app event names used with <see cref="AppEvent.Standard"/>.
    /// </summary>
    /// <remarks>
    /// <para>Each entry maps to a canonical event name string recognized by the XMediator backend
    /// and integrated mediation adapters.</para>
    /// <para>
    /// Events are grouped into the following categories:
    /// <list type="bullet">
    /// <item><description><strong>Ecommerce</strong> — Shopping and purchase lifecycle events (e.g., <see cref="AddToCart"/>, <see cref="Purchase"/>).</description></item>
    /// <item><description><strong>Finance</strong> — Financial transactions and account management (e.g., <see cref="FinancialDeposit"/>, <see cref="AccountOpened"/>).</description></item>
    /// <item><description><strong>Gaming</strong> — Game session, level progression, and in-game economy (e.g., <see cref="GameStart"/>, <see cref="LevelComplete"/>).</description></item>
    /// <item><description><strong>Onboarding</strong> — First-run experience and tutorial events (e.g., <see cref="TutorialStart"/>, <see cref="TutorialComplete"/>).</description></item>
    /// <item><description><strong>Account</strong> — Authentication and account lifecycle (e.g., <see cref="SignUp"/>, <see cref="Login"/>).</description></item>
    /// <item><description><strong>Generic</strong> — General-purpose events across app categories (e.g., <see cref="Search"/>, <see cref="GenerateLead"/>).</description></item>
    /// <item><description><strong>Social</strong> — Social interaction and content-sharing (e.g., <see cref="FriendInvite"/>, <see cref="SocialShare"/>).</description></item>
    /// <item><description><strong>Subscription</strong> — Subscription and free trial lifecycle (e.g., <see cref="Subscribe"/>, <see cref="StartTrial"/>).</description></item>
    /// <item><description><strong>Service &amp; Travel</strong> — Booking, ordering, and travel services (e.g., <see cref="Order"/>, <see cref="Booking"/>).</description></item>
    /// <item><description><strong>Engagement</strong> — Notification, re-engagement, and app lifecycle (e.g., <see cref="PushNotificationEnable"/>, <see cref="ReEngage"/>).</description></item>
    /// </list>
    /// </para>
    /// <example>
    /// <code>
    /// var appEvent = AppEvent.Standard(AppEventName.LevelComplete);
    /// XMediatorAds.EventTracker.Track(appEvent);
    /// </code>
    /// </example>
    /// </remarks>
    public enum AppEventName
    {
        // Ecommerce

        /// <summary>Ecommerce event: User navigated to a new page or screen.</summary>
        PageView,
        /// <summary>Ecommerce event: User viewed a specific product or content page.</summary>
        ViewItem,
        /// <summary>Ecommerce event: User viewed a list of products or search results.</summary>
        ViewItemList,
        /// <summary>Ecommerce event: User added an item to their cart.</summary>
        AddToCart,
        /// <summary>Ecommerce event: User removed an item from their cart.</summary>
        RemoveFromCart,
        /// <summary>Ecommerce event: User viewed their cart.</summary>
        ViewCart,
        /// <summary>Ecommerce event: User initiated the checkout process.</summary>
        BeginCheckout,
        /// <summary>Ecommerce event: User completed a purchase (ecommerce or in-app).</summary>
        Purchase,
        /// <summary>Ecommerce event: User submitted payment information.</summary>
        AddPaymentInfo,
        /// <summary>Ecommerce event: User added or removed an item from their wishlist.</summary>
        WishlistUpdated,
        /// <summary>Ecommerce event: User initiated a refund for a previous purchase.</summary>
        RefundRequested,

        // Finance

        /// <summary>Finance event: User deposited money or funds into their account.</summary>
        FinancialDeposit,
        /// <summary>Finance event: User withdrew money or funds from their account.</summary>
        FinancialWithdraw,
        /// <summary>Finance event: Any financial transaction not covered by deposit or withdraw.</summary>
        FinancialTransaction,
        /// <summary>Finance event: User submitted a financial or service application.</summary>
        SubmitApplication,
        /// <summary>Finance event: User opened a new financial account.</summary>
        AccountOpened,
        /// <summary>Finance event: User closed a financial account.</summary>
        AccountClosed,

        // Gaming

        /// <summary>Gaming event: Player began a new game session.</summary>
        GameStart,
        /// <summary>Gaming event: Player ended the game (loss, timeout, etc.).</summary>
        GameOver,
        /// <summary>Gaming event: Player started a level.</summary>
        LevelStart,
        /// <summary>Gaming event: Player successfully completed a level.</summary>
        LevelComplete,
        /// <summary>Gaming event: Player failed or lost a level.</summary>
        LevelFail,
        /// <summary>Gaming event: Player quit out of a level before finishing.</summary>
        LevelQuit,
        /// <summary>Gaming event: Player skipped past a level.</summary>
        LevelSkip,
        /// <summary>Gaming event: Player increased in rank or experience level.</summary>
        LevelUp,
        /// <summary>Gaming event: Player spent or earned in-game virtual resources.</summary>
        VirtualResourceTransaction,
        /// <summary>Gaming event: Player's in-game energy reached zero.</summary>
        EnergyDepleted,
        /// <summary>Gaming event: Player used an in-game tool or booster.</summary>
        UseProp,
        /// <summary>Gaming event: Player opened the in-game shop or store.</summary>
        GameShopEnter,
        /// <summary>Gaming event: Player selected or clicked an item in the store.</summary>
        StoreItemClick,
        /// <summary>Gaming event: Player completed an achievement.</summary>
        AchievementUnlocked,
        /// <summary>Gaming event: Player completed a milestone towards an achievement.</summary>
        AchievementStep,
        /// <summary>Gaming event: Player began watching a cinematic cutscene.</summary>
        CutsceneStart,
        /// <summary>Gaming event: Player skipped a cinematic cutscene.</summary>
        CutsceneSkip,

        // Onboarding

        /// <summary>Onboarding event: User completed their first interaction after install.</summary>
        FirstInteraction,
        /// <summary>Onboarding event: User began a tutorial.</summary>
        TutorialStart,
        /// <summary>Onboarding event: User passed a milestone within a tutorial.</summary>
        TutorialStep,
        /// <summary>Onboarding event: User completed a tutorial or onboarding flow.</summary>
        TutorialComplete,
        /// <summary>Onboarding event: User skipped a tutorial.</summary>
        TutorialSkip,

        // Account

        /// <summary>Account event: User opened or launched the app.</summary>
        AppOpen,
        /// <summary>Account event: User completed registration or account creation.</summary>
        SignUp,
        /// <summary>Account event: User logged into their account.</summary>
        Login,
        /// <summary>Account event: Account verification process began.</summary>
        AccountVerificationStart,
        /// <summary>Account event: User completed account verification.</summary>
        AccountVerificationComplete,
        /// <summary>Account event: User linked a third-party or social account.</summary>
        AccountLinked,

        // Generic

        /// <summary>Generic event: User performed a search within the app.</summary>
        Search,
        /// <summary>Generic event: User submitted info that generates a lead.</summary>
        GenerateLead,
        /// <summary>Generic event: User submitted a review or rating.</summary>
        ProductReview,

        // Social

        /// <summary>Social event: User sent an invite to another person.</summary>
        FriendInvite,
        /// <summary>Social event: User shared content via a social network.</summary>
        SocialShare,
        /// <summary>Social event: User accepted something shared through a social network.</summary>
        SocialAccept,
        /// <summary>Social event: User sent or received a gift within the app.</summary>
        GiftTransaction,
        /// <summary>Social event: User joined a group, team, or community.</summary>
        GroupJoined,
        /// <summary>Social event: User sent a message via in-app chat.</summary>
        ChatSent,
        /// <summary>Social event: User viewed a social post or user-generated content.</summary>
        PostView,
        /// <summary>Social event: User created or uploaded a new post or content.</summary>
        PostCreated,

        // Subscription

        /// <summary>Subscription event: User subscribed to a paid plan or service.</summary>
        Subscribe,
        /// <summary>Subscription event: User started a free trial.</summary>
        StartTrial,
        /// <summary>Subscription event: User canceled their subscription.</summary>
        SubscriptionCanceled,

        // Service & Travel

        /// <summary>Service &amp; Travel event: User placed a service or product order.</summary>
        Order,
        /// <summary>Service &amp; Travel event: User made a booking or reservation.</summary>
        Booking,
        /// <summary>Service &amp; Travel event: Booking was confirmed.</summary>
        BookingConfirmed,
        /// <summary>Service &amp; Travel event: User canceled a booking.</summary>
        BookingCanceled,
        /// <summary>Service &amp; Travel event: User completed online check-in.</summary>
        OnlineCheckIn,
        /// <summary>Service &amp; Travel event: User applied a promotional code.</summary>
        PromoCodeApplied,

        // Engagement

        /// <summary>Engagement event: User enabled push notifications.</summary>
        PushNotificationEnable,
        /// <summary>Engagement event: User tapped on a push notification.</summary>
        PushNotificationClick,
        /// <summary>Engagement event: User re-engaged with the app after a period of inactivity.</summary>
        ReEngage,
        /// <summary>Engagement event: App was updated to a new version.</summary>
        Update,
        /// <summary>Engagement event: User was assigned to or changed customer segment.</summary>
        CustomerSegment,
        /// <summary>Engagement event: User's location was captured.</summary>
        LocationCoordinates,
    }

    internal static class AppEventNameExtensions
    {
        internal static string ToRawValue(this AppEventName name)
        {
            switch (name)
            {
                case AppEventName.PageView: return "page_view";
                case AppEventName.ViewItem: return "view_item";
                case AppEventName.ViewItemList: return "view_item_list";
                case AppEventName.AddToCart: return "add_to_cart";
                case AppEventName.RemoveFromCart: return "remove_from_cart";
                case AppEventName.ViewCart: return "view_cart";
                case AppEventName.BeginCheckout: return "begin_checkout";
                case AppEventName.Purchase: return "purchase";
                case AppEventName.AddPaymentInfo: return "add_payment_info";
                case AppEventName.WishlistUpdated: return "wishlist_updated";
                case AppEventName.RefundRequested: return "refund_requested";
                case AppEventName.FinancialDeposit: return "financial_deposit";
                case AppEventName.FinancialWithdraw: return "financial_withdraw";
                case AppEventName.FinancialTransaction: return "financial_transaction";
                case AppEventName.SubmitApplication: return "submit_application";
                case AppEventName.AccountOpened: return "account_opened";
                case AppEventName.AccountClosed: return "account_closed";
                case AppEventName.GameStart: return "game_start";
                case AppEventName.GameOver: return "game_over";
                case AppEventName.LevelStart: return "level_start";
                case AppEventName.LevelComplete: return "level_complete";
                case AppEventName.LevelFail: return "level_fail";
                case AppEventName.LevelQuit: return "level_quit";
                case AppEventName.LevelSkip: return "level_skip";
                case AppEventName.LevelUp: return "level_up";
                case AppEventName.VirtualResourceTransaction: return "virtual_resource_transaction";
                case AppEventName.EnergyDepleted: return "energy_depleted";
                case AppEventName.UseProp: return "use_prop";
                case AppEventName.GameShopEnter: return "game_shop_enter";
                case AppEventName.StoreItemClick: return "store_item_click";
                case AppEventName.AchievementUnlocked: return "achievement_unlocked";
                case AppEventName.AchievementStep: return "achievement_step";
                case AppEventName.CutsceneStart: return "cutscene_start";
                case AppEventName.CutsceneSkip: return "cutscene_skip";
                case AppEventName.FirstInteraction: return "first_interaction";
                case AppEventName.TutorialStart: return "tutorial_start";
                case AppEventName.TutorialStep: return "tutorial_step";
                case AppEventName.TutorialComplete: return "tutorial_complete";
                case AppEventName.TutorialSkip: return "tutorial_skip";
                case AppEventName.AppOpen: return "app_open";
                case AppEventName.SignUp: return "sign_up";
                case AppEventName.Login: return "login";
                case AppEventName.AccountVerificationStart: return "account_verification_start";
                case AppEventName.AccountVerificationComplete: return "account_verification_complete";
                case AppEventName.AccountLinked: return "account_linked";
                case AppEventName.Search: return "search";
                case AppEventName.GenerateLead: return "generate_lead";
                case AppEventName.ProductReview: return "product_review";
                case AppEventName.FriendInvite: return "friend_invite";
                case AppEventName.SocialShare: return "social_share";
                case AppEventName.SocialAccept: return "social_accept";
                case AppEventName.GiftTransaction: return "gift_transaction";
                case AppEventName.GroupJoined: return "group_joined";
                case AppEventName.ChatSent: return "chat_sent";
                case AppEventName.PostView: return "post_view";
                case AppEventName.PostCreated: return "post_created";
                case AppEventName.Subscribe: return "subscribe";
                case AppEventName.StartTrial: return "start_trial";
                case AppEventName.SubscriptionCanceled: return "subscription_canceled";
                case AppEventName.Order: return "order";
                case AppEventName.Booking: return "booking";
                case AppEventName.BookingConfirmed: return "booking_confirmed";
                case AppEventName.BookingCanceled: return "booking_canceled";
                case AppEventName.OnlineCheckIn: return "online_check_in";
                case AppEventName.PromoCodeApplied: return "promo_code_applied";
                case AppEventName.PushNotificationEnable: return "push_notification_enable";
                case AppEventName.PushNotificationClick: return "push_notification_click";
                case AppEventName.ReEngage: return "re_engage";
                case AppEventName.Update: return "update";
                case AppEventName.CustomerSegment: return "customer_segment";
                case AppEventName.LocationCoordinates: return "location_coordinates";
                default: return name.ToString().ToLowerInvariant();
            }
        }
    }
}
