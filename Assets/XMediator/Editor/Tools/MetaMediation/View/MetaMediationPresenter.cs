using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XMediator.Editor.Tools.DependencyManager.Actions;
using XMediator.Editor.Tools.DependencyManager.DI;
using XMediator.Editor.Tools.DependencyManager.Entities;
using XMediator.Editor.Tools.DependencyManager.Repository;
using XMediator.Editor.Tools.MetaMediation.Actions;
using XMediator.Editor.Tools.MetaMediation.Entities;
using XMediator.Editor.Tools.MetaMediation.Repository;
using XMediator.Editor.Tools.MetaMediation.Repository.Dto;
using XMediator.Editor.Tools.SkAdNetworkIds;

namespace XMediator.Editor.Tools.MetaMediation.View
{
    internal class MetaMediationPresenter
    {
        private const string AndroidPlatform = "android";
        private const string IosPlatform = "ios";
        private const string AndroidCorePackage = "com.x3mads.android.xmediator:core";
        private const string IosCorePod = "XMediator";
        private MetaMediationManagerView View { get; }
        private GetDependencies GetDependencies { get; }
        private SelectableDependencies SelectableDependencies { get; set; }

        private SettingsRepository SettingsRepository { get; set; }
        private GenerateXML GenerateXML { get; set; }

        internal HashSet<string> CurrentNetworks { get; set; }
        internal HashSet<string> CurrentMediators { get; set; }
        internal HashSet<string> CurrentTools { get; set; }

        internal HashSet<string> CurrentPlatforms { get; set; }
        internal MetaMediationTags CurrentTags { get; set; }
        internal Tuple<Dictionary<MetaMediationDependencies.AndroidPackage, AndroidDependencyDto>, Dictionary<MetaMediationDependencies.IosPod, IOSDependencyDto>> UpdateResults { get; private set; }
        private bool IsDownloading { get; set; }

        private GetAllDependencies GetAllDependencies { get; }
        private AvailableDependency SdkDependency { get; set; }

        private ResolveCoreAdapterVersion ResolveCoreAdapterVersion { get; }
        private UpdateCoreAdapter UpdateCoreAdapter { get; }
        private RetrieveNetworksUpdates RetrieveNetworksUpdates { get; }

        internal ResolveCoreAdapterVersion.Result CoreUpdateResult;


        internal MetaMediationPresenter(MetaMediationManagerView view)
        {
            View = view;
            SettingsRepository = new SettingsRepository();
            GetDependencies = new GetDependencies(new DependencyRepository(), SettingsRepository);
            GetAllDependencies = new GetAllDependencies(
                Instancies.apiDependenciesRepository,
                Instancies.installedDependenciesRepository,
                Instancies.settingRepository);
            ResolveCoreAdapterVersion = new ResolveCoreAdapterVersion(UtilsMetaMediation.GetCurrentCoreDependencies());
            UpdateCoreAdapter = new UpdateCoreAdapter(
                ResolveCoreAdapterVersion,
                new InstallAdapter(new DefaultAssetRepository()),
                new RetrieveDependencyAdapter(new DefaultDependencyManagerRepository())
            );
            RetrieveNetworksUpdates = new RetrieveNetworksUpdates(new MetaMediationDependenciesRepository());
        }

        internal void Initialize()
        {
            Debug.Log("Initialized");
            RetrieveFromSettings();
            RetrieveDependencies();
            GenerateXML = new GenerateXML(new DependenciesPreProcessor());
            RetrieveUnityPackage();
        }

