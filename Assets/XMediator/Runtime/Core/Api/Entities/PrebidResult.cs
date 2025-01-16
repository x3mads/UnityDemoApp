using System;
using System.Collections.Generic;

namespace XMediator.Api
{
    /// <summary>
    /// Base class representing all possible prebid results.
    /// </summary>
    public abstract class PrebidResult
    {
        /// <summary>
        /// Name of the bidder.
        /// </summary>
        public abstract string BidderName { get; }
        
        /// <summary>
        /// Id of the bidder.
        /// </summary>
        public abstract string BidId { get; }
        
        /// <summary>
        /// The amount of time the bidder used to bid for the ad placement.
        /// </summary>
        public abstract TimeSpan Latency { get; }
        
        /// <summary>
        /// Bidder parameters.
        /// </summary>
        internal abstract Dictionary<string, object> Extras { get; }

        private PrebidResult()
        {
        }

        /// <summary>
        /// Class representing a successful bid
        /// </summary>
        public class Success : PrebidResult
        {
            public override string BidderName { get; }
            public override string BidId { get; }
            public override TimeSpan Latency { get; }
            internal override Dictionary<string, object> Extras { get; }
            
            /// <summary>
            /// Estimated CPM for this bid.
            /// </summary>
            public decimal BidEcpm { get; }

            public Success(
                string bidderName,
                string bidId,
                TimeSpan latency,
                decimal bidEcpm,
                Dictionary<string, object> extras = null
            )
            {
                BidderName = bidderName;
                BidId = bidId;
                Latency = latency;
                Extras = extras ?? new Dictionary<string, object>();
                BidEcpm = bidEcpm;
            }

            protected bool Equals(Success other)
            {
                return BidderName == other.BidderName && BidId == other.BidId && Latency.Equals(other.Latency) &&
                       BidEcpm.Equals(other.BidEcpm);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Success)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = (BidderName != null ? BidderName.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (BidId != null ? BidId.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ Latency.GetHashCode();
                    hashCode = (hashCode * 397) ^ BidEcpm.GetHashCode();
                    return hashCode;
                }
            }

            public override string ToString()
            {
                return
                    $"{nameof(BidderName)}: {BidderName}, {nameof(BidId)}: {BidId}, {nameof(Latency)}: {Latency}, {nameof(BidEcpm)}: {BidEcpm}";
            }
        }

        /// <summary>
        /// Class representing a failed bid
        /// </summary>
        public class Failure : PrebidResult
        {
            public override string BidderName { get; }
            public override string BidId { get; }
            public override TimeSpan Latency { get; }
            internal override Dictionary<string, object> Extras { get; }
            
            /// <summary>
            /// Error of the instance belonging to this bid.
            /// </summary>
            public InstanceError InstanceError { get; }

            public Failure(
                string bidderName,
                string bidId,
                TimeSpan latency,
                InstanceError instanceError,
                Dictionary<string, object> extras = null
            )
            {
                BidderName = bidderName;
                BidId = bidId;
                Latency = latency;
                Extras = extras ?? new Dictionary<string, object>();
                InstanceError = instanceError;
            }

            protected bool Equals(Failure other)
            {
                return BidderName == other.BidderName && BidId == other.BidId && Latency.Equals(other.Latency) &&
                       Equals(InstanceError, other.InstanceError);
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
                    var hashCode = (BidderName != null ? BidderName.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (BidId != null ? BidId.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ Latency.GetHashCode();
                    hashCode = (hashCode * 397) ^ (InstanceError != null ? InstanceError.GetHashCode() : 0);
                    return hashCode;
                }
            }

            public override string ToString()
            {
                return
                    $"{nameof(BidderName)}: {BidderName}, {nameof(BidId)}: {BidId}, {nameof(Latency)}: {Latency}, {nameof(InstanceError)}: {InstanceError}";
            }
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
    }
}