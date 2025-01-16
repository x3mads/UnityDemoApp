using System.Runtime.InteropServices;
using XMediator.Api;

namespace XMediator.iOS
{
    internal class iOSBannerProxy : BannerProxy
    {
        private string _identifier;
        private iOSNativeEventCallback _nativeEventCallback;

        public void Create(string placementId, BannerProxyListener listener, Banner.Position position, Banner.Size size, bool test, bool verbose)
        {
            _identifier = createBanner(placementId, position, size, test, verbose);
            _nativeEventCallback = new iOSNativeEventCallback(listener, listener);
            iOSCallbackServiceProxy.AddCallback(_identifier, _nativeEventCallback);
        }

        public void Create(string placementId, BannerProxyListener listener, Banner.Size size, int x, int y, bool test, bool verbose)
        {
            _identifier = createBannerWithCustomPosition(placementId, x, y, size, test, verbose);
            _nativeEventCallback = new iOSNativeEventCallback(listener, listener);
            iOSCallbackServiceProxy.AddCallback(_identifier, _nativeEventCallback);
        }

        public void Load(CustomProperties customProperties)
        {
            var customPropertiesDto = CustomPropertiesDto.FromCustomProperties(customProperties);
            loadBanner(_identifier, customPropertiesDto.ToJson());
        }

        public void SetPosition(int x, int y) => setBannerPosition(_identifier, x, y);
        public void SetAdSpace(string adSpace) => setBannerAdSpace(_identifier, adSpace);

        public void Show() => presentBannerWithoutPosition(_identifier);
        
        public void Show(Banner.Position position) => presentBanner(_identifier, position);

        public void Hide() => hideBanner(_identifier);

        public void Dispose()
        {
            DisposeNative();
            iOSCallbackServiceProxy.RemoveCallback(_identifier);
        }

        private void DisposeNative()
        {
            dispose(_identifier);
        }

        ~iOSBannerProxy()
        {
            DisposeNative();
        }

        [DllImport("__Internal")]
        private static extern string createBanner(string adUnitId, Banner.Position position, Banner.Size size, bool test, bool verbose);
        
        [DllImport("__Internal")]
        private static extern string createBannerWithCustomPosition(string adUnitId, int x, int y, Banner.Size size, bool test, bool verbose);

        [DllImport("__Internal")]
        private static extern void loadBanner(string identifier, string customProperties);

        [DllImport("__Internal")]
        private static extern void presentBanner(string identifier, Banner.Position position);
        
        [DllImport("__Internal")]
        private static extern void presentBannerWithoutPosition(string identifier);

        [DllImport("__Internal")]
        private static extern void hideBanner(string identifier);
        
        [DllImport("__Internal")]
        private static extern void setBannerAdSpace(string identifier, string adSpace);

        [DllImport("__Internal")]
        private static extern void dispose(string identifier);
        
        [DllImport("__Internal")]
        private static extern void setBannerPosition(string identifier, int x, int y);
    }
}