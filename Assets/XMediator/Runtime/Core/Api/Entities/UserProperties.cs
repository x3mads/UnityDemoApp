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
        /// The current user's custom properties
        /// </summary>
        public CustomProperties CustomProperties { get; }

        /// <summary>
        /// Creates a new <see cref="UserProperties"/> instance with the provided values.
        /// </summary>
        /// <param name="userId">An Id that uniquely identifies a user on your app. This value is optional and not mandatory.</param>
        /// <param name="customProperties">Custom properties object with additional properties you can provide.</param>
        public UserProperties([CanBeNull] string userId = null, CustomProperties customProperties = null)
        {
            UserId = userId;
            CustomProperties = customProperties ?? new CustomProperties.Builder().Build();
        }

        protected bool Equals(UserProperties other)
        {
            return UserId == other.UserId && Equals(CustomProperties, other.CustomProperties);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UserProperties) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((UserId != null ? UserId.GetHashCode() : 0) * 397) ^
                       (CustomProperties != null ? CustomProperties.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return $"{nameof(UserId)}: {UserId}, {nameof(CustomProperties)}: {CustomProperties}";
        }
        
        /// <summary>
        /// Creates a new <see cref="UserProperties"/> instance with the current values as defaults, but overrideable.
        /// </summary>
        /// <param name="userId">The user id. If null, the current value of <see cref="UserId"/> will be used.</param>
        /// <param name="customPropertiesEditAction">An action which will be invoked with a <see cref="CustomProperties.Builder"/> instance,
        /// allowing the caller to modify the custom properties. If null, the properties will be left unchanged.</param>
        /// <returns>A new <see cref="UserProperties"/> with the modified properties.</returns>
        /// <remarks>
        /// This convenience method does not allow to null the <see cref="UserId"/>. If that is desired, you should create
        /// a new instance instead (just don't forget to set again the previous properties that you do not want to lose).
        /// </remarks>
        public UserProperties CopyWith([CanBeNull] string userId = null,
            [CanBeNull] Action<CustomProperties.Builder> customPropertiesEditAction = null)
        {
            var editor = CustomProperties.NewBuilder();
            customPropertiesEditAction?.Invoke(editor);
            return new UserProperties(userId: userId ?? this.UserId, editor.Build());
        }
    }
}