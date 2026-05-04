using System;
using UnityEngine;
using XMediator.Api;

namespace XMediator.iOS
{
    [Serializable]
    internal class AppEventDto
    {
        [SerializeField] internal string name;
        [SerializeField] internal NullableObject<CustomPropertiesDto> properties;

        private AppEventDto(string name, NullableObject<CustomPropertiesDto> properties)
        {
            this.name = name;
            this.properties = properties;
        }

        internal static AppEventDto FromAppEvent(AppEvent appEvent)
        {
            var propertiesDto = appEvent.Properties != null
                ? CustomPropertiesDto.FromCustomProperties(appEvent.Properties)
                : null;
            return new AppEventDto(appEvent.Name, new NullableObject<CustomPropertiesDto>(propertiesDto));
        }

        internal string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
    }
}
