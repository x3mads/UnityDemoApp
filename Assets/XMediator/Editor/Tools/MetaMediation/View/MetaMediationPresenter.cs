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
        private Dependencies Dependencies { get; set; }

        private SettingsRepository SettingsRepository { get; set; }
        private GenerateXML GenerateXML { get; set; }

        internal HashSet<string> CurrentNetworks { get; set; }
        internal HashSet<string> CurrentMediators { get; set; }

        internal HashSet<string> CurrentPlatforms { get; set; }
        internal Tuple<Dictionary<MetaMediationDependencies.AndroidPackage, DependencyDto>, Dictionary<MetaMediationDependencies.IosPod, IOSDependencyDto>> UpdateResults { get; private set; }
        private bool IsDownloading { get; set; }

        private GetAllDependencies GetAllDependencies { get; }
        private AvailableDependency SdkDependency { get; set; }

        private ResolveCoreAdapterVersion ResolveCoreAdapterVersion { get; }
        private UpdateCoreAdapter UpdateCoreAdapter { get; }
        private RetrieveNetworksUpdates RetrieveNetworksUpdates { get; set; }

        internal ResolveCoreAdapterVersion.Result CoreUpdateResult = null;


        internal MetaMediationPresenter(MetaMediationManagerView view)
        {
            View = view;
            GetDependencies = new GetDependencies(new DependencyRepository());
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
            RetrieveDependencies();
            RetrieveFromSettings();
            GenerateXML = new GenerateXML(new DependenciesPreProcessor());
            RetrieveUnityPackage();
        }

        internal void GenerateAndSaveDependencies()
        {
            var dependenciesIOSManifest = CurrentPlatforms.Contains(IosPlatform) ? Dependencies.IOSManifest : null;
            var dependenciesAndroidManifest = CurrentPlatforms.Contains(AndroidPlatform) ? Dependencies.AndroidManifest : null;
            ResolvePlatformSemanticVersion(dependenciesIOSManifest, dependenciesAndroidManifest, out var iosCoreSemanticVersion, out var androidCoreSemanticVersion);

            if (!CanWeProceedWithTheUpdate(iosCoreSemanticVersion, androidCoreSemanticVersion))
            {
                return;
            }

            UtilsMetaMediation.ApplyMetaMediation(false);
            var dependencies = new Dependencies(
                networks: CurrentNetworks,
                mediations: CurrentMediators,
                iosManifest: dependenciesIOSManifest,
                androidManifest: dependenciesAndroidManifest
            );

            GenerateXML.Invoke(dependencies: dependencies, onSuccess: document =>
                {
                    Debug.Log(document.ToString());
                    SettingsRepository.ApplyMetaMediation();
                    SettingsRepository.Save(networks: CurrentNetworks,
                        networkIds: dependencies.GetNetworkIds(),
                        mediations: CurrentMediators,
                        platforms: CurrentPlatforms);
                    SaveXMLDependencies.SaveXMLToFile(document, "MetaMediationDependencies.xml");
                    SkAdNetworkIdsServiceFactory.CreateInstance().SynchronizeSkAdNetworkIds();

                    UpdateCoreAdapter.Invoke(SdkDependency, iosCoreSemanticVersion,
                         androidCoreSemanticVersion,
                         onSuccess: () =>
                         {
                             CheckAndShowUpdates();
                             View.OnGeneratedDependencies(false);
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

        private void RetrieveFromSettings()
        {
            SettingsRepository = new SettingsRepository();
            SettingsRepository.Init();
            UpdateSelection(ListToDictionary(SettingsRepository.RetrieveMediations()),
                ListToDictionary(SettingsRepository.RetrieveNetworks()),
                SettingsRepository.RetrievePlatforms().Contains(AndroidPlatform),
                SettingsRepository.RetrievePlatforms().Contains(IosPlatform));
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
                        Dependencies = dependencies;
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
            var (androidDiff, iosDiff) = RetrieveNetworksUpdates.Invoke(Dependencies);

            if (androidDiff == null && iosDiff == null) return;
            View.OnRefreshUi();
            UpdateResults = new Tuple<Dictionary<MetaMediationDependencies.AndroidPackage, DependencyDto>, Dictionary<MetaMediationDependencies.IosPod, IOSDependencyDto>>(androidDiff, iosDiff);
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

            View.SetViewState(new MetaMediationViewState(Dependencies, IsDownloading));
        }

        private void GetCorePackageStatusResults()
        {
            if (Dependencies == null) return;
            var dependenciesIOSManifest = CurrentPlatforms.Contains(IosPlatform) ? Dependencies.IOSManifest : null;
            var dependenciesAndroidManifest = CurrentPlatforms.Contains(AndroidPlatform) ? Dependencies.AndroidManifest : null;
            if (dependenciesAndroidManifest == null && dependenciesIOSManifest == null)
            {
                return;
            }

            ResolvePlatformSemanticVersion(dependenciesIOSManifest, dependenciesAndroidManifest, out var iosCoreSemanticVersion, out var androidCoreSemanticVersion);
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

        private static void ResolvePlatformSemanticVersion(IOSManifestDto dependenciesIOSManifest,
            AndroidManifestDto dependenciesAndroidManifest, out SemanticVersion iosCoreSemanticVersion,
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
            Dictionary<string, bool> adNetworkSelection, bool isAndroid, bool isIos)
        {
            CurrentMediators = mediatorSelection.Where(pair => pair.Value).Select(keyValuePair => keyValuePair.Key)
                .ToHashSet();
            CurrentNetworks = adNetworkSelection.Where(pair => pair.Value).Select(keyValuePair => keyValuePair.Key)
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
    }
}