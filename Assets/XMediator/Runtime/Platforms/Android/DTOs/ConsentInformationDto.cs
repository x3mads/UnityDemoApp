using JetBrains.Annotations;
using UnityEngine;
using XMediator.Api;

namespace XMediator.Android
{
    internal class ConsentInformationDto
    {
        private static string CONSENT_INFORMATION_DTO_CLASSNAME =
            "com.etermax.android.xmediator.unityproxy.dto.ConsentInformationDto";

        private static string JAVA_LANG_BOOLEAN_CLASSNAME = "java.lang.Boolean";
        internal bool? HasUserConsent { get; }
        internal bool? DoNotSell { get; }
        internal bool? IsChildDirected { get; }
        internal bool? ConsentAutomationEnabled { get; }
        [CanBeNull] internal CMPDebugSettingDto CmpDebugSettingDto { get; }

        internal ConsentInformationDto(bool? hasUserConsent = null, bool? doNotSell = null, bool? isChildDirected = null, bool? consentAutomationEnabled = null, [CanBeNull] CMPDebugSettingDto debugSettingDto = null )
        {
            HasUserConsent = hasUserConsent;
            DoNotSell = doNotSell;
            IsChildDirected = isChildDirected;
            ConsentAutomationEnabled = consentAutomationEnabled;
            CmpDebugSettingDto = debugSettingDto;
        }

        [CanBeNull]
        public static ConsentInformationDto FromNullable([CanBeNull] ConsentInformation consentInformation)
        {
            if (consentInformation == null) return null;
            return new ConsentInformationDto(
                hasUserConsent: consentInformation.HasUserConsent,
                doNotSell: consentInformation.DoNotSell,
                isChildDirected: consentInformation.IsChildDirected,
                consentAutomationEnabled: consentInformation.IsCMPAutomationEnabled,
                debugSettingDto: CMPDebugSettingDto.FromNullable(consentInformation.CMPDebugSettings)
            );
        }

        public static ConsentInformationDto From([NotNull] ConsentInformation consentInformation)
        {
            return new ConsentInformationDto(
                hasUserConsent: consentInformation.HasUserConsent,
                doNotSell: consentInformation.DoNotSell,
                isChildDirected: consentInformation.IsChildDirected,
                consentAutomationEnabled: consentInformation.IsCMPAutomationEnabled,
                debugSettingDto: CMPDebugSettingDto.FromNullable(consentInformation.CMPDebugSettings)
            );
        }

        protected bool Equals(ConsentInformationDto other)
        {
            return HasUserConsent == other.HasUserConsent && DoNotSell == other.DoNotSell && IsChildDirected == other.IsChildDirected &&
                   ConsentAutomationEnabled == other.ConsentAutomationEnabled  && CmpDebugSettingDto == other.CmpDebugSettingDto;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ConsentInformationDto)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (HasUserConsent.GetHashCode() * IsChildDirected.GetHashCode() * 397) ^ DoNotSell.GetHashCode();
            }
        }

        public override string ToString()
        {
            return
                $"{nameof(HasUserConsent)}: {HasUserConsent}, {nameof(DoNotSell)}: {DoNotSell}, {nameof(IsChildDirected)}: {IsChildDirected}, {nameof(ConsentAutomationEnabled)}: {ConsentAutomationEnabled}, {nameof(CmpDebugSettingDto)}: {CmpDebugSettingDto}";
        }

        internal AndroidJavaObject ToAndroidJavaObject()
        {
            var userConsentJavaObject = HasUserConsent == null ? null : new AndroidJavaObject(JAVA_LANG_BOOLEAN_CLASSNAME, HasUserConsent);
            var doNotSellJavaObject = DoNotSell == null ? null : new AndroidJavaObject(JAVA_LANG_BOOLEAN_CLASSNAME, DoNotSell);
            var childDirectedJavaObject = IsChildDirected == null ? null : new AndroidJavaObject(JAVA_LANG_BOOLEAN_CLASSNAME, IsChildDirected);
            var consentAutomationEnabledJavaObject = ConsentAutomationEnabled == null ? null : new AndroidJavaObject(JAVA_LANG_BOOLEAN_CLASSNAME, ConsentAutomationEnabled);
            var cmpDebugSettingJavaObject = CmpDebugSettingDto?.ToAndroidJavaObject();
            using (userConsentJavaObject)
            {
                using (doNotSellJavaObject)
                {
                    using (childDirectedJavaObject)
                    {
                        using (consentAutomationEnabledJavaObject)
                        {
                            using (cmpDebugSettingJavaObject)
                            {
                                return new AndroidJavaObject(
                                    CONSENT_INFORMATION_DTO_CLASSNAME,
                                    userConsentJavaObject,
                                    doNotSellJavaObject,
                                    childDirectedJavaObject,
                                    consentAutomationEnabledJavaObject,
                                    cmpDebugSettingJavaObject
                                );
                            }
                        }
                    }
                }
            }
        }
    }
}