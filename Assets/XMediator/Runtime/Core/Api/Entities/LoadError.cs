namespace XMediator.Api
{
    
    /// <summary>
    /// Describes an error when trying to load an Ad.
    /// </summary>
    public class LoadError
    {
        /// <summary>
        /// Enum class containing causes for a load call to fail.
        /// </summary>
        public enum ErrorType
        {
            RequestFailed = 1,
            NoFill = 2,
            AlreadyUsed = 3,
            Unexpected = 4
        }

        /// <summary>
        /// The error type related to this error.
        /// </summary>
        public ErrorType Type { get; }

        /// <summary>
        /// A message describing the error.
        /// </summary>
        public string Message { get; }

        public LoadError(ErrorType type, string message)
        {
            Type = type;
            Message = message;
        }

        protected bool Equals(LoadError other)
        {
            return Type == other.Type && Message == other.Message;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LoadError)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)Type * 397) ^ (Message != null ? Message.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return $"{nameof(Type)}: {Type}, {nameof(Message)}: {Message}";
        }
    }
}