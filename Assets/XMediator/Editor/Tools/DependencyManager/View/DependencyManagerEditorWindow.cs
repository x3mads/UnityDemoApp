using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XMediator.Editor.Tools.DependencyManager.Entities;
using XMediator.Editor.Tools.MetaMediation;
using XMediator.Editor.Tools.MetaMediation.View;
using XMediator.Editor.Tools.Settings;
using static UnityEngine.GUI;
using static UnityEngine.GUILayout;

namespace XMediator.Editor.Tools.DependencyManager.View
{
    internal class DependencyManagerEditorWindow : EditorWindow, DependencyManagerView
    {
        private Vector2 scrollPosition;
        private GUIStyle titleLabelStyle;
        private GUIStyle headerLabelStyle;
        private GUIStyle statusLabelStyle;
        private GUIStyle dependencyLabelStyle;
        private GUIStyle nativeDependencyLabelStyle;
        private GUILayoutOption versionWidth;
        private GUILayoutOption versionListWidth;
        private int minVersionWidth = 220;
        private int dependencyButtonWidth = 90;
        private int _removeButtonWidth = 30;
        private int spaceWidth = 4;
        private int documentationWith = 200;
        private Texture2D trashTexture;
        private const string documentationUrl = "https://docs.x3mads.com/unity/changelogs/xmediator_changelog/";
        private DependencyManagerViewState State { get; set; } = DependencyManagerViewState.Blank;
        private DependencyManagerPresenter Presenter { get; set; }

        private bool IsShowingProgressBar { get; set; }
        private const long ProgressBarEstimatedTime = 1000;
        private long ProgressBarEndTime { get; set; }

        private void SetupEditorWindow()
        {
            titleLabelStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold
            };

            headerLabelStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold
            };

            statusLabelStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 14,
                fontStyle = FontStyle.Italic
            };

            dependencyLabelStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 13
            };

            nativeDependencyLabelStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 11
            };
        }

        private void CreateGUI()
        {
            minSize = new Vector2(600, 600);
            trashTexture = EditorGUIUtility.FindTexture("TreeEditor.Trash");
            State = DependencyManagerViewState.Blank;
            Presenter = new DependencyManagerPresenter(this);
            Presenter.Initialize();
            
            if (!Presenter.IsMetaMediation()) return;
            UtilsMetaMediation.ApplyMetaMediation(false);
        }

        private void OnGUI()
        {
            // Handling of a case where a user updates XMediator core from 1.2.0 to 1.3.0 or later, and the older
            // version did not have the label style properties set
            if (titleLabelStyle == null)
            {
                SetupEditorWindow();
            }
            CalculateWidth();
            
            if (Presenter.IsMetaMediation())
            {
                ShowNewDependencyMenu();
                ProgressBar();
                return;
            }
            
            using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPosition, false, false))
            {
                scrollPosition = scrollView.scrollPosition;
                MainSection();
                Sdk();
                if (!Presenter.IsMetaMediation())
                {
                    AdaptersInstalled();
                    AdaptersNotInstalled();
                }
                FlexibleSpace();
                ProgressBar();
            }
        }
        
        public void OnInspectorUpdate()
        {
            if (IsShowingProgressBar)
            {
                Repaint();   
            }
        }

        private void CalculateWidth()
        {
            var currentWidth =
                position.width - spaceWidth * 2 - dependencyButtonWidth - _removeButtonWidth -
                30; // 30 is needed for now to adjust width
            var versionLabelWidth = Math.Max(currentWidth / 2, minVersionWidth);
            versionWidth = Width(versionLabelWidth);
            versionListWidth = Width(60);
        }

        void ShowNewDependencyMenu()
        {
            BeginVertical("box");
            Label("From now on you should update the SDK from the new Dependency Manager Menu:", headerLabelStyle);
            Space(10);
            if (Button("Go", Height(30), Width(dependencyButtonWidth)))
            {
                EditorApplication.ExecuteMenuItem(MetaMediationEditorWindow.DependencyManagerMenu);
                Close();
            }
            EndVertical();
        }
        
        private void MainSection()
        {
            Space(10);
            using (new EditorGUILayout.HorizontalScope())
            {
                Title();
                Space(20);
                VersionsStatus();
                Space(10);
                Export();
            }
        }

        private void Title()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                Label("Dependencies status", titleLabelStyle);
                Space(30);
            }
        }

        private void Header()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                Label("Current Version", headerLabelStyle, versionWidth);
                Label("Available Versions", headerLabelStyle, versionWidth);
            }
        }

        private void Sdk()
        {
            if (State.Sdk == null) return;
            Label("Core", titleLabelStyle);
            Header();
            HorizontalLine();
            DependencyView(State.Sdk, (updateVersion) => { Presenter.UpdateSdkLastVersion(State.Sdk, updateVersion); });
            HorizontalLine();
            Space(30);
        }

        private void AdaptersInstalled()
        {
            if (!State.Adapters.Any()) return;
            Label("Adapters Installed", titleLabelStyle);
            Header();
            HorizontalLine();
            DrawAdapters(InstalledAdapters);
            Space(20);
        }

        private void DrawAdapters(Predicate<Dependency> filter)
        {
            var adapters = State.Adapters.ToList().FindAll(filter);
            foreach (var dependency in adapters)
            {
                DependencyView(dependency,
                    (updateVersion) => { Presenter.UpdateDependencyLastVersion(dependency, updateVersion); },
                    removeOption: true);
                HorizontalLine();
            }
        }

        private void AdaptersNotInstalled()
        {
            if (!State.Adapters.Any()) return;
            Label("Adapters Not Installed", titleLabelStyle);
            Header();
            HorizontalLine();
            DrawAdapters(NotInstalledAdapters);
        }

        private bool NotInstalledAdapters(Dependency dependency)
        {
            return !dependency.IsInstalled;
        }

        private bool InstalledAdapters(Dependency dependency)
        {
            return dependency.IsInstalled;
        }

        private void DependencyView(Dependency dependency, Action<AvailableVersion> onUpdateVersion,
            bool removeOption = false)
        {
            EditorGUILayout.BeginHorizontal();

            Space(spaceWidth);

            DependencyVersion(dependency);

            DependencyWithVersionsPopup(dependency.Name, dependency.LatestVersion?.ToString(),
                dependency.LatestIOSVersion,
                dependency.LatestAndroidVersion,
                dependency.AvailableVersions,
                onUpdateVersion: onUpdateVersion);

            DependencyButton(
                text: InstallDependencyButtonText(dependency),
                hint: InstallDependencyButtonHint(dependency),
                isEnabled: dependency.IsUpdateAvailable || !dependency.IsInstalled,
                onClick: () =>
                {
                    if (dependency.Name.StartsWith("TurnKey"))
                    {
                        var result = DisplayDialogOptions(
                            dialogTitle: $"Adding TurnKey Adapter",
                            message:
                            $"This action will remove any previously installed adapters to avoid compatibility issues. Do you want to continue?",
                            "Yes", "Cancel");
                        if (result != 0) return;
                        Presenter.ApplyTurnKey(dependency.Name);
                    }
                    else if (Presenter.IsATurnKeyInstalled())
                    {
                        DisplayDialog("Compatibility issue",
                            "You should remove any TurnKey adapter before installing another adapter to avoid compatibility issues.");
                        return;
                    }

                    Presenter.UpdateDependency(dependency);
                });
            if (removeOption)
            {
                enabled = dependency.IsInstalled && !State.IsDownloading;
                if (Button(content: new GUIContent(trashTexture, tooltip: $"Remove {dependency.Name} dependency"),
                        Width(_removeButtonWidth), ExpandHeight(true)))
                {
                    Presenter.Remove(dependency);
                }
                enabled = true;
            }

            Space(spaceWidth);
            EditorGUILayout.EndHorizontal();

            CompliantRulesMessage(dependency);
        }

        private static string InstallDependencyButtonHint(Dependency dependency)
        {
            return dependency.IsUpdateAvailable
                ? $"This will change {dependency.Name} to version {dependency.LatestVersion}"
                : dependency.IsInstalled
                    ? InstalledVersionHint(dependency)
                    : null;
        }

        private static string InstallDependencyButtonText(Dependency dependency)
        {
            string message;
            if (dependency.InstalledVersion == null)
            {
                message = "Install";
            }
            else if (dependency.LatestVersion == null)
            {
                message = "Installed";
            }
            else if (dependency.InstalledVersion.IsLowerThan(dependency.LatestVersion))
            {
                message = "Update";
            }
            else if (dependency.InstalledVersion.NotEquals(dependency.LatestVersion))
            {
                message = "Downgrade";
            }
            else
            {
                message = "Up to date";
            }

            return message;
        }

        private void CompliantRulesMessage(Dependency dependency)
        {
            if (dependency.NonCompliantRules == null || dependency.NonCompliantRules.Length == 0 ||
                !dependency.IsInstalled)
            {
                return;
            }

            EditorGUILayout.BeginHorizontal();
            Space(spaceWidth);
            var depsString = String.Join(", ", dependency.NonCompliantRules);
            var message =
                $"{dependency.Name} {dependency.InstalledVersion} requires the following compatible dependencies to avoid integration problems (Refer to the documentation for more information)\n{depsString}";
            EditorGUILayout.HelpBox(message, MessageType.Error);
            Space(spaceWidth);
            if (Button("Open Documentation", Width(documentationWith), ExpandHeight(true)))
            {
                Application.OpenURL(documentationUrl);
            }

            Space(spaceWidth);
            EditorGUILayout.EndHorizontal();
        }

        private void DependencyVersion(Dependency dependency)
        {
            using (new EditorGUILayout.HorizontalScope("box", versionWidth))
            {
                using (new EditorGUILayout.VerticalScope("box"))
                {
                    Label($"{dependency.Name} {dependency.InstalledVersion?.ToString() ?? "-"}", dependencyLabelStyle);
                    Label(
                        $"Android: {dependency.InstalledAndroidVersion ?? "-"}, iOS: {dependency.InstalledIOSVersion ?? "-"}",
                        nativeDependencyLabelStyle);
                }
            }
        }

        private void DependencyWithVersionsPopup(string dependencyName, string selectVersion, string iOSVersion,
            string androidVersion,
            IEnumerable<AvailableVersion> dependencyAvailableVersions, Action<AvailableVersion> onUpdateVersion)
        {
            using (new EditorGUILayout.VerticalScope("box", versionWidth))
            {
                using (new EditorGUILayout.HorizontalScope("box"))
                {
                    var width = dependencyName.Length * 8;
                    Label($"{dependencyName}", dependencyLabelStyle, Width(width), MinWidth(30));
                    var availableVersions = dependencyAvailableVersions.ToList();
                    if (availableVersions.Any())
                        ListVersionsPopup(availableVersions, selectVersion, onUpdateVersion);
                }

                Label($"Android: {androidVersion ?? "-"}, iOS: {iOSVersion ?? "-"}", nativeDependencyLabelStyle);
            }
        }

        private void ListVersionsPopup(IEnumerable<AvailableVersion> dependencyAvailableVersions, string selectVersion,
            Action<AvailableVersion> onUpdateVersion)
        {
            var dependencyAvailableVersionsList = dependencyAvailableVersions.ToList();
            var currentOptionIndex = 0;
            var unityOptions = new List<GUIContent>();
            for (var i = 0; i < dependencyAvailableVersionsList.Count; i++)
            {
                var extraSpace = "";
                var version = dependencyAvailableVersionsList[i].Version.ToString();
                if (version.Length < 6)
                {
                    extraSpace = "  ";
                }
                else if (version.Length < 7)
                {
                    extraSpace = " ";
                }

                var androidVersion = dependencyAvailableVersionsList[i].AndroidVersion;
                var iosVersion = dependencyAvailableVersionsList[i].IOSVersion;
                var nativeVersions = $" Android: {androidVersion}  |  iOS: {iosVersion}";
                unityOptions.Add(new GUIContent($"{extraSpace}{version}    -> {nativeVersions}", $"Select version"));
                if (version == selectVersion)
                    currentOptionIndex = i;
            }

            var selectedOptionIndex =
                EditorGUILayout.Popup(
                    selectedIndex: currentOptionIndex, displayedOptions: unityOptions.ToArray(), versionListWidth);
            if (currentOptionIndex != selectedOptionIndex)
                onUpdateVersion(dependencyAvailableVersionsList[selectedOptionIndex]);
        }

        private void ProgressBar()
        {
            if (State.IsDownloading)
            {
                var currentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

                if (!IsShowingProgressBar)
                {
                    IsShowingProgressBar = true;
                    ProgressBarEndTime = currentTime + ProgressBarEstimatedTime;
                }

                var progress = (ProgressBarEstimatedTime - (ProgressBarEndTime - currentTime)) /
                               (float)ProgressBarEstimatedTime;
                var isCancelled = EditorUtility.DisplayCancelableProgressBar(
                    title: "XMediator Dependency Manager",
                    info: "Downloading dependency...",
                    progress: Math.Min((float)EaseOutQuint(progress), 0.8f));
                if (isCancelled)
                {
                    Presenter.CancelDownload();
                }
            }
            else if (IsShowingProgressBar)
            {
                IsShowingProgressBar = false;
                EditorUtility.ClearProgressBar();
            }
        }

        private void VersionsStatus()
        {
            using (new EditorGUILayout.VerticalScope("box"))
            {
                AvailableVersionsStatus();
                InstalledVersionsStatus();
            }

            BrokenDependenciesStatusMessage();
        }

        private void Export()
        {
            EnableButton("Export", () => Presenter.ExportInstalledVersions(),
                Width(dependencyButtonWidth),
                Height(30),
                "Export installed XMediator dependencies",
                !State.ShowRetryLoadInstalledVersions);
        }

        public void SaveJsonFile(string viewTitle, string fileName, string json)
        {
            EditorGUIUtility.systemCopyBuffer = json;
            var path = EditorUtility.SaveFilePanel(
                viewTitle,
                "",
                fileName,
                "json");

            if (path.Length != 0)
            {
                if (json != null)
                    File.WriteAllText(path, json);
            }
        }

        private void AvailableVersionsStatus()
        {
            if (State.AvailableVersionsStatusMessage == null) return;
            EditorGUILayout.BeginHorizontal();
            Label(State.AvailableVersionsStatusMessage, statusLabelStyle);
            if (State.ShowRetryFetchAvailableVersions)
            {
                Space(10);
                RetryButton(
                    text: "Retry",
                    onClick: () => Presenter.RetryFetchAvailableVersions());
            }

            FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void InstalledVersionsStatus()
        {
            if (State.InstalledVersionsStatusMessage == null) return;
            EditorGUILayout.BeginHorizontal();
            Label(State.InstalledVersionsStatusMessage, statusLabelStyle);
            if (State.ShowRetryLoadInstalledVersions)
            {
                Space(10);
                RetryButton(
                    text: "Retry",
                    onClick: () => Presenter.RetryLoadInstalledVersions());
            }

            FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void BrokenDependenciesStatusMessage()
        {
            if (!State.ShowBrokenDependenciesStatusMessage) return;
            using (new EditorGUILayout.HorizontalScope())
            {
                Space(spaceWidth);
                EditorGUILayout.HelpBox(State.BrokenDependenciesStatusMessage, MessageType.Warning);
                Space(spaceWidth);
            }
        }

        private static string InstalledVersionHint(Dependency dependency) =>
            $"Installed version: {dependency.Name} {dependency.InstalledVersion}" +
            (dependency.InstalledVersion?.Equals(dependency.LatestVersion) == true
                ? " (latest)"
                : "");

        public void SetViewState(DependencyManagerViewState state)
        {
            State = state;
            Repaint();
        }

        public void DisplayDialog(string title, string message)
        {
            EditorUtility.DisplayDialogComplex(
                title: title,
                message: message,
                ok: "Close", "", "");
        }

        public int DisplayDialogOptions(string dialogTitle, string message, string ok, string cancel)
        {
            return EditorUtility.DisplayDialogComplex(
                title: dialogTitle,
                message: message,
                ok: ok, cancel: cancel, "");
        }

        private void RetryButton(string text, Action onClick)
        {
            EnableButton(text, onClick, Width(dependencyButtonWidth), Height(30), null, true);
        }

        private void DependencyButton(string text, Action onClick, string hint, bool isEnabled)
        {
            EnableButton(text, onClick, Width(dependencyButtonWidth), ExpandHeight(true), hint, isEnabled);
        }

        private void EnableButton(string text, Action onClick, GUILayoutOption width, GUILayoutOption height,
            string hint, bool isEnabled)
        {
            enabled = isEnabled && !State.IsDownloading;

            var isClicked = hint != null
                ? Button(new GUIContent(text, hint), width, height)
                : Button(text, width, height);
            if (isClicked) onClick();
            enabled = true;
        }

        private static void HorizontalLine()
        {
            const float thickness = 0.5f;
            const int padding = 5;
            var rect = EditorGUILayout.GetControlRect(Height(padding + thickness));
            rect.height = thickness;
            rect.y += padding / 2;
            rect.x -= 2;
            rect.width += 6;
            EditorGUI.DrawRect(rect, Color.gray);
        }

        private static double EaseOutQuint(float x) => 1 - Math.Pow(1 - x, 5);
    }
}