using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XMediator.Api;

namespace XMediator.Android
{
    internal class CustomPropertiesDto
    {
        private static string CUSTOM_PROPERTIES_DTO_CLASSNAME =
            "com.etermax.android.xmediator.unityproxy.dto.CustomPropertiesDto";
        internal IDictionary<string, object> Properties { get; }

        public static CustomPropertiesDto From(CustomProperties customProperties) =>
            new CustomPropertiesDto(properties: customProperties.GetAll());

        internal CustomPropertiesDto(IDictionary<string, object> properties)
        {
            Properties = properties;
        }

        internal AndroidJavaObject ToAndroidJavaObject()
        {
            using (var hashMap = new AndroidJavaObject("java.util.HashMap"))
            {
                using (var clazz = new AndroidJavaClass(CUSTOM_PROPERTIES_DTO_CLASSNAME))
                {
                    foreach (var entry in Properties)
                    {
                        var key = entry.Key;
                        var value = entry.Value;
                        switch (value)
                        {
                            case int intValue:
                                clazz.CallStatic("putInt", hashMap, key, intValue);
                                break;
                            case double doubleValue:
                                clazz.CallStatic("putDouble", hashMap, key, doubleValue);
                                break;
                            case float floatValue:
                                clazz.CallStatic("putFloat", hashMap, key, floatValue);
                                break;
                            case string stringValue:
                                clazz.CallStatic("putString", hashMap, key, stringValue);
                                break;
                            case bool boolValue:
                                clazz.CallStatic("putBool", hashMap, key, boolValue);
                                break;
                            case IEnumerable<string> stringEnumerableValue:
                                clazz.CallStatic("putStringSet", hashMap, key, stringEnumerableValue.ToArray());
                                break;
                        }
                    }

                    return new AndroidJavaObject(
                        CUSTOM_PROPERTIES_DTO_CLASSNAME,
                        hashMap
                    );
                }
            }
        }
    }
}