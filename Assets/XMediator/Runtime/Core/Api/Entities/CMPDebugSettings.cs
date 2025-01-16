namespace XMediator.Api
{
    /// <summary>
    /// Settings to debug the CMP integration.
    /// <seealso cref="ConsentInformation"/>
    /// <seealso cref="InitSettings"/>
    /// <seealso cref="XMediatorSdk.SetConsentInformation"/>
    /// </summary>
    public class CMPDebugSettings
    {
        public CMPDebugGeography? DebugGeography { get; }

        /// <summary>
        /// Creates a new <see cref="CMPDebugSettings"/> instance.
        /// </summary>
        /// <param name="debugGeography">Defines the debug geography for the CMP.</param>
        public CMPDebugSettings(CMPDebugGeography? debugGeography = null)
        {
            DebugGeography = debugGeography;
        }

        protected bool Equals(CMPDebugSettings other)
        {
            return DebugGeography == other.DebugGeography;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CMPDebugSettings)obj);
        }

        public override int GetHashCode()
        {
            return DebugGeography.GetHashCode();
        }

        public override string ToString()
        {
            return $"{nameof(DebugGeography)}: {DebugGeography}";
        }
    }
}