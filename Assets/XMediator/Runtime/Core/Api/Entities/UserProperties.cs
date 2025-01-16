using JetBrains.Annotations;

namespace XMediator.Api
{
    public class UserProperties
    {
        [CanBeNull] public string UserId { get; }
        public CustomProperties CustomProperties { get; }

        public UserProperties([CanBeNull] string userId = null, CustomProperties customProperties = null)
        {
            UserId = userId;
            CustomProperties = customProperties ?? new CustomProperties.Builder().Build();
        }

        protected bool Equals(UserProperties other)
        {
            return UserId == other.UserId && Equals(CustomProperties, other.CustomProperties);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UserProperties) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((UserId != null ? UserId.GetHashCode() : 0) * 397) ^
                       (CustomProperties != null ? CustomProperties.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return $"{nameof(UserId)}: {UserId}, {nameof(CustomProperties)}: {CustomProperties}";
        }
    }
}