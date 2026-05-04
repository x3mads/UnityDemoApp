using UnityEngine;
using XMediator.Api;

namespace XMediator.Android
{
    internal class AppEventDto
    {
        private static string APP_EVENT_DTO_CLASSNAME = "com.etermax.android.xmediator.unityproxy.dto.AppEventDto";

        internal static AndroidJavaObject From(AppEvent appEvent)
        {
            var propertiesJavaObject = appEvent.Properties != null
                ? CustomPropertiesDto.From(appEvent.Properties).ToAndroidJavaObject()
                : null;
            return new AndroidJavaObject(
                APP_EVENT_DTO_CLASSNAME,
                appEvent.Name,
                propertiesJavaObject
            );
        }
    }
}
