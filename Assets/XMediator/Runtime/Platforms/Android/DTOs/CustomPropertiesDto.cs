using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XMediator.Api;

namespace XMediator.Android
{
    internal class CustomPropertiesDto
    {
        private static string CUSTOM_PROPERTIES_DTO_CLASSNAME = "com.etermax.android.xmediator.unityproxy.dto.CustomPropertiesDto";
        internal IDictionary<string, int> IntProperties { get; }
        internal IDictionary<string, float> FloatProperties { get; }
        internal IDictionary<string, double> DoubleProperties { get; }
        internal IDictionary<string, string> StringProperties { get; }
        internal IDictionary<string, bool> BoolProperties { get; }
        internal IDictionary<string, IEnumerable<string>> StringSetProperties { get; }

        public static CustomPropertiesDto From(CustomProperties customProperties) {
            IDictionary<string, int> intProperties = new Dictionary<string, int>();
            IDictionary<string, float> floatProperties = new Dictionary<string, float>();
            IDictionary<string, double> doubleProperties = new Dictionary<string, double>();
            IDictionary<string, string> stringProperties = new Dictionary<string, string>();
            IDictionary<string, bool> boolProperties = new Dictionary<string, bool>();
            IDictionary<string, IEnumerable<string>> stringSetProperties = new Dictionary<string, IEnumerable<string>>();
            if (customProperties != null) {
                foreach (var (key, value) in customProperties.GetAll()) {
                    if (value is int intValue) {
                        intProperties.Add(key, intValue);
                    } else if (value is float floatValue) {
                        floatProperties.Add(key, floatValue);
                    } else if (value is double doubleValue) {
                        doubleProperties.Add(key, doubleValue);
                    } else if (value is string stringValue) {
                        stringProperties.Add(key, stringValue);
                    } else if (value is bool boolValue) {
                        boolProperties.Add(key, boolValue);
                    } else if (value is IEnumerable<string> stringSet) {
                        stringSetProperties.Add(key, stringSet);
                    }
                };
            }
            return new CustomPropertiesDto(
                intProperties,
                floatProperties,
                doubleProperties,
                stringProperties,
                boolProperties,
                stringSetProperties
            );
        }

        public static CustomPropertiesDto From(AndroidJavaObject customProperties) {
            using var intPropertiesJavaObject = customProperties.Call<AndroidJavaObject>("getIntProperties");
            using var floatPropertiesJavaObject = customProperties.Call<AndroidJavaObject>("getFloatProperties");
            using var doublePropertiesJavaObject = customProperties.Call<AndroidJavaObject>("getDoubleProperties");
            using var stringPropertiesJavaObject = customProperties.Call<AndroidJavaObject>("getStringProperties");
            using var boolPropertiesJavaObject = customProperties.Call<AndroidJavaObject>("getBoolProperties");
            using var stringSetPropertiesJavaObject = customProperties.Call<AndroidJavaObject>("getStringSetProperties");
            
            using var inKeySet = intPropertiesJavaObject.Call<AndroidJavaObject>("keySet");
            using var floatKeySet = floatPropertiesJavaObject.Call<AndroidJavaObject>("keySet");
            using var doubleKeySet = doublePropertiesJavaObject.Call<AndroidJavaObject>("keySet");
            using var stringKeySet = stringPropertiesJavaObject.Call<AndroidJavaObject>("keySet");
            using var boolKeySet = boolPropertiesJavaObject.Call<AndroidJavaObject>("keySet");
            using var stringSetKeySet = stringSetPropertiesJavaObject.Call<AndroidJavaObject>("keySet");
            
            var intKeys = inKeySet.Call<string[]>("toArray");
            var floatKeys = floatKeySet.Call<string[]>("toArray");
            var doubleKeys = doubleKeySet.Call<string[]>("toArray");
            var stringKeys = stringKeySet.Call<string[]>("toArray");
            var boolKeys = boolKeySet.Call<string[]>("toArray");
            var stringSetKeys = stringSetKeySet.Call<string[]>("toArray");

            IDictionary<string, int> intProperties = new Dictionary<string, int>();
            IDictionary<string, float> floatProperties = new Dictionary<string, float>();
            IDictionary<string, double> doubleProperties = new Dictionary<string, double>();
            IDictionary<string, string> stringProperties = new Dictionary<string, string>();
            IDictionary<string, bool> boolProperties = new Dictionary<string, bool>();
            IDictionary<string, IEnumerable<string>> stringSetProperties = new Dictionary<string, IEnumerable<string>>();

            foreach (var key in intKeys) {
                intProperties.Add(key, intPropertiesJavaObject.Call<AndroidJavaObject>("get", key).Call<int>("intValue"));
            }

            foreach (var key in floatKeys) {
                floatProperties.Add(key, floatPropertiesJavaObject.Call<AndroidJavaObject>("get", key).Call<float>("floatValue"));
            }

            foreach (var key in doubleKeys) {
                doubleProperties.Add(key, doublePropertiesJavaObject.Call<AndroidJavaObject>("get", key).Call<double>("doubleValue"));
            }

            foreach (var key in stringKeys) {
                stringProperties.Add(key, stringPropertiesJavaObject.Call<string>("get", key));
            }

            foreach (var key in boolKeys) {
                boolProperties.Add(key, boolPropertiesJavaObject.Call<AndroidJavaObject>("get", key).Call<bool>("booleanValue"));
            }

            foreach (var key in stringSetKeys) {
                var stringsArray = stringSetPropertiesJavaObject.Call<string[]>("get", key);
                stringSetProperties.Add(key, stringsArray);
            }

            return new CustomPropertiesDto(
                intProperties,
                floatProperties,
                doubleProperties,
                stringProperties,
                boolProperties,
                stringSetProperties
            );
        }

