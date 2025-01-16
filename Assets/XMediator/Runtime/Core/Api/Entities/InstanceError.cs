namespace XMediator.Api
{
    
    /// <summary>
    /// Describes an error related to a waterfall instance.
    /// </summary>
    public class InstanceError
    {
        /// <summary>
        /// Errors that can cause a waterfall instance to fail.
        /// </summary>
        public enum ErrorType
        {
            LoadFailed = 1,
            Timeout = 2,
            InitTimeout = 2,
            InitializationFailed = 3,
            InvalidConfiguration = 4,
            InvalidClassname = 5,
            Unexpected = 6,
            UnsupportedType = 7,
            Skipped = 8,
            ReinitializationUnsupported = 9
        }

        /// <summary>
        /// The type of error that describes this waterfall instance error.
        /// For a complete list of possible values, see <see cref="ErrorType"/>.
        /// </summary>
        public ErrorType Type { get; }
        
        /// <summary>
        /// Error code of the underlying adapter, if available.
        /// </summary>
        public int? AdapterCode { get; }
        
        /// <summary>
        /// Name of the instance that failed, if available.
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// A message describing the error, if available.
        /// </summary>
        public string Message { get; }

        internal InstanceError(ErrorType type, int? adapterCode = null, string name = null, string message = null)
        {
            Type = type;
            AdapterCode = adapterCode;
            Name = name;
            Message = message;
        }

        protected bool Equals(InstanceError other)
        {
            return Type == other.Type && AdapterCode == other.AdapterCode && Name == other.Name && Message == other.Message;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((InstanceError)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)Type;
                hashCode = (hashCode * 397) ^ AdapterCode.GetHashCode();
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Message != null ? Message.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return
                $"{nameof(Type)}: {Type}, {nameof(AdapterCode)}: {AdapterCode}, {nameof(Name)}: {Name}, {nameof(Message)}: {Message}";
        }
    }
}