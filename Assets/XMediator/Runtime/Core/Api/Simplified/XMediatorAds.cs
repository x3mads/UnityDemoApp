using System;
using UnityEngine;
using XMediator.Core.Util;

namespace XMediator.Api
{
    /// <summary>
    /// XMediator is an ad mediation solution that enables integrating your app or game with many advertising partners.
    /// 
    /// Just initialize the SDK and start showing ads.
    /// 
    /// <seealso cref="XMediatorAds.Banner"/>
    /// <seealso cref="XMediatorAds.Interstitial"/>
    /// <seealso cref="XMediatorAds.Rewarded"/>
    /// <seealso cref="XMediatorAds.AppOpen"/>
    /// <seealso cref="XMediatorAds.CMPProvider"/>
    /// <seealso cref="XMediatorAds.EventTracker"/>
    /// </summary>
    public static class XMediatorAds
    {
        /// <summary>
        /// Entry point for displaying interstitial ads.
        /// </summary>
        public static readonly InterstitialAds Interstitial = new InterstitialAds();
        
        /// <summary>
        /// Entry point for displaying rewarded ads.
        /// </summary>
        public static readonly RewardedAds Rewarded = new RewardedAds();
        
        /// <summary>
        /// Entry point for displaying banner ads.
        /// </summary>
        public static readonly BannerAds Banner = new BannerAds();
        
        /// <summary>
        /// Entry point for displaying app open ads.
        /// </summary>
        public static readonly AppOpenAds AppOpen = new AppOpenAds();
        
        /// <summary>
        /// Entry point for interacting with the CMP provider.
        /// </summary>
        public static readonly CMPProviderService CMPProvider = new CMPProviderService();
        
        /// <summary>
        /// Track events such as purchases for analytics and segmentation purposes.
        /// 
        /// - Note: The <see cref="EventTracker.Track"/>  method should only be called after the SDK is properly initialized. If [EventTracker.track] is called before SDK initialization, events will be ignored.
        /// </summary>
        public static readonly EventTracker EventTracker = new EventTracker();
        
        /// <summary>
        /// Initializes XMediator.
        /// </summary>
        /// <param name="appKey">Your app or game app key. This is mandatory and cannot be null.</param>
        /// <param name="initSettings">A <see cref="InitSettings"/> object.</param>
        /// <param name="initCallback">An action to execute once the core of the SDK has been initialized.</param>
        public static void StartWith(
            string appKey,
            InitSettings initSettings = null,
            Action<InitResult> initCallback = null
        )
        {
            XMediatorMainThreadDispatcherFactory.Create();
            var xMediatorAdsProxy = ProxyFactory.CreateInstance<XMediatorAdsProxy>("XMediatorAdsProxy");
            xMediatorAdsProxy.StartWith(
                appKey: appKey,
                unityVersion: Application.unityVersion,
                initSettings: initSettings ?? new InitSettings(),
                initCallback: initCallback ?? (result => { })
            );
        }

        /// <summary>
        /// Sets or updates consent information.
        /// This will replace all of the values previously set.
        /// </summary>
        /// <param name="consentInformation">A <see cref="ConsentInformation"/> object containing the updated information.</param>
        public static void SetConsentInformation(ConsentInformation consentInformation)
        {
            var xMediatorAdsProxy = ProxyFactory.CreateInstance<XMediatorAdsProxy>("XMediatorAdsProxy");
            xMediatorAdsProxy.SetConsentInformation(consentInformation);
        }

        /// <summary>
        /// Gets the current user properties.
        /// </summary>
        /// <returns>A <see cref="UserProperties"/> object containing the current user properties.</returns>
        public static UserProperties GetUserProperties()
        {
            var xMediatorAdsProxy = ProxyFactory.CreateInstance<XMediatorAdsProxy>("XMediatorAdsProxy");
            return xMediatorAdsProxy.GetUserProperties();
        }

        /// <summary>
        /// Sets or updates user properties.
        /// This will replace all of the properties previously set.
        /// </summary>
        /// <param name="userProperties">A <see cref="UserProperties"/> object containing the updated properties.</param>
        public static void SetUserProperties(UserProperties userProperties)
        {
            var xMediatorAdsProxy = ProxyFactory.CreateInstance<XMediatorAdsProxy>("XMediatorAdsProxy");
            xMediatorAdsProxy.SetUserProperties(userProperties);
        }
        
        /// <summary>
        /// Indicates if the app should be paused if an ad ad is being presented.
        /// </summary>
        /// <remarks>
        /// This method only works for iOS, as in Android the app is always paused when showing an ad. 
        /// </remarks>
        /// <param name="shouldPause">
        /// true if the app should be paused when a fullscreen ad is being presented. Default is false.
        /// </param>
        public static void SetPauseOnAdPresentation(bool shouldPause)
        {
            var xMediatorAdsProxy = ProxyFactory.CreateInstance<XMediatorAdsProxy>("XMediatorAdsProxy");
            xMediatorAdsProxy.SetPauseOnAdPresentation(shouldPause);
        }

        /// <summary>
        /// Opens XMediator Debugging Suite
        /// </summary>
        public static void OpenDebuggingSuite()
        {
            var xMediatorAdsProxy = ProxyFactory.CreateInstance<XMediatorAdsProxy>("XMediatorAdsProxy");
            xMediatorAdsProxy.OpenDebuggerSuite();
        }

        /// <summary>
        /// Indicates whether `XMediatorSdk` has already completed its initialization sequence.
        /// </summary>
        /// <returns>True if the SDK finished its initialization with a successful result, `false` otherwise.</returns>
        public static bool IsInitialized()
        {
            var xMediatorAdsProxy = ProxyFactory.CreateInstance<XMediatorAdsProxy>("XMediatorAdsProxy");
            return xMediatorAdsProxy.IsInitialized();
        }
    }
}