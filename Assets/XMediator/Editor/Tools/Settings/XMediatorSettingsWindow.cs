using System;
using UnityEditor;
using UnityEngine;
using XMediator.Editor.Tools.MetaMediation;
using static UnityEditor.EditorGUILayout;

namespace XMediator.Editor.Tools.Settings
{
    public class XMediatorSettingsWindow : EditorWindow
    {
        private bool _disableGoogleAdsAndroidAppId;
        private bool _skipSwiftConfiguration;
        private bool _skipSkAdNetworkIdsConfiguration;
        private bool _enableGmaPropertiesTagFix;
        private bool _isMetaMediation;
        private string _googleAdsAndroidAppId;
        private string _googleAdsIOSAppId;
        private string _dependencyManagerToken;
        private string _publisher;

        private XMediatorSettingsService _settingsService = XMediatorSettingsService.Instance;

        [MenuItem("XMediator/Settings")]
        public static void Settings()
        {
            GetWindow(typeof(XMediatorSettingsWindow), true, "XMediator Settings").Show();
        }

        private void CreateGUI()
        {
            _settingsService.ReloadSettings();
            _skipSwiftConfiguration = _settingsService.IsSkipSwiftConfiguration;
            _skipSkAdNetworkIdsConfiguration = _settingsService.IsSkipSkAdNetworkIdsConfiguration;
            _disableGoogleAdsAndroidAppId = _settingsService.IsDisableGoogleAdsAndroidAppId;
            _enableGmaPropertiesTagFix = _settingsService.IsGmaPropertiesTagFixEnabled;
            _googleAdsIOSAppId = _settingsService.IOSGoogleAdsAppId;
            _googleAdsAndroidAppId = _settingsService.AndroidGoogleAdsAppId;
            _dependencyManagerToken = _settingsService.DependencyManagerToken;
            _publisher = _settingsService.Publisher;
            _isMetaMediation = _settingsService.IsMetaMediation;
            _settingsService.UpdateXMediatorDependenciesFile();
        }

        private void OnGUI()
        {
            BeginHorizontal();
            GUILayout.FlexibleSpace();
            EndHorizontal();

            HorizontalLine();
            Space(8);
            
            Label("Publisher: ");
            CheckChanges(_publisher, value => TextField(value), newValue =>
            {
                _settingsService.Publisher = newValue;
                _publisher = newValue;
            });

            Space(16);

            Label("Dependency Manager Token: ");
            CheckChanges(_dependencyManagerToken, value => TextField(value), newValue =>
            {
                _settingsService.DependencyManagerToken = newValue;
                _dependencyManagerToken = newValue;
            });

            Space(16);

            BeginHorizontal();
            Label("Disable GoogleAds Android AppId: ");
            CheckChanges(_disableGoogleAdsAndroidAppId, value => Toggle(value), newValue =>
            {
                _settingsService.IsDisableGoogleAdsAndroidAppId = newValue;
                _disableGoogleAdsAndroidAppId = newValue;
            });

            GUILayout.FlexibleSpace();
            EndHorizontal();

            Space(4);

            if (!_disableGoogleAdsAndroidAppId)
            {
                Label("GoogleAds Android AppId: ");
                CheckChanges(_googleAdsAndroidAppId, value => TextField(value), newValue =>
                {
                    _settingsService.AndroidGoogleAdsAppId = newValue;
                    _googleAdsAndroidAppId = newValue;
                });
            }

            Space(16);

            Label("GoogleAds IOS AppId: ");
            CheckChanges(_googleAdsIOSAppId, value => TextField(value), newValue =>
            {
                _settingsService.IOSGoogleAdsAppId = newValue;
                _googleAdsIOSAppId = newValue;
            });

            Space(16);

            BeginHorizontal();
            Label("Skip Swift Configuration: ");
            CheckChanges(_skipSwiftConfiguration, value => Toggle(value), newValue =>
            {
                _settingsService.IsSkipSwiftConfiguration = newValue;
                _skipSwiftConfiguration = newValue;
            });
            GUILayout.FlexibleSpace();
            EndHorizontal();

            Space(16);

            BeginHorizontal();
            Label("Skip SkAdNetwork Ids Configuration: ");
            CheckChanges(_skipSkAdNetworkIdsConfiguration, value => Toggle(value), newValue =>
            {
                _settingsService.IsSkipSkAdNetworkIdsConfiguration = newValue;
                _skipSkAdNetworkIdsConfiguration = newValue;
            });
            GUILayout.FlexibleSpace();
            EndHorizontal();
            
            Space(16);
            
            BeginHorizontal();
            Label("MetaMediation: ");
            CheckChanges(_isMetaMediation, value => Toggle(value), newValue =>
            {
                _settingsService.IsMetaMediation = newValue;
                _isMetaMediation = newValue;
                UtilsMetaMediation.ApplyMetaMediation(!newValue);
            });
            GUILayout.FlexibleSpace();
            EndHorizontal();
            
            Space(16);

            BeginHorizontal();
            Label("Enable GMA properties tag fix: ");
            CheckChanges(_enableGmaPropertiesTagFix, value => Toggle(value), newValue =>
            {
                _settingsService.IsGmaPropertiesTagFixEnabled = newValue;
                _enableGmaPropertiesTagFix = newValue;
            });
            GUILayout.FlexibleSpace();
            EndHorizontal();

            if (_enableGmaPropertiesTagFix)
            {
                Label("[REQUIRED] Add the following code snippet to your mainTemplate.gradle");
                TextArea("gradle.projectsEvaluated {\n    apply from: 'XMediatorPlugin.androidlib/validate_dependencies.gradle'\n}");
            }
        }

        private void CheckChanges<T>(T currentValue, Func<T, T> render, Action<T> onChanged)
        {
            var newValue = render(currentValue);
            if (newValue.Equals(currentValue)) return;
            onChanged(newValue);
        }

        private static void Label(string text) => GUILayout.Label(text);

        private static void HorizontalLine()
        {
            const float thickness = 0.5f;
            const int padding = 5;
            var rect = GetControlRect(GUILayout.Height(padding + thickness));
            rect.height = thickness;
            rect.y += padding / 2;
            rect.x -= 2;
            rect.width += 6;
            EditorGUI.DrawRect(rect, Color.gray);
        }
    }
}