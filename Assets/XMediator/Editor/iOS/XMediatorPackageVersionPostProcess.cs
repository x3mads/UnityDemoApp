#if UNITY_IOS

using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using XMediator.Editor.Tools.Settings;

namespace XMediator.Editor.iOS
{
    public static class XMediatorPackageVersionPostProcess
    {
        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
        {
            if (buildTarget != BuildTarget.iOS) return;
            var plistPath = Path.Combine(buildPath, "Info.plist");
            var plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            XMediatorSettingsService.Instance.ReloadSettings();
            var packageVersion = XMediatorSettingsService.Instance.PackageVersion;
            if (!string.IsNullOrEmpty(packageVersion))
            {
                plist.root.SetString("XMediatorUnityPackageVersion", packageVersion);
            }

            plist.WriteToFile(plistPath);
        }
    }
}

#endif