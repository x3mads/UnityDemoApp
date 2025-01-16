using System;
using System.Runtime.Serialization;
using UnityEngine;

namespace XMediator.Editor.Tools.DependencyManager.Entities
{
    [DataContract]
    internal class DependencyConstraint
    {
        [DataMember(Name = "name", Order = 0)]
        internal string Name { get; set; }

        private Version MinVersion { get; set; }

        private Version MaxVersion { get; set; }

        private int _fieldCount;

        [DataMember(Name = "min_version", Order = 1)]
        private string _minVersion;
        [DataMember(Name = "max_version", Order = 2)]
        private string _maxVersion;

        public DependencyConstraint(string name, Version minVersion, Version maxVersion, int fieldCount = 3)
        {
            Name = name;
            _fieldCount = fieldCount;
            MinVersion = NormalizeByFieldCount(minVersion);
            MaxVersion = NormalizeByFieldCount(maxVersion);
        }

        internal bool MinVersionIsSatisfied(Version version)
        {
            if (MinVersion == null)
                return true;
            
            return NormalizeByFieldCount(version) >= MinVersion;
        }
        
        internal bool MaxVersionIsSatisfied(Version version)
        {
            if (MaxVersion == null)
                return true;
            
            return NormalizeByFieldCount(version) < MaxVersion;
        }
        
        private Version NormalizeByFieldCount(Version version)
        {
            try
            {
                return new Version(version.ToString(_fieldCount));
            }
            catch (Exception)
            {
                return version;
            }
        }
        
        [OnSerializing]
        internal void OnSerializingMethod(StreamingContext context)
        {
            _minVersion = MinVersion?.ToString();
            _maxVersion = MaxVersion?.ToString();
        }
    }
}