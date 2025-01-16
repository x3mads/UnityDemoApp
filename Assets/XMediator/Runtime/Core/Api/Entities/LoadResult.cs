using System.Collections.Generic;
using System.Linq;

namespace XMediator.Api
{
    /// <summary>
    /// Describes the result of a load call.
    /// </summary>
    public class LoadResult
    {
        /// <summary>
        /// An identifier for the waterfall used in the current load call.
        /// </summary>
        public string WaterfallId { get; }
        
        /// <summary>
        /// An identifier for the current load call, useful to track the progress of the ad lifecycle flow.
        /// </summary>
        public string LifecycleId { get; }
        
        /// <summary>
        /// A collection of the waterfall instances and their results involved during the load call.
        /// </summary>
        public IEnumerable<InstanceResult> Instances { get; }

        public LoadResult(string waterfallId, string lifecycleId, IEnumerable<InstanceResult> instances)
        {
            WaterfallId = waterfallId;
            LifecycleId = lifecycleId;
            Instances = instances;
        }
        
        /// <summary>
        /// If the load result was successful, return an instance of <see cref="InstanceResult.Success"/>
        /// </summary>
        /// <param name="success"><see cref="InstanceResult.Success"/> When this method returns, contains the waterfall's winning instance information, if the load result was successful; otherwise, a null value. This parameter is passed uninitialized.</param>
        /// <returns><see cref="bool"/> true if the load result was successful; otherwise, false.</returns>
        public bool TryGetSuccessResult(out InstanceResult.Success success)
        {
            foreach (var instance in Instances)
            {
                if (instance.TryGetSuccess(out success))
                {
                    return true;
                }
            }

            success = null;
            return false;
        }

        protected bool Equals(LoadResult other)
        {
            return WaterfallId == other.WaterfallId && LifecycleId == other.LifecycleId && Instances.SequenceEqual(other.Instances);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LoadResult) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (WaterfallId != null ? WaterfallId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (LifecycleId != null ? LifecycleId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Instances != null ? Instances.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"WaterfallId: {WaterfallId}, LifecycleId: {LifecycleId}, {nameof(Instances)}: [{string.Join(", ", Instances)}]";
        }
    }
}