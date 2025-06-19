using System;
using JetBrains.Annotations;

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
        [CanBeNull]
        public string SubNetworkName { get; }

        /// <summary>
        /// Id of the creative rendered, if available.
        /// </summary>
        [CanBeNull]
        public string CreativeId { get; }
        
        /// <summary>
        /// The name of the ad space provided when presenting the ad, if available.
        /// </summary>
        [CanBeNull]
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
        
        /// <summary>
        /// Unique identifier generated for the impression.
        /// </summary>
        public string Id { get; }

        internal ImpressionData(decimal revenue, decimal ecpm, string placementId, string networkName,
            string subNetworkName, string creativeId, string adSpace, LoadResult waterfallResult,
            string adNetwork, string mediation, string id)
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
            Id = id;
        }

        protected bool Equals(ImpressionData other)
        {
            return Revenue == other.Revenue && Ecpm == other.Ecpm && PlacementId == other.PlacementId && NetworkName == other.NetworkName && SubNetworkName == other.SubNetworkName && CreativeId == other.CreativeId && AdSpace == other.AdSpace && AdNetwork == other.AdNetwork && Mediation == other.Mediation && Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ImpressionData) obj);
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(Revenue);
            hashCode.Add(Ecpm);
            hashCode.Add(PlacementId);
            hashCode.Add(NetworkName);
            hashCode.Add(SubNetworkName);
            hashCode.Add(CreativeId);
            hashCode.Add(AdSpace);
            hashCode.Add(AdNetwork);
            hashCode.Add(Mediation);
            hashCode.Add(Id);
            return hashCode.ToHashCode();
        }

        public override string ToString()
        {
            return $"{nameof(Revenue)}: {Revenue}, {nameof(Ecpm)}: {Ecpm}, {nameof(PlacementId)}: {PlacementId}, {nameof(Mediation)}: {Mediation}, {nameof(AdNetwork)}: {AdNetwork}, ImpressionId: {Id}, {nameof(WaterfallResult)}: {WaterfallResult}";
        }
    }
}