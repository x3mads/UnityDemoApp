using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using XMediator.Api;

namespace XMediator.Android
{
    internal class InitSettingsDto
    {
        internal const string INIT_SETTINGS_CLASSNAME = "com.etermax.android.xmediator.unityproxy.dto.InitSettingsDto";
        internal bool Test { get; }
        internal bool Verbose { get; }
        [CanBeNull] internal string ClientVersion { get; }
        [CanBeNull] internal string[] PlacementIds { get; }
        internal UserPropertiesDto UserProperties { get; }
        [CanBeNull] internal ConsentInformationDto ConsentInformation { get; }

        internal InitSettingsDto(
            bool test,
            bool verbose,
            [CanBeNull] string clientVersion,
            [CanBeNull] string[] placementIds,
            UserPropertiesDto userProperties,
            [CanBeNull] ConsentInformationDto consentInformation
        )
        {
            Test = test;
            Verbose = verbose;
            ClientVersion = clientVersion;
            PlacementIds = placementIds;
            UserProperties = userProperties;
            ConsentInformation = consentInformation;
        }

        internal AndroidJavaObject ToAndroidJavaObject()
        {
            using (var userPropertiesJavaObject = UserProperties.ToAndroidJavaObject())
            {
                using (var consentInformationJavaObject = ConsentInformation?.ToAndroidJavaObject())
                {
                    return new AndroidJavaObject(
                        INIT_SETTINGS_CLASSNAME,
                        Test,
                        Verbose,
                        ClientVersion,
                        PlacementIds,
                        userPropertiesJavaObject,
                        consentInformationJavaObject
                    );
                }
            }
        }

        protected bool Equals(InitSettingsDto other)
        {
            return Test == other.Test && Verbose == other.Verbose && ClientVersion == other.ClientVersion &&
                   PlacementIds?.SequenceEqual(other.PlacementIds ?? new string[] { }) == true &&
                   Equals(UserProperties, other.UserProperties) && Equals(ConsentInformation, other.ConsentInformation);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((InitSettingsDto) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Test.GetHashCode();
                hashCode = (hashCode * 397) ^ Verbose.GetHashCode();
                hashCode = (hashCode * 397) ^ (ClientVersion != null ? ClientVersion.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (PlacementIds != null ? PlacementIds.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (UserProperties != null ? UserProperties.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ConsentInformation != null ? ConsentInformation.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{nameof(Test)}: {Test}, {nameof(Verbose)}: {Verbose}, {nameof(ClientVersion)}: {ClientVersion}, {nameof(PlacementIds)}: {PlacementIds}, {nameof(UserProperties)}: {UserProperties}, {nameof(ConsentInformation)}: {ConsentInformation}";
        }
    }

    internal static class InitSettingsExtensions
    {
        internal static InitSettingsDto ToInitSettingsDto(this InitSettings initSettings)
        {
            return new InitSettingsDto(
                test: initSettings.Test,
                verbose: initSettings.Verbose,
                clientVersion: initSettings.ClientVersion,
                placementIds: initSettings.PlacementIds?.ToArray(),
                userProperties: UserPropertiesDto.From(initSettings.UserProperties),
                consentInformation: ConsentInformationDto.FromNullable(initSettings.ConsentInformation)
            );
        }
    }
}