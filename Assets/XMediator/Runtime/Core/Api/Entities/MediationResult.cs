using System.Collections.Generic;
using System.Linq;

namespace XMediator.Api
{
    /// <summary>
    /// Represents the result of XMediator SDK mediation initialization.
    /// <seealso cref="XMediatorSdk.Initialize"/>
    /// </summary>
    public class MediationResult
    {
        
        /// <summary>
        /// The networks that have been tried to be initialized and their results.
        /// </summary>
        public IEnumerable<NetworkInitResult> Networks { get; }

        public MediationResult(IEnumerable<NetworkInitResult> networks)
        {
            Networks = networks;
        }

        protected bool Equals(MediationResult other)
        {
            return Networks.SequenceEqual(other.Networks);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MediationResult)obj);
        }

        public override int GetHashCode()
        {
            return (Networks != null ? Networks.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return $"{nameof(Networks)}: [{string.Join(", ", Networks)}]";
        }
    }
}