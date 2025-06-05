using System;
using JetBrains.Annotations;
using UnityEngine;
using XMediator.Api;

namespace XMediator.Android
{
    internal class UserPropertiesDto
    {
        internal const string USER_PROPERTIES_DTO_CLASSNAME = "com.etermax.android.xmediator.unityproxy.dto.UserPropertiesDto";
        [CanBeNull] internal string UserId { get; }
        [CanBeNull] private long? InstallDate { get; }
        [CanBeNull] private InAppPurchaseSummaryDto InAppPurchaseSummary { get; }
        [CanBeNull] internal CustomPropertiesDto CustomProperties { get; }

        internal UserPropertiesDto(
            [CanBeNull] string userId,
            [CanBeNull] long? installDate,
            [CanBeNull] InAppPurchaseSummaryDto inAppPurchaseSummary,
            [CanBeNull] CustomPropertiesDto customProperties
        )
        {
            UserId = userId;
            InstallDate = installDate;
            InAppPurchaseSummary = inAppPurchaseSummary;
            CustomProperties = customProperties;
        }

        internal static UserPropertiesDto From(UserProperties userProperties)
        {
            return new UserPropertiesDto(
                userId: userProperties.UserId,
                installDate: userProperties.InstallDate?.ToUnixTimeMilliseconds(),
                inAppPurchaseSummary: userProperties.InAppPurchaseSummary == null ? null : InAppPurchaseSummaryDto.From(userProperties.InAppPurchaseSummary),
                customProperties: CustomPropertiesDto.From(userProperties.CustomProperties)
            );
        }

        internal static UserPropertiesDto From(AndroidJavaObject userPropertiesJavaObject)
        {
            return new UserPropertiesDto(
                userId: userPropertiesJavaObject.Call<string>("getUserId"),
                installDate: Utils.LongFromAndroidJavaObject(userPropertiesJavaObject.Call<AndroidJavaObject>("getInstallDate")),
                inAppPurchaseSummary: InAppPurchaseSummaryDto.From(userPropertiesJavaObject.Call<AndroidJavaObject>("getInAppPurchaseSummary")),
                customProperties: CustomPropertiesDto.From(userPropertiesJavaObject.Call<AndroidJavaObject>("getCustomProperties"))
            );
        }

        [CanBeNull]
        internal static UserPropertiesDto FromNullable([CanBeNull] UserProperties userProperties)
        {
            return userProperties == null ? null : From(userProperties);
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
            return Equals((UserPropertiesDto)obj);
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
            using (var customPropertiesJavaObject = CustomProperties?.ToAndroidJavaObject())
            {
                using (var inAppPurchaseSummaryJavaObject = InAppPurchaseSummary?.ToAndroidJavaObject())
                {
                    return new AndroidJavaObject(
                        USER_PROPERTIES_DTO_CLASSNAME,
                        UserId,
                        Utils.ToAndroidLong(InstallDate),
                        inAppPurchaseSummaryJavaObject,
                        customPropertiesJavaObject
                    );
                }
            }
        }

        internal UserProperties ToUserProperties()
        {
            return new UserProperties(
                userId: UserId,
                installDate: InstallDate == null ? null : DateTimeOffset.FromUnixTimeMilliseconds(InstallDate.Value),
                inAppPurchaseSummary: InAppPurchaseSummary?.ToInAppPurchaseSummary(),
                customProperties: CustomProperties?.ToCustomProperties()
            );
        }
        
    }
}