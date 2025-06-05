using System;
using JetBrains.Annotations;

namespace XMediator.Api
{
    public class UserProperties
    {
        /// <summary>
        /// The current user's unique identifier.
        /// </summary>
        [CanBeNull] public string UserId { get; }

        /// <summary>
        /// The date the user installed the app, represented as a <see cref="DateTimeOffset"/>.
        /// </summary>
        [CanBeNull] public DateTimeOffset? InstallDate { get; }

        /// <summary>
        /// The user's in-app purchase summary.
        /// </summary>
        [CanBeNull] public InAppPurchaseSummary InAppPurchaseSummary { get; }

        /// <summary>
        /// The current user's custom properties
        /// </summary>
        public CustomProperties CustomProperties { get; }

        /// <summary>
        /// Creates a new <see cref="UserProperties"/> instance with the provided values.
        /// </summary>
        /// <param name="userId">An Id that uniquely identifies a user on your app. This value is optional and not mandatory.</param>
        /// <param name="customProperties">Custom properties object with additional properties you can provide.</param>
        /// <param name="installDate">The date the user installed the app, represented as a <see cref="DateTimeOffset"/>. This value is optional and not mandatory.</param>
        /// <param name="inAppPurchaseSummary">The user's in-app purchase summary. This value is optional and not mandatory.</param>
        public UserProperties(
            [CanBeNull] string userId = null,
            [CanBeNull] CustomProperties customProperties = null,
            [CanBeNull] DateTimeOffset? installDate = null,
            [CanBeNull] InAppPurchaseSummary inAppPurchaseSummary = null)
        {
            UserId = userId;
            InstallDate = installDate;
            InAppPurchaseSummary = inAppPurchaseSummary;
            CustomProperties = customProperties ?? new CustomProperties.Builder().Build();
        }

        private bool Equals(UserProperties other)
        {
            return UserId == other.UserId && Nullable.Equals(InstallDate, other.InstallDate) && Equals(InAppPurchaseSummary, other.InAppPurchaseSummary) && Equals(CustomProperties, other.CustomProperties);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((UserProperties) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(UserId, InstallDate, InAppPurchaseSummary, CustomProperties);
        }

        public override string ToString()
        {
            return $"{nameof(UserId)}: {UserId}, {nameof(InstallDate)}: {InstallDate}, {nameof(InAppPurchaseSummary)}: {InAppPurchaseSummary}, {nameof(CustomProperties)}: {CustomProperties}";
        }

        /// <summary>
        /// Creates a new <see cref="UserProperties"/> instance with the current values as defaults, but overrideable.
        /// </summary>
        /// <param name="userId">The user id. If null, the current value of <see cref="UserId"/> will be used.</param>
        /// <param name="installDate">The install date. If null, the current value of <see cref="InstallDate"/> will be used.</param>
        /// <param name="inAppPurchaseSummary">The in-app purchase summary. If null, the current value of <see cref="InAppPurchaseSummary"/> will be used.</param>
        /// <param name="customPropertiesEditAction">An action which will be invoked with a <see cref="CustomProperties.Builder"/> instance,
        /// allowing the caller to modify the custom properties. If null, the properties will be left unchanged.</param>
        /// <returns>A new <see cref="UserProperties"/> with the modified properties.</returns>
        /// <remarks>
        /// This convenience method does not allow to null the <see cref="UserId"/>. If that is desired, you should create
        /// a new instance instead (just don't forget to set again the previous properties that you do not want to lose).
        /// </remarks>
        public UserProperties CopyWith(
            [CanBeNull] string userId = null,
            [CanBeNull] DateTimeOffset? installDate = null,
            [CanBeNull] InAppPurchaseSummary inAppPurchaseSummary = null,
            [CanBeNull] Action<CustomProperties.Builder> customPropertiesEditAction = null)
        {
            var editor = CustomProperties.NewBuilder();
            customPropertiesEditAction?.Invoke(editor);
            return new UserProperties(
                userId: userId ?? this.UserId,
                customProperties: editor.Build(),
                installDate: installDate ?? this.InstallDate,
                inAppPurchaseSummary: inAppPurchaseSummary ?? this.InAppPurchaseSummary);
        }
    }
}