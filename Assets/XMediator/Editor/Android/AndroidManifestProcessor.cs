using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using XMediator.Editor.Tools.Settings;

namespace XMediator.Editor.Android
{
    public class AndroidManifestProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        private const string AndroidManifestTemplatePath = "com.x3mads.xmediator/AndroidManifest.xml-TEMPLATE";
        private const string ProjectPropertiesTemplatePath = "com.x3mads.xmediator/project.properties-TEMPLATE";
        private const string ValidateDependenciesGradleTemplatePath = "com.x3mads.xmediator/validate_dependencies-TEMPLATE.gradle";
        private const string GaidPlaceholder = "<!-- GAID_PLACEHOLDER -->";
        internal const string XMediatorPluginAndroidLibPath = "Plugins/Android/XMediatorPlugin.androidlib";
        internal const string XMediatorPluginAndroidLibMetaPath = "Plugins/Android/XMediatorPlugin.androidlib.meta";
        internal const string AndroidManifestPath = "Plugins/Android/XMediatorPlugin.androidlib/AndroidManifest.xml";
        internal const string ProjectPropertiesPath = "Plugins/Android/XMediatorPlugin.androidlib/project.properties";
        internal const string ValidateDependenciesGradlePath = "Plugins/Android/XMediatorPlugin.androidlib/validate_dependencies.gradle";

        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            OnPreprocessBuild(report.summary.platform);
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            OnPostProcessBuild();
        }

        internal void OnPreprocessBuild(BuildTarget buildTarget)
        {
            if (!IsBuildingAndroid(buildTarget)) return;
            if (!IsSetGaidEnabled() && !IsGmaPropertiesTagFixEnabled()) return;

            CreatePluginDirectory();
            CreateManifest();
            CreateValidateDependencies();
            CreateProperties();
        }

        internal void OnPostProcessBuild()
        {
            RemovePluginDirectory();
        }

        private static bool IsBuildingAndroid(BuildTarget buildTarget)
        {
            return buildTarget == BuildTarget.Android;
        }

        private static bool IsSetGaidEnabled()
        {
            XMediatorSettingsService.Instance.ReloadSettings();
            return !XMediatorSettingsService.Instance.IsDisableGoogleAdsAndroidAppId;
        }

        private static bool IsGmaPropertiesTagFixEnabled()
        {
            XMediatorSettingsService.Instance.ReloadSettings();
            return XMediatorSettingsService.Instance.IsGmaPropertiesTagFixEnabled;
        }

        private static void CreatePluginDirectory()
        {
            Directory.CreateDirectory(Path.Combine(Application.dataPath, XMediatorPluginAndroidLibPath));
        }

        private static void CreateManifest()
        {
            var manifestPath = Path.Combine(Application.dataPath, AndroidManifestPath);
            var manifestAsset = Resources.Load<TextAsset>(AndroidManifestTemplatePath);
            var manifestText = manifestAsset.text;

            if (IsSetGaidEnabled())
            {
                var placeholder = GaidPlaceholder;
                var googleAppId = GetAndroidGaid();
                manifestText = manifestText.Replace(placeholder, BuildGaidTag(googleAppId));
            }

            File.WriteAllText(manifestPath, manifestText);
        }

        private static string GetAndroidGaid()
        {
            XMediatorSettingsService.Instance.ReloadSettings();
            return XMediatorSettingsService.Instance.AndroidGoogleAdsAppId;
        }

        private static void CreateProperties()
        {
            var projectPropertiesPath = Path.Combine(Application.dataPath, ProjectPropertiesPath);
            var propertiesAsset = Resources.Load<TextAsset>(ProjectPropertiesTemplatePath);
            var propertiesText = propertiesAsset.text;
            File.WriteAllText(projectPropertiesPath, propertiesText);
        }

        private static void CreateValidateDependencies()
        {
            if (!IsGmaPropertiesTagFixEnabled()) return;
            
            var validateDependenciesPath = Path.Combine(Application.dataPath, ValidateDependenciesGradlePath);
            var validateDependenciesAsset = Resources.Load<TextAsset>(ValidateDependenciesGradleTemplatePath);
            var validateDependenciesText = validateDependenciesAsset.text;
            File.WriteAllText(validateDependenciesPath, validateDependenciesText);
        }

        private void RemovePluginDirectory()
        {
            try
            {
                Directory.Delete(Path.Combine(Application.dataPath, XMediatorPluginAndroidLibPath), true);
            }
            catch (DirectoryNotFoundException)
            {
                // Ignore exception when Directory is not present
            }

            try
            {
                File.Delete(Path.Combine(Application.dataPath, XMediatorPluginAndroidLibMetaPath));
            }
            catch (DirectoryNotFoundException)
            {
                // Ignore exception when Directory is not present
            }
        }

        private static string BuildGaidTag(string gaid)
        {
            return $"<meta-data android:name=\"com.google.android.gms.ads.APPLICATION_ID\" android:value=\"{gaid}\"/>";
        }
    }
}