        internal CustomPropertiesDto(
            IDictionary<string, int> intProperties,
            IDictionary<string, float> floatProperties,
            IDictionary<string, double> doubleProperties,
            IDictionary<string, string> stringProperties,
            IDictionary<string, bool> boolProperties,
            IDictionary<string, IEnumerable<string>> stringSetProperties
        ) {
            this.IntProperties = intProperties;
            this.FloatProperties = floatProperties;
            this.DoubleProperties = doubleProperties;
            this.StringProperties = stringProperties;
            this.BoolProperties = boolProperties;
            this.StringSetProperties = stringSetProperties;
        }

        internal AndroidJavaObject ToAndroidJavaObject()
        {
            using var customPropertiesClass = new AndroidJavaClass(CUSTOM_PROPERTIES_DTO_CLASSNAME);
            using var intProperties = new AndroidJavaObject("java.util.HashMap");
            using var floatProperties = new AndroidJavaObject("java.util.HashMap");
            using var doubleProperties = new AndroidJavaObject("java.util.HashMap");
            using var stringProperties = new AndroidJavaObject("java.util.HashMap");
            using var boolProperties = new AndroidJavaObject("java.util.HashMap");
            using var stringSetProperties = new AndroidJavaObject("java.util.HashMap");
            
            foreach (var entry in IntProperties)
            {
                customPropertiesClass.CallStatic("putInt", intProperties, entry.Key, entry.Value);
            }
            foreach (var entry in FloatProperties)
            {
                customPropertiesClass.CallStatic("putFloat", floatProperties, entry.Key, entry.Value);
            }
            foreach (var entry in DoubleProperties)
            {
                customPropertiesClass.CallStatic("putDouble", doubleProperties, entry.Key, entry.Value);
            }
            foreach (var entry in StringProperties)
            {
                customPropertiesClass.CallStatic("putString", stringProperties, entry.Key, entry.Value);
            }
            foreach (var entry in BoolProperties)
            {
                customPropertiesClass.CallStatic("putBoolean", boolProperties, entry.Key, entry.Value);
            }
            foreach (var entry in StringSetProperties)
            {
                customPropertiesClass.CallStatic("putStringSet", stringSetProperties, entry.Key, entry.Value.ToArray());
            }

            return new AndroidJavaObject(
                CUSTOM_PROPERTIES_DTO_CLASSNAME,
                intProperties,
                floatProperties,
                doubleProperties,
                stringProperties,
                boolProperties,
                stringSetProperties
            );
        }

        internal CustomProperties ToCustomProperties()
        {
            CustomProperties.Builder builder = new CustomProperties.Builder();
            foreach (var entry in IntProperties)
            {
                builder.AddInt(entry.Key, entry.Value);
            }
            foreach (var entry in FloatProperties)
            {
                builder.AddFloat(entry.Key, entry.Value);
            }
            foreach (var entry in DoubleProperties)
            {
                builder.AddDouble(entry.Key, entry.Value);
            }
            foreach (var entry in StringProperties)
            {
                builder.AddString(entry.Key, entry.Value);
            }
            foreach (var entry in BoolProperties)
            {
                builder.AddBoolean(entry.Key, entry.Value);
            }
            foreach (var entry in StringSetProperties)
            {
                builder.AddStringSet(entry.Key, entry.Value);
            }
            return builder.Build();
        }
    }
}