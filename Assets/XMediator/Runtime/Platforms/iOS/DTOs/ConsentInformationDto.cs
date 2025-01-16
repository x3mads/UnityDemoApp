using System;
using JetBrains.Annotations;
using UnityEngine;
using XMediator.Api;

namespace XMediator.iOS
{
    [Serializable]
    internal class ConsentInformationDto
    {
        [SerializeField] internal bool isNull;
        [SerializeField] internal string hasUserConsent;
        [SerializeField] internal string doNotSell;
        [SerializeField] internal string isChildDirected;
        [SerializeField] internal string consentAutomationEnabled;
        [SerializeField] internal string cmpDebugGeography;

        internal ConsentInformationDto(bool isNull, bool? hasUserConsent, bool? doNotSell, bool? isChildDirected, bool? consentAutomationEnabled,
            CMPDebugGeography? cmpDebugGeography)
        {
            this.isNull = isNull;
            this.hasUserConsent = ToNullableBooleanString(hasUserConsent);
            this.doNotSell = ToNullableBooleanString(doNotSell);
            this.isChildDirected = ToNullableBooleanString(isChildDirected);
            this.consentAutomationEnabled = ToNullableBooleanString(consentAutomationEnabled);
            this.cmpDebugGeography = ToNullableCMPDebugGeographyString(cmpDebugGeography);
        }

        internal static ConsentInformationDto FromConsentInformation([CanBeNull] ConsentInformation consentInformation)
        {
            return new ConsentInformationDto(
                isNull: consentInformation == null,
                hasUserConsent: consentInformation?.HasUserConsent,
                doNotSell: consentInformation?.DoNotSell,
                isChildDirected: consentInformation?.IsChildDirected,
                consentAutomationEnabled: consentInformation?.IsCMPAutomationEnabled,
                cmpDebugGeography: consentInformation?.CMPDebugSettings?.DebugGeography
            );
        }

        internal string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        private string ToNullableBooleanString(bool? boolean) =>
            boolean == null ? "" : (bool)boolean ? "true" : "false";

        private static string ToNullableCMPDebugGeographyString(CMPDebugGeography? debugGeography)
        {
            switch (debugGeography)
            {
                case CMPDebugGeography.Disabled:
                    return "disabled";
                case CMPDebugGeography.EEA:
                    return "EEA";
                case CMPDebugGeography.NotEEA:
                    return "notEEA";
                case null:
                    return "";
                default:
                    throw new ArgumentOutOfRangeException(nameof(debugGeography), debugGeography, null);
            }
        }
    }
}