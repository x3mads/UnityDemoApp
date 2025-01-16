using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace XMediator.Editor.Tools.DependencyManager.Entities
{
    [DataContract]
    internal class InstalledVersion
    {
        [DataMember(Name = "name", Order = 0)]
        public string Name { get; set; }
        
        [DataMember(Name = "description", Order = 1)]
        public string Description { get; set; } 
        
        public SemanticVersion Version { get; set; }
        
        [DataMember(Name = "android_version", Order = 3)] 
        public string InstalledAndroidVersion { get; set; }
        
        [DataMember(Name = "ios_version", Order = 4)]
        public string InstalledIOSVersion { get; set; }
        
        [DataMember(Name = "android_dependencies", Order = 6)]
        internal List<DependencyConstraint> androidDependencies { get; set; }
        
        [DataMember(Name = "ios_dependencies", Order = 7)]
        internal List<DependencyConstraint> iosDependencies { get; set; }
        
        
        public long InstalledAt { get; set; }
        
        private DateTime _installedAt;

        [DataMember(Name = "installed_at", Order = 5)] 
        private string _installedAtDate;

        [DataMember(Name = "version", Order = 2)]
        private string _version;
        

        public InstalledVersion(string name, string description, SemanticVersion version,
            string installedAndroidVersion, string installedIOSVersion, long installedAt,
            List<DependencyConstraint> androidConstraints = null, List<DependencyConstraint> iOSConstraints = null)
        {
            Name = name;
            Description = description;
            InstalledAndroidVersion = installedAndroidVersion;
            InstalledIOSVersion = installedIOSVersion;
            Version = version;
            InstalledAt = installedAt;
            androidDependencies = androidConstraints ?? new List<DependencyConstraint>();
            iosDependencies = iOSConstraints ?? new List<DependencyConstraint>();
        }
        
        [OnSerializing]
        internal void OnSerializingMethod(StreamingContext context)
        {
            _installedAtDate = new DateTime(ticks: InstalledAt * TimeSpan.TicksPerMillisecond).ToString(format:"yyyy-MM-dd HH:mm");
            _version = Version.ToString();
        }
    }
}