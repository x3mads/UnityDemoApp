using System.Runtime.InteropServices;
using XMediator.Api;

namespace XMediator.iOS
{
    internal class iOSInterstitialProxy : InterstitialProxy
    {
        private string _identifier;
        private iOSNativeEventCallback _nativeEventCallback;

        public void Create(string placementId, InterstitialProxyListener listener, bool test, bool verbose)
        {
            _identifier = createInterstitial(placementId, test, verbose);
            _nativeEventCallback = new iOSNativeEventCallback(listener, listener);
            iOSCallbackServiceProxy.AddCallback(_identifier, _nativeEventCallback);
        }

        public void Load(CustomProperties customProperties)
        {
            var customPropertiesDto = CustomPropertiesDto.FromCustomProperties(customProperties);
            loadInterstitial(_identifier, customPropertiesDto.ToJson());
        }

        public void Show() => presentInterstitial(_identifier);
        
        public void Show(string adSpace) => presentInterstitialWithAdSpace(_identifier, adSpace);

        public bool IsReady() => isReadyInterstitial(_identifier);

        public bool IsAdSpaceCapped(string adSpace) => isInterstitialAdSpaceCapped(_identifier, adSpace);

        public void Dispose()
        {
            DisposeNative();
            iOSCallbackServiceProxy.RemoveCallback(_identifier);
        }

        private void DisposeNative()
        {
            dispose(_identifier);
        }

        ~iOSInterstitialProxy()
        {
            DisposeNative();
        }

        [DllImport("__Internal")]
        private static extern string createInterstitial(string adUnitId, bool test, bool verbose);

        [DllImport("__Internal")]
        private static extern void loadInterstitial(string identifier, string customProperties);

        [DllImport("__Internal")]
        private static extern void presentInterstitial(string identifier);

        [DllImport("__Internal")]
        private static extern void presentInterstitialWithAdSpace(string identifier, string adSpace);
        
        [DllImport("__Internal")]
        private static extern bool isReadyInterstitial(string identifier);

        [DllImport("__Internal")]
        private static extern bool isInterstitialAdSpaceCapped(string identifier, string adSpace);

        [DllImport("__Internal")]
        private static extern void dispose(string identifier);
    }
}