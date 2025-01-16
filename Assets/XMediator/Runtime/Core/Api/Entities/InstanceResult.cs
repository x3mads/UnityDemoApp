using System;

namespace XMediator.Api
{
    /// <summary>
    /// Base class representing all possible waterfall instance results.
    /// <seealso cref="Success"/>
    /// <seealso cref="Failure"/>
    /// <seealso cref="Unused"/>
    /// </summary>
    public abstract class InstanceResult
    {
        
        /// <summary>
        /// Information of the instance affected by this result.
        /// </summary>
        public abstract InstanceInformation Information { get; }

        private InstanceResult()
        {
        }

        /// <summary>
        /// A class with the relevant information of a waterfall instance that has failed.
        /// </summary>
        public class Failure : InstanceResult
        {
            /// <summary>
            /// The amount of time the instance used to load an ad before failing.
            /// </summary>
            public TimeSpan Latency { get; }
            
            /// <summary>
            /// The error that caused the failure.
            /// </summary>
            public InstanceError Error { get; }

            public override InstanceInformation Information { get; }

            public Failure(InstanceInformation information, TimeSpan latency, InstanceError error)
            {
                Information = information;
                Latency = latency;
                Error = error;
            }

            protected bool Equals(Failure other)
            {
                return Latency == other.Latency && Equals(Error, other.Error) && Information.Equals(other.Information);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Failure)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = Latency.GetHashCode();
                    hashCode = (hashCode * 397) ^ (Error != null ? Error.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ Information.GetHashCode();
                    return hashCode;
                }
            }

            public override string ToString()
            {
                return $"{nameof(Latency)}: {Latency}, {nameof(Error)}: {Error}, {nameof(Information)}: {Information}";
            }
        }
        
        /// <summary>
        /// A class with the relevant information of a waterfall instance that has succeeded.
        /// </summary>
        public class Success : InstanceResult
        {
            /// <summary>
            /// The amount of time the instance used to load an ad before succeeding.
            /// </summary>
            public TimeSpan Latency { get; }
            
            /// <summary>
            /// Id of the creative loaded by this instance, if available.
            /// </summary>
            public String CreativeId { get; }
            
            /// <summary>
            /// Name of the network that filled the ad.
            /// </summary>
            [Obsolete("NetworkName is deprecated. Please use AdNetwork instead.")]
            public string NetworkName { get; }
        
            /// <summary>
            /// Name of the sub network that filled the ad, if available.
            /// </summary>
            public string SubNetworkName { get; }
            
            /// <summary>
            /// Ecpm value of the ad.
            /// </summary>
            public decimal Ecpm { get; }

            /// <summary>
            /// Name of the network that filled the ad.
            /// </summary>
            public string AdNetwork { get; }
        
            /// <summary>
            /// Name of the mediation service used to mediate this ad network.
            /// </summary>
            public string Mediation { get; }
            
            public override InstanceInformation Information { get; }

            public Success(InstanceInformation information, TimeSpan latency, string creativeId, string subNetworkName,
                decimal ecpm, string adNetwork, string mediation)
            {
                Information = information;
                NetworkName = information.Name;
                Latency = latency;
                CreativeId = creativeId;
                SubNetworkName = subNetworkName;
                Ecpm = ecpm;
                AdNetwork = adNetwork;
                Mediation = mediation;
            }

            protected bool Equals(Success other)
            {
                return Latency.Equals(other.Latency) &&
                       CreativeId == other.CreativeId &&
                       NetworkName == other.NetworkName &&
                       SubNetworkName == other.SubNetworkName &&
                       Ecpm == other.Ecpm &&
                       Equals(Information,
                           other.Information);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Success) obj);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Latency, CreativeId, NetworkName, SubNetworkName, Ecpm, Information);
            }

            public override string ToString()
            {
                return $"{nameof(Latency)}: {Latency}, {nameof(CreativeId)}: {CreativeId}, {nameof(Information)}: {Information}, {nameof(Ecpm)}: {Ecpm}";
            }
        }

        /// <summary>
        /// A class with the relevant information of a waterfall instance that has not been used, usually because a previous instance succeeded to load an ad.
        /// </summary>
        public class Unused : InstanceResult
        {
            public override InstanceInformation Information { get; }

            public Unused(InstanceInformation information)
            {
                Information = information;
            }

            protected bool Equals(Unused other)
            {
                return Information.Equals(other.Information);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Unused)obj);
            }

            public override int GetHashCode()
            {
                return Information.GetHashCode();
            }

            public override string ToString()
            {
                return $"{nameof(Information)}: {Information}";
            }
        }

        public bool TryGetFailure(out Failure failure)
        {
            if (this is Failure)
            {
                failure = (Failure)this;
                return true;
            }

            failure = null;
            return false;
        }

        public bool TryGetSuccess(out Success success)
        {
            if (this is Success)
            {
                success = (Success)this;
                return true;
            }

            success = null;
            return false;
        }

        public bool TryGetUnused(out Unused success)
        {
            if (this is Unused)
            {
                success = (Unused)this;
                return true;
            }

            success = null;
            return false;
        }
    }
}