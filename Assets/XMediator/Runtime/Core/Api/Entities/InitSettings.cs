using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace XMediator.Api
{
    /// <summary>
    /// A configuration object used during SDK initialization.
    /// <seealso cref="XMediatorSdk.Initialize"/>
    /// </summary>
    public class InitSettings
    {
        public bool Test { get; }
        public bool Verbose { get; }
        [CanBeNull] public string ClientVersion { get; }
        [CanBeNull] public IEnumerable<string> PlacementIds { get; }
        [CanBeNull] public UserProperties UserProperties { get; }
        [CanBeNull] public ConsentInformation ConsentInformation { get; }

        /// <summary>
        /// Creates a new <see cref="InitSettings"/> instance.
        /// </summary>
        /// <param name="test">Whether the SDK should be initialized in test mode. This is only for testing purposes, should be false on production builds.</param>
        /// <param name="verbose">Enable logging for this session. This is only for debugging purposes, should be false on production builds.</param>
        /// <param name="clientVersion">Optional. A string identifying the version of who interacts with the SDK.</param>
        /// <param name="placementIds">Optional. A collection of placements ids that will be used during this session.</param>
        /// <param name="userProperties">A <see cref="UserProperties"/> object containing useful information during initialization.</param>
        /// <param name="consentInformation">A <see cref="ConsentInformation"/> object containing user consent information.</param>
        public InitSettings(
            bool test = false,
            bool verbose = false,
            [CanBeNull] string clientVersion = null,
            [CanBeNull] IEnumerable<string> placementIds = null,
            [CanBeNull] UserProperties userProperties = null,
            [CanBeNull] ConsentInformation consentInformation = null
        )
        {
            Test = test;
            Verbose = verbose;
            ClientVersion = clientVersion;
            PlacementIds = placementIds;
            UserProperties = userProperties;
            ConsentInformation = consentInformation;
        }

        protected bool Equals(InitSettings other)
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
            return Equals((InitSettings) obj);
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
            return
                $"{nameof(Test)}: {Test}, {nameof(Verbose)}: {Verbose}, {nameof(ClientVersion)}: {ClientVersion}, {nameof(PlacementIds)}: [{string.Join(", ", PlacementIds ?? new List<string>())}], {nameof(UserProperties)}: {UserProperties}, {nameof(ConsentInformation)}: {ConsentInformation}";
        }
    }
}