using JetBrains.Annotations;
using UnityEngine;
using XMediator.Api;

namespace XMediator.Android
{
    public class CMPDebugSettingDto
    {
        private static string CMP_DEBUG_SETTING_DTO_CLASSNAME =
            "com.etermax.android.xmediator.unityproxy.dto.CMPDebugSettingDto";
        private static string JAVA_LANG_INTEGER_CLASSNAME = "java.lang.Integer";

        private static int GEOGRAPHY_DISABLED = 0;
        private static int GEOGRAPHY_EEA = 1;
        private static int GEOGRAPHY_NOT_EEA = 2;
        
        internal int? CMPDebugGeography { get; }

        internal CMPDebugSettingDto(int? cmpDebugGeography = null)
        {
            CMPDebugGeography = cmpDebugGeography;
        }
        
        [CanBeNull]
        public static CMPDebugSettingDto FromNullable([CanBeNull] CMPDebugSettings cmpDebugSettings)
        {
            if (cmpDebugSettings == null) return null;
            return new CMPDebugSettingDto(
                cmpDebugGeography: GetCmpDebugGeography(cmpDebugSettings.DebugGeography)
            );
        }

        private static int? GetCmpDebugGeography(CMPDebugGeography? debugGeography)
        {
            int? cmpDebugGeography;
            switch (debugGeography)
            {
                case Api.CMPDebugGeography.Disabled:
                    cmpDebugGeography= GEOGRAPHY_DISABLED;
                    break;
                case Api.CMPDebugGeography.EEA:
                    cmpDebugGeography= GEOGRAPHY_EEA;
                    break;
                case Api.CMPDebugGeography.NotEEA:
                    cmpDebugGeography= GEOGRAPHY_NOT_EEA;
                    break;
                default:
                    cmpDebugGeography = null;
                    break;
            }
            return cmpDebugGeography;
        }


        public AndroidJavaObject ToAndroidJavaObject()
        {
            var cmpDebugGeographyJavaObject = CMPDebugGeography == null ? null : new AndroidJavaObject(JAVA_LANG_INTEGER_CLASSNAME, CMPDebugGeography);

            return new AndroidJavaObject(
                CMP_DEBUG_SETTING_DTO_CLASSNAME,
                cmpDebugGeographyJavaObject
            );
        }
    }
}