        internal void GenerateAndSaveDependencies()
        {
            var iosManifest = GetIosManifestForMediators();
            var androidManifest = GetAndroidManifestForMediators();
            ResolvePlatformSemanticVersion(iosManifest, androidManifest, out var iosCoreSemanticVersion, out var androidCoreSemanticVersion);

            if (!CanWeProceedWithTheUpdate(iosCoreSemanticVersion, androidCoreSemanticVersion))
            {
                return;
            }

            UtilsMetaMediation.ApplyMetaMediation(false);
            var dependencies = new SelectedDependencies(
                networks: CurrentNetworks,
                mediations: CurrentMediators,
                tools: CurrentTools,
                iosManifest: iosManifest,
                androidManifest: androidManifest
            );

            GenerateXML.Invoke(selectableDependencies: dependencies, onSuccess: document =>
                {
                    Debug.Log(document.ToString());
                    SettingsRepository.ApplyMetaMediation();
                    MarkCmpToolAsShownToUser();
                    SettingsRepository.Save(networks: CurrentNetworks,
                        networkIds: dependencies.GetNetworkIds(),
                        mediations: CurrentMediators,
                        tools: CurrentTools,
                        platforms: CurrentPlatforms,
                        tags: CurrentTags);
                    SaveXMLDependencies.SaveXMLToFile(document, "MetaMediationDependencies.xml");
                    SkAdNetworkIdsServiceFactory.CreateInstance().SynchronizeSkAdNetworkIds();

                    UpdateCoreAdapter.Invoke(SdkDependency, iosCoreSemanticVersion,
                         androidCoreSemanticVersion,
                         onSuccess: () =>
                         {
                             CheckAndShowUpdates();
                             View.OnGeneratedDependencies(false);
                             ExcludeDependencyManager.ExtractAndSaveExcludeDependencies(androidManifest);
                         },
                         onError: errorMessage => { View.OnGeneratedDependencies(true, errorMessage); }
                     );
                },
                onError: error =>
                {
                    View.OnGeneratedDependencies(true, error.Message);
                    Debug.LogError(error);
                });
        }

        private void MarkCmpToolAsShownToUser()
        {
            if (SelectableDependencies.Tools.ContainsKey(SelectableDependencies.UMPName))
            {
                SettingsRepository.MarkCmpToolAsShownToUser();                
            }
        }

        private void RetrieveFromSettings()
        {
            SettingsRepository.Init();
            UpdateSelection(ListToDictionary(SettingsRepository.RetrieveMediations()),
                ListToDictionary(SettingsRepository.RetrieveNetworks()),
                ListToDictionary(SettingsRepository.RetrieveTools()),
                SettingsRepository.RetrievePlatforms().Contains(AndroidPlatform),
                SettingsRepository.RetrievePlatforms().Contains(IosPlatform));
            CurrentTags = SettingsRepository.RetrieveTags();
        }

        private static Dictionary<string, bool> ListToDictionary(IEnumerable<string> enableList)
        {
            return enableList.ToDictionary(key => key, key => true);
        }

        private void RetrieveDependencies()
        {
            IsDownloading = true;
            GetDependencies.Invoke(
                onSuccess: dependencies =>
                {
                    Debug.Log("GetDependencies onSuccess");
                    SetState(() =>
                    {
                        IsDownloading = false;
                        SelectableDependencies = dependencies;
                        CheckAndShowUpdates();
                        GetCorePackageStatusResults();
                    });
                },
                onError: exception =>
                {
                    IsDownloading = false;
                    Debug.LogError("GetDependencies error: " + exception);
                    View.OnDialogWithCloseWindow("Error", "There was an error downloading dependencies, try again later");
                }
            );
        }

        private void CheckAndShowUpdates()
        {
            var androidManifest = GetAndroidManifestForMediators();
            var iosManifest = GetIosManifestForMediators();
            var (androidDiff, iosDiff) = RetrieveNetworksUpdates.Invoke(iosManifest, androidManifest);
            if (androidDiff == null && iosDiff == null) return;
            
            View.OnRefreshUi();
            UpdateResults = new Tuple<Dictionary<MetaMediationDependencies.AndroidPackage, AndroidDependencyDto>, Dictionary<MetaMediationDependencies.IosPod, IOSDependencyDto>>(androidDiff, iosDiff);
        }

        private ManifestDto<AndroidDependencyDto> GetAndroidManifestForMediators()
        {
            return GetManifestForCurrentMediators(AndroidPlatform, SelectableDependencies.AndroidManifest, CurrentTags.Android);
        }
        
        private ManifestDto<IOSDependencyDto> GetIosManifestForMediators()
        {
            return GetManifestForCurrentMediators(IosPlatform, SelectableDependencies.IOSManifest, CurrentTags.IOS);
        }

