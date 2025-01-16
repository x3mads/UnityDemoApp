using UnityEngine;

namespace XMediator.Android
{
    internal static class AndroidUtils
    {
        private static AndroidJavaObject unityActivity;
        
        internal static AndroidJavaObject GetUnityActivity()
        {
            if (unityActivity != null) return unityActivity;

            using(AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                return unityActivity;
            }
        }
    }
}