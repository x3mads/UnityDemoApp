namespace XMediator.Api
{
    /// <summary>
    /// Describes an error when trying to show an Ad.
    /// </summary>
    public class ShowError
    {
        /// <summary>
        /// Enum class containing causes for a show call to fail.
        /// </summary>
        public enum ErrorType
        {
            Unexpected = 1,
            NotRequested = 2,
            NoLongerAvailable = 3,
            Loading = 4,
            ShowFailed = 5,
            AlreadyUsed = 6,
        }

        /// <summary>
        /// The error type related to this error.
        /// </summary>
        public ErrorType Type { get; }

        /// <summary>
        /// A message describing the error.
        /// </summary>
        public string Message { get; }
        
        /// <summary>
        /// An error code provided by the underlying adapter, if available.
        /// </summary>
        public int? AdapterCode { get; }
        
        /// <summary>
        /// Name of the error, if available.
        /// </summary>
        public string ErrorName { get; }

        public ShowError(ErrorType type, string message, int? adapterCode = null, string errorName = null)
        {
            Type = type;
            Message = message;
            AdapterCode = adapterCode;
            ErrorName = errorName;
        }

        protected bool Equals(ShowError other)
        {
            return Type == other.Type && Message == other.Message && AdapterCode == other.AdapterCode &&
                   ErrorName == other.ErrorName;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ShowError)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)Type;
                hashCode = (hashCode * 397) ^ (Message != null ? Message.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ AdapterCode.GetHashCode();
                hashCode = (hashCode * 397) ^ (ErrorName != null ? ErrorName.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return
                $"{nameof(Type)}: {Type}, {nameof(Message)}: {Message}, {nameof(AdapterCode)}: {AdapterCode}, {nameof(ErrorName)}: {ErrorName}";
        }
    }
}