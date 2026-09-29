using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace XMediator.Api
{
    /// <summary>
    /// Entry point for managing user properties.
    ///
    /// Access it through <see cref="XMediatorAds.UserProperties"/>:
    /// <code>
    /// XMediatorAds.UserProperties.SetUserId("user-123");
    /// XMediatorAds.UserProperties.SetCustomProperty("level", 5);
    /// </code>
    /// </summary>
    public class UserPropertiesService
    {
        private static readonly XMediatorAdsProxy Proxy = ProxyFactory.CreateInstance<XMediatorAdsProxy>("XMediatorAdsProxy");

        internal UserPropertiesService()
        {
        }

        /// <summary>
        /// Gets the current user properties.
        /// </summary>
        /// <returns>A <see cref="UserProperties"/> object containing the current user properties.</returns>
        public UserProperties Get()
        {
            return Proxy.GetUserProperties();
        }

        /// <summary>
        /// Sets or updates the unique user ID.
        /// </summary>
        /// <param name="userId">An ID that uniquely identifies a user in your app, or null to clear it.</param>
        public void SetUserId([CanBeNull] string userId)
        {
            Proxy.SetUserId(userId);
        }

        /// <summary>
        /// Sets or updates the app install date.
        /// </summary>
        /// <param name="installDate">The date the user installed the app, or null to clear it.</param>
        public void SetInstallDate(DateTimeOffset? installDate)
        {
            Proxy.SetInstallDate(installDate);
        }

        /// <summary>
        /// Sets or updates the in-app purchase summary.
        /// </summary>
        /// <param name="purchaseSummary">The user's in-app purchase summary, or null to clear it.</param>
        public void SetPurchaseSummary([CanBeNull] InAppPurchaseSummary purchaseSummary)
        {
            Proxy.SetPurchaseSummary(purchaseSummary);
        }

        /// <summary>
        /// Sets or updates a boolean custom property.
        /// </summary>
        /// <param name="key">The key of the custom property.</param>
        /// <param name="value">The value of the custom property.</param>
        public void SetCustomProperty(string key, bool value)
        {
            Proxy.SetCustomProperty(key, value);
        }

        /// <summary>
        /// Sets or updates an integer custom property.
        /// </summary>
        /// <param name="key">The key of the custom property.</param>
        /// <param name="value">The value of the custom property.</param>
        public void SetCustomProperty(string key, int value)
        {
            Proxy.SetCustomProperty(key, value);
        }

        /// <summary>
        /// Sets or updates a double custom property.
        /// </summary>
        /// <param name="key">The key of the custom property.</param>
        /// <param name="value">The value of the custom property.</param>
        public void SetCustomProperty(string key, double value)
        {
            Proxy.SetCustomProperty(key, value);
        }

        /// <summary>
        /// Sets or updates a string custom property.
        /// </summary>
        /// <param name="key">The key of the custom property.</param>
        /// <param name="value">The value of the custom property.</param>
        public void SetCustomProperty(string key, string value)
        {
            Proxy.SetCustomProperty(key, value);
        }

        /// <summary>
        /// Sets or updates a string set custom property.
        /// </summary>
        /// <param name="key">The key of the custom property.</param>
        /// <param name="value">The value of the custom property.</param>
        public void SetCustomProperty(string key, IEnumerable<string> value)
        {
            Proxy.SetCustomProperty(key, value);
        }

        /// <summary>
        /// Removes a custom property by key.
        /// </summary>
        /// <param name="key">The key of the custom property to remove.</param>
        public void RemoveCustomProperty(string key)
        {
            Proxy.RemoveCustomProperty(key);
        }

        /// <summary>
        /// Clears all custom properties while preserving user ID, install date, and purchase summary.
        /// </summary>
        public void ClearCustomProperties()
        {
            Proxy.ClearCustomProperties();
        }

        /// <summary>
        /// Clears all user properties (user ID, install date, purchase summary, and custom properties).
        /// </summary>
        public void Clear()
        {
            Proxy.ClearUserProperties();
        }
    }
}
