#if UNITY_IOS

using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using XMediator.Editor.Tools.Settings;

namespace XMediator.Editor.iOS
{
    public static class SwiftSupportPostProcess
    {
        [PostProcessBuild(999)]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
        {
            if (buildTarget != BuildTarget.iOS) return;
            XMediatorSettingsService.Instance.ReloadSettings();
            if (XMediatorSettingsService.Instance.IsSkipSwiftConfiguration) return;

            var projPath = PBXProject.GetPBXProjectPath(buildPath);
            var proj = new PBXProject();
            proj.ReadFromFile(projPath);
            SetBuildProperties(proj);
            proj.WriteToFile(projPath);
        }

        private static void SetBuildProperties(PBXProject proj)
        {
            var frameworkTargetId = GetFrameworkTargetId(proj);
            var mainTargetId = GetMainTargetGuid(proj);
            
            // We need this properties set, and a Swift class added to the Unity generated framework (XMediatorSwiftSupportPlugin.swift in our case)
            proj.SetBuildProperty(frameworkTargetId, "SWIFT_VERSION", "5.0");
            proj.AddBuildProperty(mainTargetId, "OTHER_LDFLAGS", "-ObjC");
        }

        private static string GetFrameworkTargetId(PBXProject proj)
        {
#if UNITY_2019_3_OR_NEWER
            return proj.GetUnityFrameworkTargetGuid();
#else
            return proj.TargetGuidByName(PBXProject.GetUnityTargetName());
#endif
        }
        
        private static string GetMainTargetGuid(PBXProject proj)
        {
#if UNITY_2019_3_OR_NEWER
          return proj.GetUnityMainTargetGuid();
#else
          return proj.TargetGuidByName(PBXProject.GetUnityTargetName());
#endif
        }
    }
}

#endif