        private ManifestDto<TDependency> GetManifestForCurrentMediators<TDependency>(string platformString, FlavoredManifestDto<TDependency> manifest, string tag)
        {
            return !CurrentPlatforms.Contains(platformString) ? null : manifest.GetManifestForMediators(CurrentMediators.ToList(), CurrentTools?.ToList(), tag);
        }

        private void RetrieveUnityPackage()
        {
            GetAllDependencies.Invoke(
                onSuccess: dependencies =>
                {
                    Debug.Log("RetrieveUnityCorePackage onSuccess");
                    SdkDependency = dependencies.Sdk;
                },
                onError: exception =>
                {
                    Debug.LogError("GetAllDependencies error: " + exception);
                    View.OnDialogWithCloseWindow(" Error", "There was an error downloading dependencies, try again later");
                });
        }

        private void SetState(Action action)
        {
            action();

            View.SetViewState(new MetaMediationViewState(SelectableDependencies, IsDownloading));
        }

        private void GetCorePackageStatusResults()
        {
            if (SelectableDependencies == null) return;
            var androidManifest = GetAndroidManifestForMediators();
            var iosManifest = GetIosManifestForMediators();
            if (androidManifest == null && iosManifest == null)
            {
                return;
            }

            ResolvePlatformSemanticVersion(iosManifest, androidManifest, out var iosCoreSemanticVersion, out var androidCoreSemanticVersion);
            CoreUpdateResult ??= ResolveCoreAdapterVersion.Invoke(SdkDependency, iosCoreSemanticVersion, androidCoreSemanticVersion);
        }

        private bool CanWeProceedWithTheUpdate(SemanticVersion iosCoreSemanticVersion,
            SemanticVersion androidCoreSemanticVersion)
        {
            try
            {
                var result = ResolveCoreAdapterVersion.Invoke(SdkDependency, iosCoreSemanticVersion, androidCoreSemanticVersion);
                return GetUpdateConfirmation(result);
            }
            catch (Exception exception)
            {
                View.OnGeneratedDependencies(true, exception.Message);
                return false;
            }
        }

        private bool GetUpdateConfirmation(ResolveCoreAdapterVersion.Result result)
        {
            if (result.Status != ResolveCoreAdapterVersion.Status.Downgrade)
            {
                return true;
            }

            return View.OnConfirmationDialog("Downgrading Core SDK", $"Are you sure you want to downgrade the SDK from version {result.PreviousVersion} to {result.Version.Version}?");
        }

        private static void ResolvePlatformSemanticVersion(ManifestDto<IOSDependencyDto> dependenciesIOSManifest,
            ManifestDto<AndroidDependencyDto> dependenciesAndroidManifest, out SemanticVersion iosCoreSemanticVersion,
            out SemanticVersion androidCoreSemanticVersion)
        {
            var iosDependencyDto = dependenciesIOSManifest?.core.FirstOrDefault(it => it.pod == IosCorePod);
            var androidDependencyDto = dependenciesAndroidManifest?.core.FirstOrDefault(it =>
                it.group_id + ":" + it.artifact_name == AndroidCorePackage);

            iosCoreSemanticVersion = null;
            if (iosDependencyDto != null)
                iosCoreSemanticVersion = new SemanticVersion(iosDependencyDto.suggested_version);
            androidCoreSemanticVersion = null;
            if (androidDependencyDto != null)
                androidCoreSemanticVersion = new SemanticVersion(androidDependencyDto.suggested_version);
        }

        public void CancelDownload()
        {
            SetState(() => { IsDownloading = false; });
        }

        public void UpdateSelection(Dictionary<string, bool> mediatorSelection,
            Dictionary<string, bool> adNetworkSelection, Dictionary<string, bool> toolSelection, bool isAndroid, bool isIos)
        {
            CurrentMediators = mediatorSelection.Where(pair => pair.Value).Select(keyValuePair => keyValuePair.Key)
                .ToHashSet();
            CurrentNetworks = adNetworkSelection.Where(pair => pair.Value).Select(keyValuePair => keyValuePair.Key)
                .ToHashSet();
            CurrentTools = toolSelection.Where(pair => pair.Value).Select(keyValuePair => keyValuePair.Key)
                .ToHashSet();
            CurrentPlatforms = new HashSet<string>();
            if (isAndroid)
            {
                CurrentPlatforms.Add(AndroidPlatform);
            }

            if (isIos)
            {
                CurrentPlatforms.Add(IosPlatform);
            }
        }
        
