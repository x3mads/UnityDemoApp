using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using XMediator.Api;

namespace XMediator.iOS
{
    [Serializable]
    internal class UserPropertiesDto
    {
        [SerializeField] internal bool hasValue;
        [SerializeField] internal string userId;
        [SerializeField] internal NullableLongValue installDate;
        [SerializeField] internal NullableObject<InAppPurchaseSummaryDto> purchaseSummary;
        [SerializeField] internal CustomPropertiesDto customProperties;

        private UserPropertiesDto(bool hasValue, string userId, NullableLongValue installDate,
            NullableObject<InAppPurchaseSummaryDto> purchaseSummary,
            CustomPropertiesDto customPropertiesDto)
        {
            this.hasValue = hasValue;
            this.userId = userId;
            this.installDate = installDate;
            this.purchaseSummary = purchaseSummary;
            customProperties = customPropertiesDto;
        }

        internal static UserPropertiesDto FromUserProperties([CanBeNull] UserProperties userProperties)
        {
            if (userProperties == null)
            {
                return new UserPropertiesDto(false, null, null, null, null);
            }
            
            return new UserPropertiesDto(true, userProperties.UserId,
                new NullableLongValue(userProperties.InstallDate?.ToUnixTimeSeconds()),
                new NullableObject<InAppPurchaseSummaryDto>(InAppPurchaseSummaryDto.FromInAppPurchaseSummary(userProperties.InAppPurchaseSummary)),
                CustomPropertiesDto.FromCustomProperties(userProperties.CustomProperties));
        }
        
        internal string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        public UserProperties ToUserProperties()
        {
            if (!hasValue)
            {
                return new UserProperties();
            }
            
            DateTimeOffset? dateTimeOffset = null;
            if (installDate?.GetValue() is { } value)
            {
                dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(value);
            }

            return new UserProperties(
                userId: userId,
                customProperties: customProperties.ToCustomProperties(),
                installDate: dateTimeOffset,
                inAppPurchaseSummary: purchaseSummary?.GetValue()?.ToInAppPurchaseSummary());
        }
    }
    
    [Serializable]
    internal class CustomPropertiesDto
    {
        [SerializeField] internal List<CustomBoolPropertyDto> bools;
        [SerializeField] internal List<CustomIntPropertyDto> ints;
        [SerializeField] internal List<CustomFloatPropertyDto> floats;
        [SerializeField] internal List<CustomDoublePropertyDto> doubles;
        [SerializeField] internal List<CustomStringPropertyDto> strings;
        [SerializeField] internal List<CustomStringListPropertyDto> stringLists;

        public CustomPropertiesDto(IDictionary<string, object> customProperties)
        {
            bools = new List<CustomBoolPropertyDto>();
            ints = new List<CustomIntPropertyDto>();
            floats = new List<CustomFloatPropertyDto>();
            doubles = new List<CustomDoublePropertyDto>();
            strings = new List<CustomStringPropertyDto>();
            stringLists = new List<CustomStringListPropertyDto>();
            
            foreach (var keyValue in customProperties)
            {
                switch (keyValue.Value)
                {
                    case int intValue:
                        ints.Add(new CustomIntPropertyDto(keyValue.Key, intValue));
                        break;
                    case double doubleValue:
                        doubles.Add(new CustomDoublePropertyDto(keyValue.Key, doubleValue));
                        break;
                    case float floatValue:
                        floats.Add(new CustomFloatPropertyDto(keyValue.Key, floatValue));
                        break;
                    case string stringValue:
                        strings.Add(new CustomStringPropertyDto(keyValue.Key, stringValue));
                        break;
                    case bool boolValue:
                        bools.Add(new CustomBoolPropertyDto(keyValue.Key, boolValue));
                        break;
                    case IEnumerable<string> stringEnumerableValue:
                        stringLists.Add(new CustomStringListPropertyDto(keyValue.Key, stringEnumerableValue.ToList()));
                        break;
                }
            }
        }

        internal static CustomPropertiesDto FromCustomProperties(CustomProperties customProperties)
        {
            return new CustomPropertiesDto(customProperties.GetAll());
        }
        
        internal string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        public CustomProperties ToCustomProperties()
        {
            var properties = new Dictionary<string, object>();
            
            bools?.ForEach(p => properties[p.k] = p.v);
            ints?.ForEach(p => properties[p.k] = p.v);
            floats?.ForEach(p => properties[p.k] = p.v);
            doubles?.ForEach(p => properties[p.k] = p.v);
            strings?.ForEach(p => properties[p.k] = p.v);
            stringLists?.ForEach(p => properties[p.k] = p.v);

            return new CustomProperties(properties);
        }
    }

    [Serializable]
    internal class CustomPropertyDto<T>
    {
        [SerializeField] internal string k;
        [SerializeField] internal T v;

        public CustomPropertyDto(string key, T value)
        {
            this.k = key;
            this.v = value;
        }
    }
    
    [Serializable]
    internal class CustomIntPropertyDto: CustomPropertyDto<int>
    {
        public CustomIntPropertyDto(string key, int value) : base(key, value) { }
    }
    
    [Serializable]
    internal class CustomBoolPropertyDto: CustomPropertyDto<bool>
    {
        public CustomBoolPropertyDto(string key, bool value) : base(key, value) { }
    }
    
    [Serializable]
    internal class CustomFloatPropertyDto: CustomPropertyDto<float>
    {
        public CustomFloatPropertyDto(string key, float value) : base(key, value) { }
    }
    
    [Serializable]
    internal class CustomDoublePropertyDto: CustomPropertyDto<double>
    {
        public CustomDoublePropertyDto(string key, double value) : base(key, value) { }
    }
    
    [Serializable]
    internal class CustomStringPropertyDto: CustomPropertyDto<string>
    {
        public CustomStringPropertyDto(string key, string value) : base(key, value) { }
    }
    
    [Serializable]
    internal class CustomStringListPropertyDto: CustomPropertyDto<List<string>>
    {
        public CustomStringListPropertyDto(string key, List<string> value) : base(key, value) { }
    }
}