using System;
using System.Runtime.Serialization;

namespace XMediator.Editor.Tools.DependencyManager.Entities
{
    [DataContract]
    internal class SemanticVersion : IComparable<SemanticVersion>, IEquatable<SemanticVersion>
    {
        [DataMember]
        internal Version Version { get; set; }

        internal SemanticVersion(string version)
        {
            var split = version.Split('.');
            var major = int.Parse(split[0]);
            var minor = int.Parse(split[1]);
            var patch = int.Parse(split[2]);
            Version = new Version(major: major, minor: minor, build: patch);
        }

        internal bool IsLowerThan(SemanticVersion other)
        {
            return Version.CompareTo(other.Version) < 0;
        }
        
        internal bool IsLowerOrEqualThan(SemanticVersion other)
        {
            return Version.CompareTo(other.Version) <= 0;
        }
        
        internal bool IsMajorVersionEqual(SemanticVersion other)
        {
            return Version.Major.CompareTo(other.Version.Major) == 0 ;
        }

        internal bool NotEquals(SemanticVersion other)
        {
            return !(Version.Major == other.Version.Major && Version.Minor == other.Version.Minor &&
                    Version.Build == other.Version.Build);
        }

        protected bool Equals(SemanticVersion other)
        {
            return other != null && Version.Equals(other.Version);
        }

        public int CompareTo(SemanticVersion other)
        {
            return other == null ? 1 : Version.CompareTo(other.Version);
        }

        bool IEquatable<SemanticVersion>.Equals(SemanticVersion other)
        {
            return Equals(other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SemanticVersion)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Version.Major;
                hashCode = (hashCode * 397) ^ Version.Minor;
                hashCode = (hashCode * 397) ^ Version.Build;
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{Version.Major}.{Version.Minor}.{Version.Build}";
        }
    }
}