using System;
using System.Collections.Generic;
using UnityEngine;


namespace XMediator.Android
{
    internal class PropertiesMapDto
    {
        private static string PROPERTIES_MAP_DTO_CLASSNAME = "com.etermax.android.xmediator.unityproxy.dto.PropertiesMapDto";
        internal IDictionary<string, int> IntProperties { get; }
        internal IDictionary<string, long> LongProperties { get; }
        internal IDictionary<string, float> FloatProperties { get; }
        internal IDictionary<string, double> DoubleProperties { get; }
        internal IDictionary<string, string> StringProperties { get; }
        internal IDictionary<string, bool> BoolProperties { get; }
        internal IDictionary<string, List<string>> StringSetProperties { get; }

        internal PropertiesMapDto(
            IDictionary<string, int> intProperties,
            IDictionary<string, long> longProperties,
            IDictionary<string, float> floatProperties,
            IDictionary<string, double> doubleProperties,
            IDictionary<string, string> stringProperties,
            IDictionary<string, bool> boolProperties,
            IDictionary<string, List<string>> stringSetProperties
        ) {
            this.IntProperties = intProperties;
            this.LongProperties = longProperties;
            this.FloatProperties = floatProperties;
            this.DoubleProperties = doubleProperties;
            this.StringProperties = stringProperties;
            this.BoolProperties = boolProperties;
            this.StringSetProperties = stringSetProperties;
        }

        public static PropertiesMapDto From(AndroidJavaObject propertiesMap) {
            using var intPropertiesJavaObject = propertiesMap.Call<AndroidJavaObject>("getIntProperties");
            using var longPropertiesJavaObject = propertiesMap.Call<AndroidJavaObject>("getLongProperties");
            using var floatPropertiesJavaObject = propertiesMap.Call<AndroidJavaObject>("getFloatProperties");
            using var doublePropertiesJavaObject = propertiesMap.Call<AndroidJavaObject>("getDoubleProperties");
            using var stringPropertiesJavaObject = propertiesMap.Call<AndroidJavaObject>("getStringProperties");
            using var boolPropertiesJavaObject = propertiesMap.Call<AndroidJavaObject>("getBoolProperties");
            using var stringSetPropertiesJavaObject = propertiesMap.Call<AndroidJavaObject>("getStringSetProperties");
            
            using var inKeySet = intPropertiesJavaObject.Call<AndroidJavaObject>("keySet");
            using var longKeySet = longPropertiesJavaObject.Call<AndroidJavaObject>("keySet");
            using var floatKeySet = floatPropertiesJavaObject.Call<AndroidJavaObject>("keySet");
            using var doubleKeySet = doublePropertiesJavaObject.Call<AndroidJavaObject>("keySet");
            using var stringKeySet = stringPropertiesJavaObject.Call<AndroidJavaObject>("keySet");
            using var boolKeySet = boolPropertiesJavaObject.Call<AndroidJavaObject>("keySet");
            using var stringSetKeySet = stringSetPropertiesJavaObject.Call<AndroidJavaObject>("keySet");
            
            var intKeys = inKeySet.Call<string[]>("toArray");
            var longKeys = longKeySet.Call<string[]>("toArray");
            var floatKeys = floatKeySet.Call<string[]>("toArray");
            var doubleKeys = doubleKeySet.Call<string[]>("toArray");
            var stringKeys = stringKeySet.Call<string[]>("toArray");
            var boolKeys = boolKeySet.Call<string[]>("toArray");
            var stringSetKeys = stringSetKeySet.Call<string[]>("toArray");

            IDictionary<string, int> intProperties = new Dictionary<string, int>();
            IDictionary<string, long> longProperties = new Dictionary<string, long>();
            IDictionary<string, float> floatProperties = new Dictionary<string, float>();
            IDictionary<string, double> doubleProperties = new Dictionary<string, double>();
            IDictionary<string, string> stringProperties = new Dictionary<string, string>();
            IDictionary<string, bool> boolProperties = new Dictionary<string, bool>();
            IDictionary<string, List<string>> stringSetProperties = new Dictionary<string, List<string>>();

            foreach (var key in intKeys) {
                intProperties.Add(key, intPropertiesJavaObject.Call<AndroidJavaObject>("get", key).Call<int>("intValue"));
            }

            foreach (var key in longKeys) {
                longProperties.Add(key, longPropertiesJavaObject.Call<AndroidJavaObject>("get", key).Call<long>("longValue"));
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
                stringSetProperties.Add(key, new List<string>(stringsArray));
            }

            return new PropertiesMapDto(
                intProperties,
                longProperties,
                floatProperties,
                doubleProperties,
                stringProperties,
                boolProperties,
                stringSetProperties
            );
        }

        public IDictionary<string, object> ToDictionary() {
            var dictionary = new Dictionary<string, object>();
            foreach (var entry in IntProperties) {
                dictionary.Add(entry.Key, entry.Value);
            }
            foreach (var entry in LongProperties) {
                dictionary.Add(entry.Key, entry.Value);
            }
            foreach (var entry in FloatProperties) {
                dictionary.Add(entry.Key, entry.Value);
            }
            foreach (var entry in DoubleProperties) {
                dictionary.Add(entry.Key, entry.Value);
            }
            foreach (var entry in StringProperties) {
                dictionary.Add(entry.Key, entry.Value);
            }
            foreach (var entry in BoolProperties) {
                dictionary.Add(entry.Key, entry.Value);
            }
            foreach (var entry in StringSetProperties) {
                dictionary.Add(entry.Key, entry.Value);
            }
            return dictionary;
        }
    }
}