        internal void RefreshUpdatesWithNewSelection(Dictionary<string, bool> mediatorSelection,
            Dictionary<string, bool> adNetworkSelection, Dictionary<string, bool> toolSelection, bool isAndroid, bool isIos)
        {
            UpdateSelection(mediatorSelection, adNetworkSelection, toolSelection, isAndroid, isIos);
            CheckAndShowUpdates();
        }
        
        internal void SelectIOSTag(string tag)
        {
            CurrentTags = CurrentTags.UpdatingIOS(tag);
            CheckAndShowUpdates();
        }

        internal string GetAndroidVersionForAdapter(string dependencyName)
        {
            if (SelectableDependencies?.AndroidManifest == null) return null;
            
            var manifest = GetAndroidManifestForMediators();
            return VersionForNetwork(dependencyName, manifest, it => it.suggested_version) ??
                   VersionForTool(dependencyName, manifest, it => it.suggested_version);
        }
        
        internal string GetIOSVersionForAdapter(string dependencyName)
        {
            if (SelectableDependencies?.IOSManifest == null) return null;
            
            var manifest = GetIosManifestForMediators();
            return VersionForNetwork(dependencyName, manifest, it => it.suggested_version) ??
                VersionForTool(dependencyName, manifest, it => it.suggested_version);
        }

        private static string VersionForNetwork<T>(string networkName, ManifestDto<T> manifest, Func<T, string> getVersion)
        {
            if (manifest?.networks == null) return null;
            
            foreach (var networkDto in manifest.networks)
            {
                if (networkDto.display_name.Equals(networkName, StringComparison.OrdinalIgnoreCase))
                {
                    if (networkDto.dependencies != null && networkDto.dependencies.Count > 0)
                    {
                        return getVersion(networkDto.dependencies[0]);
                    }
                }
            }
            return null;
        }
        
        private static string VersionForTool<T>(string toolName, ManifestDto<T> manifest, Func<T, string> getVersion)
        {
            if (manifest?.additional_tools == null) return null;
            
            foreach (var toolDto in manifest.additional_tools)
            {
                if (toolDto.tool.Equals(toolName, StringComparison.OrdinalIgnoreCase))
                {
                    if (toolDto.dependencies != null && toolDto.dependencies.Count > 0)
                    {
                        return getVersion(toolDto.dependencies[0]);
                    }
                }
            }
            return null;
        }
        
        internal string GetAndroidVersionForMediator(string mediatorName)
        {
            if (SelectableDependencies?.AndroidManifest == null) return null;
            
            var manifest = GetAndroidManifestForMediators();
            if (manifest?.core == null) return null;
            
            // For mediators, we look at the core dependencies
            foreach (var coreDto in manifest.core)
            {
                if (coreDto.group_id != null && coreDto.artifact_name != null)
                {
                    var fullName = $"{coreDto.group_id}:{coreDto.artifact_name}";
                    if (fullName.Contains(mediatorName, StringComparison.OrdinalIgnoreCase) ||
                        mediatorName.Contains(coreDto.artifact_name, StringComparison.OrdinalIgnoreCase))
                    {
                        return coreDto.suggested_version;
                    }
                }
            }
            return null;
        }
        
        internal string GetIOSVersionForMediator(string mediatorName)
        {
            if (SelectableDependencies?.IOSManifest == null) return null;
            
            var manifest = GetIosManifestForMediators();
            if (manifest?.core == null) return null;
            
            // For mediators, we look at the core dependencies
            foreach (var coreDto in manifest.core)
            {
                if (coreDto.pod != null)
                {
                    if (coreDto.pod.Contains(mediatorName, StringComparison.OrdinalIgnoreCase) ||
                        mediatorName.Contains(coreDto.pod, StringComparison.OrdinalIgnoreCase))
                    {
                        return coreDto.suggested_version;
                    }
                }
            }
            return null;
        }
    }
}