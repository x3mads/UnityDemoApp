using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using XMediator.Editor.Tools.MetaMediation.Actions;
using XMediator.Editor.Tools.MetaMediation.Entities;
using XMediator.Editor.Tools.MetaMediation.Repository.Dto;

namespace XMediator.Editor.Tools.MetaMediation.View
{
    internal class MetaMediationEditorWindow : EditorWindow, MetaMediationManagerView
    {
        private MetaMediationViewState State { get; set; } = new(null, true);

        private const string X3MMediatorName = "X3M";
        private const long ProgressBarEstimatedTime = 2000;
        private const int NumColumns = 3;
        internal const string DependencyManagerMenu = "XMediator/Mediators Dependency Manager";

        private bool _isAndroidSelected = true;
        private bool _isIosSelected = true;

        private readonly Dictionary<string, bool> _mediatorSelection = new()
        {
            { X3MMediatorName, true }
        };

        private readonly Dictionary<string, bool> _adNetworkSelection = new();

        private string _adNetworkSearchTerm = "";

        private MetaMediationPresenter Presenter { get; set; }

        private bool IsShowingProgressBar { get; set; }
        private long ProgressBarEndTime { get; set; }

        [MenuItem(DependencyManagerMenu, false, 1)]
        public static void ShowWindow()
        {
            GetWindow<MetaMediationEditorWindow>("Dependencies Generator");
        }

        private void InitializeSelections()
        {
            if (State.Dependencies == null) return;
            foreach (var mediator in State.Dependencies.Mediations)
            {
                _mediatorSelection.TryAdd(mediator, false);
            }

            foreach (var network in State.Dependencies.Networks)
            {
                _adNetworkSelection.TryAdd(network, false);
            }
        }

        private void CreateGUI()
        {
            minSize = new Vector2(675, 500);
            maxSize = new Vector2(675, 500);
            Presenter = new MetaMediationPresenter(this);
            Presenter.Initialize();
            InitializeSelections();
        }

        private void OnGUI()
        {
            if (State.Dependencies != null)
            {
                PlatformPicker();
                MediatorsPicker();
                NetworkPicker();
                GenerateDependenciesButton();
            }

            ProgressBar();
            DisplayUpdateResults();
        }

        public void OnInspectorUpdate()
        {
            if (IsShowingProgressBar)
            {
                Repaint();
            }
        }

        public void SetViewState(MetaMediationViewState state)
        {
            State = state;
            if (!Presenter.CurrentPlatforms.Any()) return;
            _isAndroidSelected = Presenter.CurrentPlatforms.Contains("android");
            _isIosSelected = Presenter.CurrentPlatforms.Contains("ios");
        }

        public void OnGeneratedDependencies(bool isError, string message)
        {
            var currentMessage = "Success";
            var dialogTitle = "Dependencies Generation";
            if (isError)
            {
                currentMessage = message ?? "Something went wrong, try again later";
                dialogTitle = "Error";
            }

            EditorUtility.DisplayDialog(dialogTitle, currentMessage, "OK");
        }

        public void OnDialogWithCloseWindow(string dialogTitle, string message)
        {
            EditorUtility.ClearProgressBar();
            var result = EditorUtility.DisplayDialog(dialogTitle, message, "OK");
            if (result)
            {
                Close();
            }
        }

        public bool OnConfirmationDialog(string dialogTitle, string message)
        {
            return EditorUtility.DisplayDialog(dialogTitle, message, "OK", "Cancel");
        }

        public void OnRefreshUi()
        {
            Repaint();
        }

        private void GenerateDependenciesButton()
        {
            if (GUILayout.Button("Generate Dependencies", GUILayout.Height(30)))
            {
                GenerateDependenciesAction();
            }
        }

        private static int DisplayDialogOptions(string dialogTitle, string message, string ok, string cancel)
        {
            return EditorUtility.DisplayDialogComplex(
                title: dialogTitle,
                message: message,
                ok: ok, cancel: cancel, "");
        }

