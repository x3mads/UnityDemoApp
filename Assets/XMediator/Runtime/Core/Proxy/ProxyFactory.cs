using System;
using UnityEngine;

namespace XMediator
{
    internal static class ProxyFactory
    {
        public static T CreateInstance<T>(string className)
        {
            var typeName = GetPlatformSpecificTypeName(className);
            return (T) Activator.CreateInstance(Type.GetType(typeName));;
        }

        private static string GetPlatformSpecificTypeName(string className)
        {
            var platform = GetPlatform();
            var type = $"XMediator.{platform}.{platform}{className},XMediator.{platform}";
            // TODO add type caching to avoid repetitive reflection lookups
            return type;
        }

        private static string GetPlatform()
        {
            if (Application.platform == RuntimePlatform.Android)
                return "Android";
            if (Application.platform == RuntimePlatform.IPhonePlayer)
                return "iOS";
            return "Unity";
        }
    }
}