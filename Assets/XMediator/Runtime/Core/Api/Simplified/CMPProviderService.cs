using System;

namespace XMediator.Api
{
    /// <summary>
    /// This class is the entry point for interacting with the CMP provider.
    /// </summary>
    public class CMPProviderService
    {
        private readonly CMPProviderServiceProxy _cmpProviderServiceProxy;

        internal CMPProviderService()
        {
            _cmpProviderServiceProxy = ProxyFactory.CreateInstance<CMPProviderServiceProxy>("CMPProviderServiceProxy");
        }

        /// <summary>
        /// Returns true if the CMP has a form available to show to the user, false otherwise. Requires the CMP to have updated the user requirements
        /// in this session, this can either be done by enabling CMP Automation on XMediator or by requesting it to the CMP directly via its SDK API.
        /// </summary>
        public bool IsPrivacyFormAvailable()
        {
            return _cmpProviderServiceProxy.IsPrivacyFormAvailable();
        }

        /// <summary>
        /// Shows the CMP privacy form if available.
        /// </summary>
        public void ShowPrivacyForm(Action<Exception> onComplete = null)
        {
            _cmpProviderServiceProxy.ShowPrivacyForm(onComplete ?? (exception => { }));
        }
        
        /// <summary>
        /// Resets the user's consent (debug only).
        /// </summary>
        public void Reset()
        {
            _cmpProviderServiceProxy.Reset();
        }
    }
}
