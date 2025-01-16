using UnityEngine;
using XMediator.Api;

namespace XMediator.Unity
{
    public class MockAdsConfigurationScript : MonoBehaviour
    {
        public bool failToLoad;
        public LoadError.ErrorType loadErrorType;
        public bool failToShow;
        public ShowError.ErrorType showErrorType;

        private void OnValidate()
        {
            MockAdsConfiguration.FailToLoad = failToLoad;
            MockAdsConfiguration.FailToShow = failToShow;
            MockAdsConfiguration.LoadError = new LoadError(loadErrorType, "Simulated load error");
            MockAdsConfiguration.ShowError = new ShowError(showErrorType, "Simulated show error");
        }

    }
}