using System.Runtime.InteropServices;
using XMediator.Api;

namespace XMediator.iOS
{
    internal class iOSRewardedProxy : RewardedProxy
    {
        private string _identifier;
        private iOSNativeEventCallback _nativeEventCallback;

        public void Create(string placementId, RewardedProxyListener listener, bool test, bool verbose)
        {
            _identifier = createRewarded(placementId, test, verbose);
            _nativeEventCallback = new iOSNativeEventCallback(listener, listener, listener);
            iOSCallbackServiceProxy.AddCallback(_identifier, _nativeEventCallback);
        }

        public void Load(CustomProperties customProperties)
        {
            var customPropertiesDto = CustomPropertiesDto.FromCustomProperties(customProperties);
            loadRewarded(_identifier, customPropertiesDto.ToJson());
        }

        public void Show() => presentRewarded(_identifier);
        
        public void Show(string adSpace) => presentRewardedWithAdSpace(_identifier, adSpace);

        public bool IsReady() => isReadyRewarded(_identifier);

        public void Dispose()
        {
            DisposeNative();
            iOSCallbackServiceProxy.RemoveCallback(_identifier);
        }

        private void DisposeNative()
        {
            dispose(_identifier);
        }

        ~iOSRewardedProxy()
        {
            DisposeNative();
        }

        [DllImport("__Internal")]
        private static extern string createRewarded(string adUnitId, bool test, bool verbose);

        [DllImport("__Internal")]
        private static extern void loadRewarded(string identifier, string customProperties);

        [DllImport("__Internal")]
        private static extern void presentRewarded(string identifier);
        
        [DllImport("__Internal")]
        private static extern void presentRewardedWithAdSpace(string identifier, string adSpace);

        [DllImport("__Internal")]
        private static extern bool isReadyRewarded(string identifier);
        
        [DllImport("__Internal")]
        private static extern void dispose(string identifier); 
    }
}