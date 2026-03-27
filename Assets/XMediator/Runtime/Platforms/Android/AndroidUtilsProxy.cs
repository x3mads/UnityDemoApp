using UnityEngine;

namespace XMediator.Android
{
    internal class AndroidUtilsProxy : UtilsProxy
    {
        private const string UTILS_CLASS_NAME = "com.x3mads.android.xmediator.unityproxy.utils.ScreenUtilsProxy";
        private const string GET_SCREEN_DENSITY_METHOD_NAME = "getScreenDensity";
        private const string IS_TABLET_METHOD_NAME = "isTablet";
        
        private static readonly AndroidJavaClass utilsJavaClass = new AndroidJavaClass(UTILS_CLASS_NAME);
        
        public float GetScreenDensity()
        {
            return utilsJavaClass.CallStatic<float>(GET_SCREEN_DENSITY_METHOD_NAME, AndroidUtils.GetUnityActivity());
        }

        public bool IsTablet()
        {
            return utilsJavaClass.CallStatic<bool>(IS_TABLET_METHOD_NAME, AndroidUtils.GetUnityActivity());
        }
    }
}
