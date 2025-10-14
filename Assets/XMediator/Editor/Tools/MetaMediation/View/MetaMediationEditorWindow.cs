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

        private const long ProgressBarEstimatedTime = 2000;
        private const int BaseHeight = 520;
        internal const string DependencyManagerMenu = "XMediator/Mediators Dependency Manager";

        private bool _isAndroidSelected = true;
        private bool _isIosSelected = true;

        private readonly Dictionary<string, bool> _mediatorSelection = new();
        private readonly Dictionary<string, bool> _adNetworkSelection = new();
        private readonly Dictionary<string, bool> _previousAdNetworkSelection = new();
        private readonly Dictionary<string, bool> _previousMediatorSelection = new();

        private string _adNetworkSearchTerm = "";

        private MetaMediationPresenter Presenter { get; set; }

        private bool IsShowingProgressBar { get; set; }
        private long ProgressBarEndTime { get; set; }
        
        private Vector2 _lastWindowSize;

        [MenuItem(DependencyManagerMenu, false, 0)]
        public static void ShowWindow()
        {
            GetWindow<MetaMediationEditorWindow>("Dependencies Generator");
        }

        private void InitializeSelections()
        {
            if (State.SelectableDependencies == null) return;
            foreach (var mediator in State.SelectableDependencies.Mediations)
            {
                _mediatorSelection.TryAdd(mediator, false);
            }

            foreach (var network in State.SelectableDependencies.Networks)
            {
                _adNetworkSelection.TryAdd(network, false);
                _previousAdNetworkSelection.TryAdd(network, false);
            }
            
            foreach (var mediator in State.SelectableDependencies.Mediations)
            {
                _previousMediatorSelection.TryAdd(mediator, false);
            }
        }

        private void CreateGUI()
        {
            minSize = new Vector2(675, BaseHeight);
            Presenter = new MetaMediationPresenter(this);
            Presenter.Initialize();
            InitializeSelections();
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Current Versions Checker", GUILayout.Width(160)))
            {
                VersionsCheckerEditorWindow.ShowWindow();
            }
            GUILayout.EndHorizontal();

            if (State.SelectableDependencies != null)
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
            
            // Initialize previous network and mediator selection with current state
            InitializePreviousNetworkSelection();
            InitializePreviousMediatorSelection();
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

            GUILayout.BeginHorizontal("box");

            GUILayout.BeginVertical();
            _isAndroidSelected = GUILayout.Toggle(_isAndroidSelected, "Android", "Button", GUILayout.Height(25), GUILayout.MaxWidth(position.width / 2));
            GUILayout.EndVertical();
            
            GUILayout.BeginVertical();
            _isIosSelected = GUILayout.Toggle(_isIosSelected, "iOS", "Button", GUILayout.Height(25), GUILayout.MaxWidth(position.width / 2));
            if (_isIosSelected)
            {
                IOSLegacySupportTagsPicker();
            }
            GUILayout.EndVertical();
            
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
        }
        
        private void IOSLegacySupportTagsPicker()
        {
            var tagOptions = State.SelectableDependencies.IOSManifest.GetLegacySupportTags();
            if (tagOptions.Count > 0)
            {
                tagOptions.Insert(0, "None"); // We add the "None" option to be selected as default
                var currentIndex = Math.Max(tagOptions.IndexOf(Presenter.CurrentTags.IOS), 0);
               
                var newIndex = RenderIOSTags(tagOptions, currentIndex);
                if (newIndex != currentIndex)
                {
                    var selectedTag = newIndex > 0 ? tagOptions[newIndex] : null; // We filter the "None" option
                    Presenter.SelectIOSTag(selectedTag);
                }
            }
        }

        private int RenderIOSTags(List<string> tagOptions, int currentIndex)
        {
            var legacyTooltip =
                "Select to generate dependencies that support an older Xcode version, if you can't upgrade to a newer one right now.";
            var displayedOptions = GetDisplayedTagOptions(tagOptions, legacyTooltip);
                
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Legacy Support:", legacyTooltip));
            var newIndex = EditorGUILayout.Popup(selectedIndex: currentIndex, displayedOptions: displayedOptions);
            GUILayout.EndHorizontal();
            
            return newIndex;
        }

        private static GUIContent[] GetDisplayedTagOptions(List<string> tagOptions, string legacyTooltip)
        {
            return tagOptions.Select(t => new GUIContent(MapTagToReadableString(t), legacyTooltip)).ToArray();
        }
        
        private static string MapTagToReadableString(string tag)
        {
            return tag.Replace("xcode", "Xcode")
                .Replace("ios", "iOS")
                .Replace("_", " ");
        }

        private void NetworkPicker()
        {
            GUILayout.Label("Ad Network Selection", EditorStyles.boldLabel);
            GUILayout.BeginVertical("box");

            GUILayout.Space(10);
            _adNetworkSearchTerm = EditorGUILayout.TextField("Search", _adNetworkSearchTerm, GUILayout.MaxWidth(400));
            GUILayout.Space(10);
            var adNetworks = State.SelectableDependencies.Networks.ToList();
            foreach (var network in adNetworks.Where(network => !_adNetworkSelection.ContainsKey(network)))
            {
                _adNetworkSelection.Add(network, Presenter.CurrentNetworks.Contains(network));
            }

            var sortedAdNetworks = _adNetworkSelection
                .OrderBy(network => network.Key)
                .Where(network => network.Key.ToLower().Contains(_adNetworkSearchTerm.ToLower()))
                .ToList();

            // Calculate columns based on window width
            int columns = Mathf.Max(1, Mathf.FloorToInt(position.width / 225));
            
            bool networkChanged = false;
            for (var i = 0; i < sortedAdNetworks.Count; i += columns)
            {
                GUILayout.BeginHorizontal();
                for (var j = 0; j < columns; j++)
                {
                    var index = i + j;
                    if (index >= sortedAdNetworks.Count) continue;
                    var key = sortedAdNetworks[index].Key;
                    var previousValue = _adNetworkSelection[key];
                    _adNetworkSelection[key] = EditorGUILayout.ToggleLeft(key, _adNetworkSelection[key],
                        GUILayout.MaxWidth(position.width / columns - 10));
                    if (previousValue != _adNetworkSelection[key])
                    {
                        networkChanged = true;
                    }
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();

            GUILayout.Space(20);
        }

        private void MediatorsPicker()
        {
            GUILayout.Label("Mediator Selection", EditorStyles.boldLabel);
            GUILayout.BeginVertical("box");
            
            if (Presenter.CurrentMediators.Count == 0)
            {
                _mediatorSelection.TryAdd(SelectableDependencies.X3MMediationName, true);
            }
            foreach (var mediator in State.SelectableDependencies.Mediations.ToList())
            {
                _mediatorSelection.TryAdd(mediator, Presenter.CurrentMediators.Contains(mediator));
            }

            var dirty = false;
            foreach (var key in _mediatorSelection.Keys.ToList())
            {
                var previosValue = _mediatorSelection[key];
                _mediatorSelection[key] = EditorGUILayout.ToggleLeft(key, _mediatorSelection[key]);
                dirty = dirty || _mediatorSelection[key] != previosValue;
            }

            GUILayout.EndVertical();

            GUILayout.Space(10);

            if (dirty)
            {
                Presenter.RefreshUpdatesWithNewSelection(_mediatorSelection, _adNetworkSelection, _isAndroidSelected, _isIosSelected);
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
            
            // Clear network and mediator changes after generating dependencies
            UpdatePreviousNetworkSelection();
            UpdatePreviousMediatorSelection();
            
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
            if (Presenter.UpdateResults == null && Presenter.CoreUpdateResult == null && !HasNetworkChanges() && !HasMediatorChanges()) return;
            
            var androidDiff = Presenter.UpdateResults?.Item1;
            var iosDiff = Presenter.UpdateResults?.Item2;

            GUILayout.Space(8);
            if (ShouldShowDependencyUpdates(androidDiff, iosDiff) || ShouldShowCoreUpdates() || HasNetworkChanges() || HasMediatorChanges())
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(EditorGUIUtility.IconContent("console.warnicon"), GUILayout.Width(20), GUILayout.Height(20));
                GUILayout.Label("You have updates (Run \"Generate dependencies\" to apply them)", EditorStyles.boldLabel);
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.Label("No updates available", EditorStyles.boldLabel);
            }

            // Calculate precise content height based on actual items to display
            int contentHeight = 0;
            
            // Network changes section height
            if (HasNetworkChanges())
            {
                var (addedNetworks, removedNetworks) = GetNetworkChanges();
                contentHeight += 40; // Section header + spacing
                contentHeight += (addedNetworks.Count + removedNetworks.Count) * 25; // Each item + spacing
                contentHeight += 20; // Box padding
            }
            
            // Mediator changes section height
            if (HasMediatorChanges())
            {
                var (addedMediators, removedMediators) = GetMediatorChanges();
                contentHeight += 40; // Section header + spacing
                contentHeight += (addedMediators.Count + removedMediators.Count) * 25; // Each item + spacing
                contentHeight += 20; // Box padding
            }
            
            // Core updates section height
            if (ShouldShowCoreUpdates())
            {
                contentHeight += 70; // Header + content + spacing
            }
            
            // Android updates section height (if any)
            if (androidDiff != null && androidDiff.Count > 0)
            {
                contentHeight += 40; // Section header + spacing
                contentHeight += androidDiff.Count * 25; // Each item + spacing
                contentHeight += 20; // Box padding
            }
            
            // iOS updates section height (if any)
            if (iosDiff != null && iosDiff.Count > 0)
            {
                contentHeight += 40; // Section header + spacing
                contentHeight += iosDiff.Count * 25; // Each item + spacing
                contentHeight += 20; // Box padding
            }
            
            contentHeight += 30; // Additional padding for safety
            
            // Precise calculation of available window space
            float headerHeight = 90; // Height of UI elements above the updates section
            int columns = Mathf.Max(1, Mathf.FloorToInt(position.width / 225)); // Number of columns for networks
            float additionalUIHeight = 260; // Other UI elements (platform selectors, etc.)
            float topUIHeight = headerHeight + additionalUIHeight;
            
            // Reserve space at bottom of window - increased to ensure content doesn't overflow
            float bottomMargin = Mathf.Max(135f,220 - (columns - 3) * 30);
            
            // Calculate available height
            float availableHeight = position.height - topUIHeight - bottomMargin;
            
            // Choose appropriate height for scrollview
            float scrollViewHeight = Mathf.Min(contentHeight, availableHeight);
            
            // Ensure minimum height
            scrollViewHeight = Mathf.Max(100f, scrollViewHeight);
            
            // First create a vertical layout to contain everything
            GUILayout.BeginVertical();
            
            // Create scrollview with calculated dimensions
            _updateResultsScrollPosition = EditorGUILayout.BeginScrollView(
                _updateResultsScrollPosition,
                GUILayout.Width(position.width - 20),
                GUILayout.Height(scrollViewHeight)
            );
            
            // Display content
            DisplayUpdateResultsScrollView(0, androidDiff, iosDiff);
            EditorGUILayout.EndScrollView();
            
            // Explicitly end vertical layout
            GUILayout.EndVertical();
            
            // Add explicit bottom margin to ensure content doesn't overflow
            GUILayout.Space(bottomMargin);
            
            // Monitor window size changes 
            if (Event.current.type == EventType.Layout)
            {
                Vector2 currentSize = position.size;
                if (_lastWindowSize != currentSize)
                {
                    _lastWindowSize = currentSize;
                    Repaint();
                }
            }
        }

        private bool ShouldShowCoreUpdates()
        {
            return Presenter.CoreUpdateResult != null && Presenter.CoreUpdateResult.Status != ResolveCoreAdapterVersion.Status.NoChanges;
        }

        private static bool ShouldShowDependencyUpdates(Dictionary<MetaMediationDependencies.AndroidPackage, AndroidDependencyDto> androidDiff, Dictionary<MetaMediationDependencies.IosPod, IOSDependencyDto> iosDiff)
        {
            return (androidDiff != null && iosDiff != null && (androidDiff.Count > 0 || iosDiff.Count > 0));
        }

        private void DisplayUpdateResultsScrollView(int contentHeight, Dictionary<MetaMediationDependencies.AndroidPackage, AndroidDependencyDto> androidDiff, Dictionary<MetaMediationDependencies.IosPod, IOSDependencyDto> iosDiff)
        {
            
            var filteredAndroidDiff = FilterAndroidUpdatesForRemovedNetworks(androidDiff);
            var filteredIosDiff = FilterIOSUpdatesForRemovedNetworks(iosDiff);
            
            if (HasMediatorChanges())
            {
                var (addedMediators, removedMediators) = GetMediatorChanges();
                GUILayout.Space(8);
                GUILayout.Label("Mediator Changes:", EditorStyles.boldLabel);
                GUILayout.Space(6);
                GUILayout.BeginVertical("box");
                
                foreach (var mediatorInfo in addedMediators)
                {
                    var versionText = GetMediatorVersionDisplayText(mediatorInfo);
                    GUILayout.Label($"<color=green>+ {mediatorInfo.MediatorName}</color> (Added){versionText}",
                        new GUIStyle(EditorStyles.label) { richText = true, wordWrap = true });
                    GUILayout.Space(3);
                }
                
                foreach (var mediatorInfo in removedMediators)
                {
                    GUILayout.Label($"<color=red>- {mediatorInfo.MediatorName}</color> (Removed)",
                        new GUIStyle(EditorStyles.label) { richText = true, wordWrap = true });
                    GUILayout.Space(3);
                }
                
                GUILayout.EndVertical();
            }
            
            if (HasNetworkChanges())
            {
                var (addedNetworks, removedNetworks) = GetNetworkChanges();
                GUILayout.Space(8);
                GUILayout.Label("Ad Network Changes:", EditorStyles.boldLabel);
                GUILayout.Space(6);
                GUILayout.BeginVertical("box");
                
                foreach (var networkInfo in addedNetworks)
                {
                    var versionText = GetVersionDisplayText(networkInfo);
                    GUILayout.Label($"<color=green>+ {networkInfo.NetworkName}</color> (Added){versionText}",
                        new GUIStyle(EditorStyles.label) { richText = true, wordWrap = true });
                    GUILayout.Space(3);
                }
                
                foreach (var networkInfo in removedNetworks)
                {
                    GUILayout.Label($"<color=red>- {networkInfo.NetworkName}</color> (Removed)",
                        new GUIStyle(EditorStyles.label) { richText = true, wordWrap = true });
                    GUILayout.Space(3);
                }
                
                GUILayout.EndVertical();
            }

            if (ShouldShowCoreUpdates())
            {
                var status = Presenter.CoreUpdateResult.Status.ToString();
                GUILayout.Space(8);
                GUILayout.Label("Unity Package Update:", EditorStyles.boldLabel);
                GUILayout.Space(6);
                GUILayout.BeginVertical("box");
                GUILayout.Label($"{status} {Presenter.CoreUpdateResult.PreviousVersion} -> {Presenter.CoreUpdateResult.Version.Version.Version}",
                    new GUIStyle(EditorStyles.label) { richText = true, wordWrap = true });
                GUILayout.EndVertical();
            }

            switch (filteredAndroidDiff)
            {
                case {Count: <= 0} when filteredIosDiff is {Count: <= 0}:
                {
                    return;
                }
                case { Count: > 0 }:
                {
                    GUILayout.Space(8);
                    GUILayout.Label("Android Updates:", EditorStyles.boldLabel);
                    GUILayout.Space(6);
                    GUILayout.BeginVertical("box");
                    foreach (var kvp in filteredAndroidDiff)
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
            if (filteredIosDiff is { Count: > 0 })
            {
                GUILayout.Label("iOS Updates:", EditorStyles.boldLabel);
                GUILayout.Space(6);
                GUILayout.BeginVertical("box");
                foreach (var kvp in filteredIosDiff)
                {
                    GUILayout.Label($"<b>{kvp.Key.Name}</b> {kvp.Key.Version} -> {kvp.Value.suggested_version}", new GUIStyle(EditorStyles.label) { richText = true, wordWrap = true });
                    GUILayout.Space(3);
                }

                GUILayout.EndVertical();
            }
        }

        private static double EaseOutQuint(float x) => 1 - Math.Pow(1 - x, 5);

        private Dictionary<MetaMediationDependencies.AndroidPackage, AndroidDependencyDto> FilterAndroidUpdatesForRemovedNetworks(Dictionary<MetaMediationDependencies.AndroidPackage, AndroidDependencyDto> androidDiff)
        {
            if (androidDiff == null) return null;
            
            var (_, removedNetworks) = GetNetworkChanges();
            if (removedNetworks.Count == 0) return androidDiff;
            
            var filteredDiff = new Dictionary<MetaMediationDependencies.AndroidPackage, AndroidDependencyDto>();
            
            foreach (var kvp in androidDiff)
            {
                var package = kvp.Key;
                var dependency = kvp.Value;
                
                bool belongsToRemovedNetwork = false;
                foreach (var removedNetwork in removedNetworks)
                {
                    if (DoesDependencyBelongToNetwork(package.Artifact, package.Group, removedNetwork.NetworkName))
                    {
                        belongsToRemovedNetwork = true;
                        break;
                    }
                }
                
                if (!belongsToRemovedNetwork)
                {
                    filteredDiff[package] = dependency;
                }
            }
            
            return filteredDiff;
        }
        
        private Dictionary<MetaMediationDependencies.IosPod, IOSDependencyDto> FilterIOSUpdatesForRemovedNetworks(Dictionary<MetaMediationDependencies.IosPod, IOSDependencyDto> iosDiff)
        {
            if (iosDiff == null) return null;
            
            var (_, removedNetworks) = GetNetworkChanges();
            if (removedNetworks.Count == 0) return iosDiff;
            
            var filteredDiff = new Dictionary<MetaMediationDependencies.IosPod, IOSDependencyDto>();
            
            foreach (var kvp in iosDiff)
            {
                var pod = kvp.Key;
                var dependency = kvp.Value;
                
                // Check if this dependency belongs to a removed network
                bool belongsToRemovedNetwork = false;
                foreach (var removedNetwork in removedNetworks)
                {
                    if (DoesDependencyBelongToNetwork(pod.Name, null, removedNetwork.NetworkName))
                    {
                        belongsToRemovedNetwork = true;
                        break;
                    }
                }
                
                
                if (!belongsToRemovedNetwork)
                {
                    filteredDiff[pod] = dependency;
                }
            }
            
            return filteredDiff;
        }
        
        private bool DoesDependencyBelongToNetwork(string dependencyName, string dependencyGroup, string networkName)
        {
            var lowerNetworkName = networkName.ToLower();
            var lowerDependencyName = dependencyName.ToLower();
            var lowerDependencyGroup = dependencyGroup?.ToLower();
            
            return lowerDependencyName.Contains(lowerNetworkName) || 
                   lowerNetworkName.Contains(lowerDependencyName) ||
                   (lowerDependencyGroup != null && (lowerDependencyGroup.Contains(lowerNetworkName) || lowerNetworkName.Contains(lowerDependencyGroup)));
        }

        private void UpdatePreviousNetworkSelection()
        {
            _previousAdNetworkSelection.Clear();
            foreach (var kvp in _adNetworkSelection)
            {
                _previousAdNetworkSelection[kvp.Key] = kvp.Value;
            }
        }
        
        private void UpdatePreviousMediatorSelection()
        {
            _previousMediatorSelection.Clear();
            foreach (var kvp in _mediatorSelection)
            {
                _previousMediatorSelection[kvp.Key] = kvp.Value;
            }
        }
        
        private void InitializePreviousNetworkSelection()
        {
            if (State.SelectableDependencies == null || Presenter == null) return;
            
            _previousAdNetworkSelection.Clear();
            foreach (var network in State.SelectableDependencies.Networks)
            {
                var isCurrentlySelected = Presenter.CurrentNetworks.Contains(network);
                _previousAdNetworkSelection[network] = isCurrentlySelected;
            }
        }
        
        private void InitializePreviousMediatorSelection()
        {
            if (State.SelectableDependencies == null || Presenter == null) return;
            
            _previousMediatorSelection.Clear();
            foreach (var mediator in State.SelectableDependencies.Mediations)
            {
                var isCurrentlySelected = Presenter.CurrentMediators.Contains(mediator);
                _previousMediatorSelection[mediator] = isCurrentlySelected;
            }
        }

        private (List<NetworkChangeInfo> addedNetworks, List<NetworkChangeInfo> removedNetworks) GetNetworkChanges()
        {
            var addedNetworks = new List<NetworkChangeInfo>();
            var removedNetworks = new List<NetworkChangeInfo>();

            foreach (var kvp in _adNetworkSelection)
            {
                var networkName = kvp.Key;
                var currentlySelected = kvp.Value;
                var previouslySelected = _previousAdNetworkSelection.ContainsKey(networkName) && _previousAdNetworkSelection[networkName];

                if (currentlySelected && !previouslySelected)
                {
                    addedNetworks.Add(GetNetworkChangeInfo(networkName));
                }
                else if (!currentlySelected && previouslySelected)
                {
                    removedNetworks.Add(GetNetworkChangeInfo(networkName));
                }
            }

            return (addedNetworks, removedNetworks);
        }
        
        private (List<MediatorChangeInfo> addedMediators, List<MediatorChangeInfo> removedMediators) GetMediatorChanges()
        {
            var addedMediators = new List<MediatorChangeInfo>();
            var removedMediators = new List<MediatorChangeInfo>();

            foreach (var kvp in _mediatorSelection)
            {
                var mediatorName = kvp.Key;
                var currentlySelected = kvp.Value;
                var previouslySelected = _previousMediatorSelection.ContainsKey(mediatorName) && _previousMediatorSelection[mediatorName];

                if (currentlySelected && !previouslySelected)
                {
                    addedMediators.Add(GetMediatorChangeInfo(mediatorName));
                }
                else if (!currentlySelected && previouslySelected)
                {
                    removedMediators.Add(GetMediatorChangeInfo(mediatorName));
                }
            }

            return (addedMediators, removedMediators);
        }

        private NetworkChangeInfo GetNetworkChangeInfo(string networkName)
        {
            var androidVersion = Presenter.GetAndroidVersionForNetwork(networkName);
            var iosVersion = Presenter.GetIOSVersionForNetwork(networkName);
            return new NetworkChangeInfo(networkName, androidVersion, iosVersion);
        }
        
        private MediatorChangeInfo GetMediatorChangeInfo(string mediatorName)
        {
            var androidVersion = Presenter.GetAndroidVersionForMediator(mediatorName);
            var iosVersion = Presenter.GetIOSVersionForMediator(mediatorName);
            return new MediatorChangeInfo(mediatorName, androidVersion, iosVersion);
        }


        private string GetVersionDisplayText(NetworkChangeInfo networkInfo)
        {
            var versions = new List<string>();
            
            if (!string.IsNullOrEmpty(networkInfo.AndroidVersion))
            {
                versions.Add($"Android: {networkInfo.AndroidVersion}");
            }
            
            if (!string.IsNullOrEmpty(networkInfo.IOSVersion))
            {
                versions.Add($"iOS: {networkInfo.IOSVersion}");
            }
            
            return versions.Count > 0 ? $" - {string.Join(", ", versions)}" : "";
        }
        
        private string GetMediatorVersionDisplayText(MediatorChangeInfo mediatorInfo)
        {
            var versions = new List<string>();
            
            if (!string.IsNullOrEmpty(mediatorInfo.AndroidVersion))
            {
                versions.Add($"Android: {mediatorInfo.AndroidVersion}");
            }
            
            if (!string.IsNullOrEmpty(mediatorInfo.IOSVersion))
            {
                versions.Add($"iOS: {mediatorInfo.IOSVersion}");
            }
            
            return versions.Count > 0 ? $" - {string.Join(", ", versions)}" : "";
        }

        private class NetworkChangeInfo
        {
            public string NetworkName { get; }
            public string AndroidVersion { get; }
            public string IOSVersion { get; }

            public NetworkChangeInfo(string networkName, string androidVersion, string iosVersion)
            {
                NetworkName = networkName;
                AndroidVersion = androidVersion;
                IOSVersion = iosVersion;
            }
        }
        
        private class MediatorChangeInfo
        {
            public string MediatorName { get; }
            public string AndroidVersion { get; }
            public string IOSVersion { get; }

            public MediatorChangeInfo(string mediatorName, string androidVersion, string iosVersion)
            {
                MediatorName = mediatorName;
                AndroidVersion = androidVersion;
                IOSVersion = iosVersion;
            }
        }

        private bool HasNetworkChanges()
        {
            var (addedNetworks, removedNetworks) = GetNetworkChanges();
            return addedNetworks.Count > 0 || removedNetworks.Count > 0;
        }
        
        private bool HasMediatorChanges()
        {
            var (addedMediators, removedMediators) = GetMediatorChanges();
            return addedMediators.Count > 0 || removedMediators.Count > 0;
        }
    }
}