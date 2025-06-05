using JetBrains.Annotations;
using UnityEngine;

namespace XMediator.Android
{
    internal static class Utils
    {

        [CanBeNull]
        internal static AndroidJavaObject ToAndroidDouble(decimal? value)
        {
            return value == null ? null : new AndroidJavaObject("java.lang.Double", (double) value);
        }
        
        [CanBeNull]
        internal static AndroidJavaObject ToAndroidInt(int? value)
        {
            return value == null ? null : new AndroidJavaObject("java.lang.Integer", value);
        }
        
        [CanBeNull]
        internal static AndroidJavaObject ToAndroidLong(long? value)
        {
            return value == null ? null : new AndroidJavaObject("java.lang.Long", value);
        }
        
        internal static int? IntFromAndroidJavaObject([CanBeNull] AndroidJavaObject javaObject)
        {
            if (javaObject == null) return null;
            using (javaObject)
            {
                return javaObject.Call<int>("intValue");
            }
        }

        internal static long? LongFromAndroidJavaObject([CanBeNull] AndroidJavaObject javaObject)
        {
            if (javaObject == null) return null;
            using (javaObject)
            {
                return javaObject.Call<long>("longValue");
            }
        }

        internal static decimal? DecimalFromAndroidJavaObject([CanBeNull] AndroidJavaObject javaObject)
        {
            if (javaObject == null) return null;
            using (javaObject)
            {
                return (decimal) javaObject.Call<double>("doubleValue");
            }
        }
    }
}