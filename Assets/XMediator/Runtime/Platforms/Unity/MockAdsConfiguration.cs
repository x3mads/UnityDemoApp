using XMediator.Api;

namespace XMediator.Unity
{
    public class MockAdsConfiguration
    {
        public static bool FailToLoad = false;
        public static bool FailToShow = false;
        public static LoadError LoadError;
        public static ShowError ShowError;
    }
}