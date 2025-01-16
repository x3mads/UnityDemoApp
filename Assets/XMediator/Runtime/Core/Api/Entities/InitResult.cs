namespace XMediator.Api
{
    /// <summary>
    /// Represents the result of the XMediator SDK core initialization.
    /// <seealso cref="XMediatorSdk.Initialize"/>
    /// </summary>
    public abstract class InitResult
    {
        private InitResult()
        {
        }

        /// <summary>
        /// Returns whether initialization was successful or not.
        /// </summary>
        public bool IsSuccess => this is Success;

        /// <summary>
        /// Represents a successful result of XMediator SDK core initialization.
        /// </summary>
        public class Success : InitResult
        {
            /// <summary>
            /// Id that represents the newly initialized session of XMediator.
            /// </summary>
            public string SessionId { get; }

            public Success(string sessionId)
            {
                SessionId = sessionId;
            }

            protected bool Equals(Success other)
            {
                return SessionId.Equals(other.SessionId);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Success)obj);
            }

            public override int GetHashCode()
            {
                return (SessionId != null ? SessionId.GetHashCode() : 0);
            }

            public override string ToString()
            {
                return $"{nameof(SessionId)}: {SessionId}]";
            }
        }

        /// <summary>
        /// Represents a failure result of XMediator SDK core initialization.
        /// </summary>
        public class Failure : InitResult
        {
            public int ErrorCode { get; }
            public string ErrorDescription { get; } // TODO check if it should be renamed to errorMessage like Android

            public Failure(int errorCode, string errorDescription)
            {
                ErrorCode = errorCode;
                ErrorDescription = errorDescription;
            }

            protected bool Equals(Failure other)
            {
                return ErrorCode == other.ErrorCode && ErrorDescription == other.ErrorDescription;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Failure)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (ErrorCode * 397) ^ (ErrorDescription != null ? ErrorDescription.GetHashCode() : 0);
                }
            }

            public override string ToString()
            {
                return $"{nameof(ErrorCode)}: {ErrorCode}, {nameof(ErrorDescription)}: {ErrorDescription}";
            }
        }

        public bool TryGetSuccess(out Success success)
        {
            if (this is Success)
            {
                success = (Success)this;
                return true;
            }

            success = null;
            return false;
        }

        public bool TryGetFailure(out Failure failure)
        {
            if (this is Failure)
            {
                failure = (Failure)this;
                return true;
            }

            failure = null;
            return false;
        }
    }
}