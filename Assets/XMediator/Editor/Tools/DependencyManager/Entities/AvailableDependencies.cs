using System.Collections.Generic;
using System.Linq;

namespace XMediator.Editor.Tools.DependencyManager.Entities
{
    internal class AvailableDependencies
    {
        public AvailableDependency Sdk { get; }
        public IEnumerable<AvailableDependency> Adapters { get; }
        
        internal static AvailableDependencies Blank = new AvailableDependencies(null, new AvailableDependency[] { });

        public AvailableDependencies(AvailableDependency sdk, IEnumerable<AvailableDependency> adapters)
        {
            Sdk = sdk;
            Adapters = adapters;
        }
        
        public AvailableDependency GetByName(string name)
        {
            return Adapters.SingleOrDefault(available => available.Name.Equals(name));
        }
    }
    
   
}