using System;
using UnityEngine;
using XMediator.Core.Util;

namespace XMediator.Api
{
    /// <summary>
    /// XMediator is an ad mediation solution that enables integrating your app or game with many advertising partners.
    /// Just initialize the SDK and start showing ads.
    /// <seealso cref="Banner"/>
    /// <seealso cref="Interstitial"/>
    /// <seealso cref="Rewarded"/>
    /// </summary>
    [Obsolete("XMediatorSdk class is deprecated. Please use XMediatorAds instead.")]
    public static class XMediatorSdk
    {
        
        /// <summary>
        /// Initializes XMediator Sdk.
        /// </summary>
        /// <param name="appKey">Your app or game app key. This is mandatory and cannot be null.</param>
        /// <param name="initSettings">A <see cref="InitSettings"/> object.</param>
        /// <param name="initCallback">An action to execute once the core of the SDK has been initialized.</param>
        /// <param name="mediationCallback">An action to execute once the mediation partners of the SDK have been initialized.</param>
        [Obsolete("XMediatorSdk.Initialize is deprecated. Please use XMediatorAds.StartWith instead.")]
        public static void Initialize(
            string appKey,
            InitSettings initSettings = null,
            Action<InitResult> initCallback = null,
            Action<MediationResult> mediationCallback = null
        )
        {
            XMediatorMainThreadDispatcherFactory.Create();
            var xMediatorSdkProxy = ProxyFactory.CreateInstance<XMediatorSdkProxy>("XMediatorSdkProxy");
            xMediatorSdkProxy.Initialize(
                appKey: appKey,
                unityVersion: Application.unityVersion,
                initSettings: initSettings ?? new InitSettings(),
                initCallback: initCallback ?? (result => { }),
                mediationCallback: mediationCallback ?? (result => { })
            );
        }

        /// <summary>
        /// Sets or updates user properties.
        /// This will replace all of the properties previously set.
        /// </summary>
        /// <param name="userProperties">A <see cref="UserProperties"/> object containing the updated properties.</param>
        public static void SetUserProperties(UserProperties userProperties)
        {
            var xMediatorSdkProxy = ProxyFactory.CreateInstance<XMediatorSdkProxy>("XMediatorSdkProxy");
            xMediatorSdkProxy.SetUserProperties(userProperties);
        }

        /// <summary>
        /// Sets or updates consent information.
        /// This will replace all of the values previously set.
        /// </summary>
        /// <param name="consentInformation">A <see cref="ConsentInformation"/> object containing the updated information.</param>
        public static void SetConsentInformation(ConsentInformation consentInformation)
        {
            var xMediatorSdkProxy = ProxyFactory.CreateInstance<XMediatorSdkProxy>("XMediatorSdkProxy");
            xMediatorSdkProxy.SetConsentInformation(consentInformation);
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
            var xMediatorSdkProxy = ProxyFactory.CreateInstance<XMediatorSdkProxy>("XMediatorSdkProxy");
            xMediatorSdkProxy.SetPauseOnAdPresentation(shouldPause);
        }

        /// <summary>
        /// Opens XMediator Debugging Suite
        /// </summary>
        public static void OpenDebuggingSuite()
        {
            var xMediatorSdkProxy = ProxyFactory.CreateInstance<XMediatorSdkProxy>("XMediatorSdkProxy");
            xMediatorSdkProxy.OpenDebuggerSuite();
        }
    }
}