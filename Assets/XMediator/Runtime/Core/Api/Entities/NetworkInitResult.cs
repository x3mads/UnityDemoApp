namespace XMediator.Api
{
    
    /// <summary>
    /// Base class for the result of a network initialization attempt.  
    /// </summary>
    public abstract class NetworkInitResult
    {
        private NetworkInitResult()
        {
        }

        /// <summary>
        /// A class that contains information about a successful network initialization.
        /// </summary>
        public class Success : NetworkInitResult
        {
            /// <summary>
            /// The name of the network that has been initialized.
            /// </summary>
            public string Name { get; }
            
            /// <summary>
            /// A description for this result.
            /// </summary>
            public string Description { get; }

            internal Success(string name, string description)
            {
                Name = name;
                Description = description;
            }

            protected bool Equals(Success other)
            {
                return Name == other.Name && Description == other.Description;
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
                unchecked
                {
                    return ((Name != null ? Name.GetHashCode() : 0) * 397) ^
                           (Description != null ? Description.GetHashCode() : 0);
                }
            }

            public override string ToString()
            {
                return $"{nameof(Name)}: {Name}, {nameof(Description)}: {Description}";
            }
        }
        
        /// <summary>
        /// A class that contains information about a failed network initialization.
        /// </summary>
        public class Failure : NetworkInitResult
        {
            /// <summary>
            /// The name of the network that failed to be initialized.
            /// </summary>
            public string Name { get; }
            
            /// <summary>
            /// A description of the failure.
            /// </summary>
            public string Description { get; }
            
            /// <summary>
            /// An instance of <see cref="InstanceError"/> containing information about the error.
            /// </summary>
            public InstanceError Error { get; }

            internal Failure(string name, string description, InstanceError error)
            {
                Name = name;
                Description = description;
                Error = error;
            }

            protected bool Equals(Failure other)
            {
                return Name == other.Name && Description == other.Description && Equals(Error, other.Error);
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
                    var hashCode = (Name != null ? Name.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (Description != null ? Description.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (Error != null ? Error.GetHashCode() : 0);
                    return hashCode;
                }
            }

            public override string ToString()
            {
                return $"{nameof(Name)}: {Name}, {nameof(Description)}: {Description}, {nameof(Error)}: {Error}";
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