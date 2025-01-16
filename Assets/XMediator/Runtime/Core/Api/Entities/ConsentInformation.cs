using JetBrains.Annotations;

namespace XMediator.Api
{
    /// <summary>
    /// Consent configuration settings object.
    /// <seealso cref="InitSettings"/>
    /// <seealso cref="XMediatorSdk.SetConsentInformation"/>
    /// </summary>
    public class ConsentInformation
    {
        public bool? HasUserConsent { get; }
        public bool? DoNotSell { get; }
        public bool? IsChildDirected { get; }
        public bool IsCMPAutomationEnabled { get; }
        [CanBeNull] public CMPDebugSettings CMPDebugSettings { get; }

        /// <summary>
        /// Creates a new <see cref="ConsentInformation"/> instance.
        /// </summary>
        /// <param name="hasUserConsent">Whether the user has given consent to serve personalized ads.</param>
        /// <param name="doNotSell">Whether the user has opted out of data sale. </param>
        /// <param name="isChildDirected">Whether the user is a child. Review the Children's Online Privacy Protection Act (COPPA). </param>
        /// <param name="isCMPAutomationEnabled">Enables automatic consent gathering using UMP before initializing ad mediation. </param>
        /// <param name="cmpDebugSettings">Settings to debug the CMP integration. </param>
        public ConsentInformation(bool? hasUserConsent = null, bool? doNotSell = null, bool? isChildDirected = null, bool isCMPAutomationEnabled = false, CMPDebugSettings cmpDebugSettings = null)
        {
            HasUserConsent = hasUserConsent;
            DoNotSell = doNotSell;
            IsChildDirected = isChildDirected;
            IsCMPAutomationEnabled = isCMPAutomationEnabled;
            CMPDebugSettings = cmpDebugSettings;
        }

        protected bool Equals(ConsentInformation other)
        {
            return HasUserConsent == other.HasUserConsent && DoNotSell == other.DoNotSell && IsChildDirected == other.IsChildDirected &&
                   IsCMPAutomationEnabled == other.IsCMPAutomationEnabled;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ConsentInformation) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = HasUserConsent.GetHashCode();
                hashCode = (hashCode * 397) ^ DoNotSell.GetHashCode();
                hashCode = (hashCode * 397) ^ IsChildDirected.GetHashCode();
                hashCode = (hashCode * 397) ^ IsCMPAutomationEnabled.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{nameof(HasUserConsent)}: {HasUserConsent}, {nameof(DoNotSell)}: {DoNotSell}, {nameof(IsChildDirected)}: {IsChildDirected}, {nameof(IsCMPAutomationEnabled)}: {IsCMPAutomationEnabled}, {nameof(CMPDebugSettings)}: {CMPDebugSettings}";
        }
    }
}