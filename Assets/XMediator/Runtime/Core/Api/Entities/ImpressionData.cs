using System;

namespace XMediator.Api
{
    /// <summary>
    /// Describes the impression data of an ad.
    /// </summary>
    public class ImpressionData
    {
        /// <summary>
        /// Estimated revenue that the ad generated.
        /// </summary>
        public decimal Revenue { get; }

        /// <summary>
        /// Ecpm value for the ad.
        /// </summary>
        public decimal Ecpm { get; }

        /// <summary>
        /// Placement id of the waterfall used to load this ad.
        /// </summary>
        public string PlacementId { get; }

        /// <summary>
        /// Name of the network used to render this ad.
        /// </summary>
        [Obsolete("NetworkName is deprecated. Please use AdNetwork instead.")]
        public string NetworkName { get; }

        /// <summary>
        /// Name of the sub network used to render this ad, if available.
        /// </summary>
        public string SubNetworkName { get; }

        /// <summary>
        /// Id of the creative rendered, if available.
        /// </summary>
        public string CreativeId { get; }
        
        /// <summary>
        /// The name of the ad space provided when presenting the ad, if available.
        /// </summary>
        public string AdSpace { get; }

        /// <summary>
        /// Name of the network that filled the ad.
        /// </summary>
        public string AdNetwork { get; }
        
        /// <summary>
        /// Name of the mediation service used to mediate this ad network.
        /// </summary>
        public string Mediation { get; }

        /// <summary>
        /// Describes the result of the waterfall used to load this ad.
        /// </summary>
        public LoadResult WaterfallResult { get; }

        internal ImpressionData(decimal revenue, decimal ecpm, string placementId, string networkName,
            string subNetworkName, string creativeId, string adSpace, LoadResult waterfallResult,
            string adNetwork, string mediation)
        {
            Revenue = revenue;
            Ecpm = ecpm;
            PlacementId = placementId;
            NetworkName = networkName;
            CreativeId = creativeId;
            SubNetworkName = subNetworkName;
            AdSpace = adSpace;
            WaterfallResult = waterfallResult;
            AdNetwork = adNetwork;
            Mediation = mediation;
        }

        protected bool Equals(ImpressionData other)
        {
            return Revenue == other.Revenue && Ecpm == other.Ecpm && PlacementId == other.PlacementId &&
                   NetworkName == other.NetworkName && SubNetworkName == other.SubNetworkName &&
                   CreativeId == other.CreativeId && AdSpace == other.AdSpace
                   && Equals(WaterfallResult, other.WaterfallResult);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ImpressionData) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Revenue.GetHashCode();
                hashCode = (hashCode * 397) ^ Ecpm.GetHashCode();
                hashCode = (hashCode * 397) ^ (PlacementId != null ? PlacementId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (NetworkName != null ? NetworkName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (SubNetworkName != null ? SubNetworkName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CreativeId != null ? CreativeId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (AdSpace != null ? AdSpace.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (WaterfallResult != null ? WaterfallResult.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{nameof(Revenue)}: {Revenue}, {nameof(Ecpm)}: {Ecpm}, {nameof(PlacementId)}: {PlacementId}, {nameof(NetworkName)}: {NetworkName}, {nameof(WaterfallResult)}: {WaterfallResult}";
        }
    }
}