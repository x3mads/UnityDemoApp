#if UNITY_IOS

using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using XMediator.Editor.Tools.Settings;

namespace XMediator.Editor.iOS
{
    public static class GoogleAdsiOSAppIdPostProcess
    {
        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
        {
            if (buildTarget != BuildTarget.iOS) return;
            var plistPath = Path.Combine(buildPath, "Info.plist");
            var plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            XMediatorSettingsService.Instance.ReloadSettings();
            var googleAdsIOSAppId = XMediatorSettingsService.Instance.IOSGoogleAdsAppId;
            if (!string.IsNullOrEmpty(googleAdsIOSAppId))
            {
                plist.root.SetString("GADApplicationIdentifier", googleAdsIOSAppId);
            }

            plist.WriteToFile(plistPath);
        }
    }
}

#endif