namespace XMediator.Api
{
    /// <summary>
    /// A group of properties that describes a waterfall instance.
    /// </summary>
    public class InstanceInformation
    {
        /// <summary>
        /// Identifier of this waterfall instance.
        /// </summary>
        public string Id { get; }
        
        /// <summary>
        /// Name of the network being used.
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// Classname of the underlying adapter.
        /// </summary>
        public string Classname { get; }
        
        /// <summary>
        /// Ecpm value for this instance.
        /// </summary>
        public decimal Ecpm { get; }

        public InstanceInformation(string id, string name, string classname, decimal ecpm)
        {
            Id = id;
            Name = name;
            Classname = classname;
            Ecpm = ecpm;
        }

        protected bool Equals(InstanceInformation other)
        {
            return Id == other.Id && Name == other.Name && Classname == other.Classname && Ecpm == other.Ecpm;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((InstanceInformation) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Id != null ? Id.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Classname != null ? Classname.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Ecpm.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}, {nameof(Classname)}: {Classname}, {nameof(Ecpm)}: {Ecpm}";
        }
    }
}