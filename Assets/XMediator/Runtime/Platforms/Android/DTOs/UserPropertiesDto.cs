using JetBrains.Annotations;
using UnityEngine;
using XMediator.Api;

namespace XMediator.Android
{
    internal class UserPropertiesDto
    {
        internal const string USER_PROPERTIES_DTO_CLASSNAME = "com.etermax.android.xmediator.unityproxy.dto.UserPropertiesDto";
        [CanBeNull] internal string UserId { get; }
        [CanBeNull] internal CustomPropertiesDto CustomProperties { get; }

        internal UserPropertiesDto([CanBeNull] string userId, CustomPropertiesDto customProperties)
        {
            UserId = userId;
            CustomProperties = customProperties;
        }

        internal static UserPropertiesDto From(UserProperties userProperties)
        {
            return new UserPropertiesDto(
                userId: userProperties.UserId,
                customProperties: CustomPropertiesDto.From(userProperties.CustomProperties)
            );
        }

        protected bool Equals(UserPropertiesDto other)
        {
            return UserId == other.UserId && Equals(CustomProperties, other.CustomProperties);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UserPropertiesDto) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((UserId != null ? UserId.GetHashCode() : 0) * 397) ^ (CustomProperties != null ? CustomProperties.GetHashCode() : 0);
            }
        }

        internal AndroidJavaObject ToAndroidJavaObject()
        {
            using (var customPropertiesJavaObject = CustomProperties.ToAndroidJavaObject())
            {
                return new AndroidJavaObject(
                    USER_PROPERTIES_DTO_CLASSNAME,
                    UserId,
                    customPropertiesJavaObject
                );
            }
        }
    }
}