        private void PlatformPicker()
        {
            GUILayout.Space(10);
            GUILayout.Label("Platform Selection", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();

            _isAndroidSelected = GUILayout.Toggle(_isAndroidSelected, "Android", "Button", GUILayout.Height(25));
            _isIosSelected = GUILayout.Toggle(_isIosSelected, "iOS", "Button", GUILayout.Height(25));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
        }

        private void NetworkPicker()
        {
            GUILayout.Label("Ad Network Selection", EditorStyles.boldLabel);
            GUILayout.BeginVertical("box");

            GUILayout.Space(10);
            _adNetworkSearchTerm = EditorGUILayout.TextField("Search", _adNetworkSearchTerm, GUILayout.MaxWidth(400));
            GUILayout.Space(10);
            var adNetworks = State.Dependencies.Networks.ToList();
            foreach (var network in adNetworks.Where(network => !_adNetworkSelection.ContainsKey(network)))
            {
                _adNetworkSelection.Add(network, Presenter.CurrentNetworks.Contains(network));
            }

            var sortedAdNetworks = _adNetworkSelection
                .OrderBy(network => network.Key)
                .Where(network => network.Key.ToLower().Contains(_adNetworkSearchTerm.ToLower()))
                .ToList();

            for (var i = 0; i < sortedAdNetworks.Count; i += NumColumns)
            {
                GUILayout.BeginHorizontal();
                for (var j = 0; j < NumColumns; j++)
                {
                    var index = i + j;
                    if (index >= sortedAdNetworks.Count) continue;
                    var key = sortedAdNetworks[index].Key;
                    _adNetworkSelection[key] = EditorGUILayout.ToggleLeft(key, _adNetworkSelection[key],
                        GUILayout.MaxWidth(position.width / NumColumns - 10));
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();

            GUILayout.Space(20);
            UpdateSavedParams();
        }

        private void MediatorsPicker()
        {
            GUILayout.Label("Mediator Selection", EditorStyles.boldLabel);
            GUILayout.BeginVertical("box");
            var mediators = new List<string>() { X3MMediatorName };
            mediators.AddRange(State.Dependencies.Mediations.ToList());
            foreach (var mediator in mediators)
            {
                _mediatorSelection.TryAdd(mediator, Presenter.CurrentMediators.Contains(mediator));
            }

            foreach (var key in _mediatorSelection.Keys.ToList())
            {
                _mediatorSelection[key] = EditorGUILayout.ToggleLeft(key, _mediatorSelection[key] || key.Equals("X3M"));
            }

            GUILayout.EndVertical();

            GUILayout.Space(10);
        }

        private void UpdateSavedParams()
        {
            if (State.SavedMediations != null)
            {
                foreach (var stateSavedMediation in State.SavedMediations)
                {
                    Debug.Log("Saved mediation: " + stateSavedMediation);
                    _mediatorSelection.TryAdd(stateSavedMediation, true);
                }
            }

            if (State.SavedNetworks != null)
            {
                foreach (var stateSavedNetwork in State.SavedNetworks)
                {
                    Debug.Log("Saved network: " + stateSavedNetwork);
                    _adNetworkSelection.TryAdd(stateSavedNetwork, true);
                }
            }

            if (State.SavedPlatforms == null) return;
            foreach (var stateSavedPlatform in State.SavedPlatforms)
            {
                Debug.Log("Saved platform: " + stateSavedPlatform);
                if (stateSavedPlatform.Contains("android"))
                {
                    _isAndroidSelected = true;
                }
                else if (stateSavedPlatform.Contains("ios"))
                {
                    _isIosSelected = true;
                }
            }
        }

        private void GenerateDependenciesAction()
        {
            var result = DisplayDialogOptions(
                dialogTitle: $"Generating dependencies",
                message:
                $"This action will remove or update the previously generated dependencies. Do you wish to proceed?",
                "Yes", "Cancel");
            if (result != 0) return;
            Presenter.UpdateSelection(_mediatorSelection, _adNetworkSelection, _isAndroidSelected, _isIosSelected);
            Debug.Log("Generating dependencies");
            Presenter.GenerateAndSaveDependencies();
        }

        private void ProgressBar()
        {
            if (State.IsDownloading)
            {
                var progressTitle = "XMediator MetaMediation Manager";
                var progressInfo = "Downloading dependencies...";
                ShowProgressBar(progressTitle, progressInfo);
            }
            else if (IsShowingProgressBar)
            {
                IsShowingProgressBar = false;
                EditorUtility.ClearProgressBar();
            }
        }

        private void ShowProgressBar(string progressTitle, string progressInfo)
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
                title: progressTitle,
                info: progressInfo,
                progress: Math.Min((float)EaseOutQuint(progress), 0.8f));
            if (isCancelled)
            {
                Presenter.CancelDownload();
            }
        }


        private Vector2 _updateResultsScrollPosition;

        private void DisplayUpdateResults()
        {
            
            if (Presenter.UpdateResults == null && Presenter.CoreUpdateResult == null) return;
            var androidDiff = Presenter.UpdateResults?.Item1;
            var iosDiff = Presenter.UpdateResults?.Item2;

            GUILayout.Space(8);
            int heightOffset;

            if ((androidDiff != null && iosDiff != null && (androidDiff.Count > 0 || iosDiff.Count > 0)) || Presenter.CoreUpdateResult?.Status != ResolveCoreAdapterVersion.Status.NoChanges)
            {
                heightOffset = 200;
                GUILayout.BeginHorizontal();
                GUILayout.Label(EditorGUIUtility.IconContent("console.warnicon"), GUILayout.Width(20), GUILayout.Height(20));
                GUILayout.Label("You have updates (Run \"Generate dependencies\" to apply them)", EditorStyles.boldLabel);
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.Label("No updates available", EditorStyles.boldLabel);
                heightOffset = 25;
            }

            _updateResultsScrollPosition = GUILayout.BeginScrollView(_updateResultsScrollPosition, GUILayout.Width(500), GUILayout.Height(175));
            DisplayUpdateResultsScrollView(heightOffset, androidDiff, iosDiff);
            GUILayout.EndScrollView();
        }

        private void DisplayUpdateResultsScrollView(int heightOffset, Dictionary<MetaMediationDependencies.AndroidPackage, DependencyDto> androidDiff, Dictionary<MetaMediationDependencies.IosPod, IOSDependencyDto> iosDiff)
        {
            if (Presenter.CoreUpdateResult != null && Presenter.CoreUpdateResult.Status != ResolveCoreAdapterVersion.Status.NoChanges)
            {
                var status = Presenter.CoreUpdateResult.Status.ToString();
                GUILayout.Space(8);
                GUILayout.Label("Unity Package Update:", EditorStyles.boldLabel);
                GUILayout.Space(6);
                GUILayout.BeginVertical("box");
                GUILayout.Label($"{status} {Presenter.CoreUpdateResult.PreviousVersion} -> {Presenter.CoreUpdateResult.Version.Version.Version}",
                    new GUIStyle(EditorStyles.label) { richText = true, wordWrap = true });
                GUILayout.EndVertical();
                heightOffset += 50;
            }

            minSize = new Vector2(675, 500 + heightOffset);
            maxSize = new Vector2(675, 500 + heightOffset);


            switch (androidDiff)
            {
                case {Count: <= 0} when iosDiff is {Count: <= 0}:
                {
                    return;
                }
                case { Count: > 0 }:
                {
                    GUILayout.Space(8);
                    GUILayout.Label("Android Updates:", EditorStyles.boldLabel);
                    GUILayout.Space(6);
                    GUILayout.BeginVertical("box");
                    foreach (var kvp in androidDiff)
                    {
                        GUILayout.Label($"{kvp.Key.Group}:<b>{kvp.Key.Artifact}:</b>{kvp.Key.Version} -> {kvp.Value.suggested_version}",
                            new GUIStyle(EditorStyles.label) { richText = true, wordWrap = true });
                        GUILayout.Space(3);
                    }

                    GUILayout.EndVertical();
                    break;
                }
            }

            GUILayout.Space(10);
            if (iosDiff is { Count: > 0 })
            {
                GUILayout.Label("iOS Updates:", EditorStyles.boldLabel);
                GUILayout.Space(6);
                GUILayout.BeginVertical("box");
                foreach (var kvp in iosDiff)
                {
                    GUILayout.Label($"<b>{kvp.Key.Name}</b> {kvp.Key.Version} -> {kvp.Value.suggested_version}", new GUIStyle(EditorStyles.label) { richText = true, wordWrap = true });
                    GUILayout.Space(3);
                }

                GUILayout.EndVertical();
            }
        }

        private static double EaseOutQuint(float x) => 1 - Math.Pow(1 - x, 5);
    }
}