using System.Collections.Generic;
using System.Linq;

namespace XMediator.Api
{
    
    /// <summary>
    /// Container class for the results of a prebid call.
    /// </summary>
    public class PrebiddingResults
    {
        /// <summary>
        /// A collection of the bidders and their results.
        /// </summary>
        public IEnumerable<PrebidResult> Results { get; }

        public PrebiddingResults(IEnumerable<PrebidResult> results)
        {
            Results = results;
        }

        protected bool Equals(PrebiddingResults other)
        {
            return Results.SequenceEqual(other.Results);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PrebiddingResults)obj);
        }

        public override int GetHashCode()
        {
            return (Results != null ? Results.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return $"{nameof(Results)}: [{string.Join(",", Results)}]";
        }
    }
}