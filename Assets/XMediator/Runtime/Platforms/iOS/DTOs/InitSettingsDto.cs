using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XMediator.Api;

namespace XMediator.iOS
{
    [Serializable]
    internal class InitSettingsDto
    {
        [SerializeField] internal List<string> placementIds;
        [SerializeField] internal string clientVersion;
        [SerializeField] internal string unityVersion;
        [SerializeField] internal UserPropertiesDto userProperties;
        [SerializeField] internal ConsentInformationDto consentInformation;
        [SerializeField] internal bool test;
        [SerializeField] internal bool verbose;

        internal InitSettingsDto(List<string> placementIds, string clientVersion, string unityVersion, UserPropertiesDto userProperties, ConsentInformationDto consentInformation, bool test, bool verbose)
        {
            this.placementIds = placementIds;
            this.clientVersion = clientVersion;
            this.unityVersion = unityVersion;
            this.userProperties = userProperties;
            this.consentInformation = consentInformation;
            this.test = test;
            this.verbose = verbose;
        }

        internal static InitSettingsDto FromInitSettings(InitSettings initSettings, string unityVersion)
        {
            var userPropertiesDto = UserPropertiesDto.FromUserProperties(initSettings.UserProperties);
            var consentInformationDto = ConsentInformationDto.FromConsentInformation(initSettings.ConsentInformation);
            return new InitSettingsDto(
                initSettings.PlacementIds?.ToList(),
                initSettings.ClientVersion,
                unityVersion,
                userPropertiesDto,
                consentInformationDto,
                initSettings.Test,
                initSettings.Verbose);
        }
        
        internal string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
    }
}