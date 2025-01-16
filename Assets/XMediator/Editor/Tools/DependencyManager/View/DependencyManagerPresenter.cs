using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using UnityEngine;
using XMediator.Editor.Tools.DependencyManager.Actions;
using XMediator.Editor.Tools.DependencyManager.DI;
using XMediator.Editor.Tools.DependencyManager.Entities;
using XMediator.Editor.Tools.DependencyManager.Repository;
using XMediator.Editor.Tools.MetaMediation;
using XMediator.Editor.Tools.Settings;
using XMediator.Editor.Tools.SkAdNetworkIds;
using Action = System.Action;

namespace XMediator.Editor.Tools.DependencyManager.View
{
    internal class DependencyManagerPresenter
    {
        private DependencyManagerView View { get; }
        private DependencyManagerRepository DependencyManagerRepository { get; }
        private AssetRepository AssetRepository { get; }
        private GetAllDependencies GetAllDependencies { get; }
        private GetInstalledDependencies GetInstalledDependencies { get; }
        private RemoveDependency RemoveDependency { get; }
        private UpdateAndroidDependenciesToPublic UpdateAndroidDependenciesToPublic { get; }

        // State
        private bool IsDownloading { get; set; }
        private InstalledVersions InstalledVersions { get; set; }
        private string InstalledVersionsStatusMessage { get; set; }
        private string AvailableVersionsStatusMessage { get; set; }
        private bool ShowRetryLoadInstalledVersions { get; set; }
        private bool ShowRetryFetchAvailableVersions { get; set; }
        private SkAdNetworkIdsService SkAdNetworkIdsService { get; }
        private bool ShouldSynchronizeSkAdNetworkIds = true;
        private AvailableDependencies AvailableDependencies { get; set; }


        private readonly XMediatorSettingsRepository _settingsRepository;
        private const string StagingAccount = "x3m_developer";

        internal DependencyManagerPresenter(
            DependencyManagerView view,
            DependencyManagerRepository dependencyManagerRepository = null,
            AssetRepository assetRepository = null,
            SkAdNetworkIdsService skAdNetworkIdsService = null,
            XMediatorSettingsRepository settingsRepository = null,
            UpdateAndroidDependenciesToPublic updateAndroidDependenciesToPublic = null
        )
        {
            View = view;
            DependencyManagerRepository = dependencyManagerRepository ?? new DefaultDependencyManagerRepository();
            AssetRepository = assetRepository ?? new DefaultAssetRepository();
            SkAdNetworkIdsService = skAdNetworkIdsService ?? SkAdNetworkIdsServiceFactory.CreateInstance();
            AvailableDependencies = AvailableDependencies.Blank;
            InstalledVersions = InstalledVersions.Blank;
            GetAllDependencies = new GetAllDependencies(
                Instancies.apiDependenciesRepository,
                Instancies.installedDependenciesRepository,
                Instancies.settingRepository);
            GetInstalledDependencies = new GetInstalledDependencies(
                Instancies.installedDependenciesRepository
            );
            RemoveDependency = new RemoveDependency(Instancies.installedDependenciesRepository);
            _settingsRepository = settingsRepository ?? Instancies.settingRepository;
            UpdateAndroidDependenciesToPublic = updateAndroidDependenciesToPublic ?? new UpdateAndroidDependenciesToPublic();
        }

        internal void Initialize()
        {
            XMediatorSettingsService.Instance.ReloadSettings();
            XMediatorSettingsService.Instance.UpdateXMediatorDependenciesFile();
            UpdateAndroidDependenciesToPublic.Execute();
            GetDependencies();
        }

        private void GetDependencies()
        {
            SetState(() =>
            {
                AvailableVersionsStatusMessage = "Loading...";
                ShowRetryFetchAvailableVersions = false;
                InstalledVersionsStatusMessage = null;
                ShowRetryLoadInstalledVersions = false;
            });
            var installedDependencies = GetInstalledDependencies.Invoke();
            if (installedDependencies != null)
            {
                SetState(() =>
                {
                    InstalledVersions = installedDependencies;
                    InstalledVersionsStatusMessage = null;
                });
            }
            else
            {
                SetState(() =>
                {
                    InstalledVersions = InstalledVersions.Blank;
                    InstalledVersionsStatusMessage = "Failed to load installed versions.";
                    ShowRetryLoadInstalledVersions = true;
                });
            }

            GetAllDependencies.Invoke(
                onSuccess: dependencies =>
                {
                    SetState(() =>
                    {
                        AvailableDependencies = dependencies;
                        AvailableVersionsStatusMessage = null;
                    });
                },
                onError: exception =>
                {
                    SetState(() =>
                    {
                        AvailableDependencies = AvailableDependencies.Blank;
                        AvailableVersionsStatusMessage = "Failed to fetch available versions.";
                        ShowRetryFetchAvailableVersions = true;
                    });
                });
        }

        internal void InstallDependency(Dependency dependency)
        {
            ShouldSynchronizeSkAdNetworkIds = true; // A new dependency should always try to sync skan ids
            _performInstall(dependency);
        }

        internal bool IsMetaMediation()
        {
            return _settingsRepository.GetSettingValue(XMediatorSettingsService.SettingsKeys.IsMetaMediation,
                "False") == "True";
        }

        internal void UpdateDependency(Dependency dependency)
        {
            if (!dependency.IsInstalled)
                ShouldSynchronizeSkAdNetworkIds = true; // A new dependency should always try to sync skan ids
            _performInstall(dependency);
        }

        public void CancelDownload()
        {
            SetState(() => { IsDownloading = false; });
        }

        public void RetryLoadInstalledVersions()
        {
            _refreshInstalledVersions();
        }

        public void RetryFetchAvailableVersions()
        {
            GetDependencies();
        }

        public void Remove(Dependency dependency)
        {
            var result = View.DisplayDialogOptions(
                dialogTitle: $"Removing {dependency.Name} dependency",
                message: $"Are you sure you want to remove {dependency.Name} dependency?", "Yes", "Cancel");
            if (result != 0) return;
            var suffix = "";
            if (!dependency.Name.Equals("Metamediation"))
                suffix = "XMediator";
            var xmlFile = $"{suffix}{dependency.Name}Dependencies.xml";
            RemoveDependency.Invoke(xmlFile, isSuccess =>
            {
                if (isSuccess)
                {
                    _refreshInstalledVersions();
                }
                else
                {
                    View.DisplayDialog(
                        title: "Remove failed",
                        message: $"Could not remove {dependency.Name}");
                }
            });
        }

        public void ExportInstalledVersions()
        {
            DataContractJsonSerializer jsonSer = new DataContractJsonSerializer(typeof(InstalledVersions));
            MemoryStream stream = new MemoryStream();
            jsonSer.WriteObject(stream, InstalledVersions);
            stream.Position = 0;
            StreamReader sr = new StreamReader(stream);
            var jsonString = sr.ReadToEnd();
            var fileName = _settingsRepository.GetSettingValue("publisher", "default") + "_installed_versions";
            View.SaveJsonFile("Save exported XMediator installed versions", fileName, jsonString);
        }

        internal void UpdateSdkLastVersion(Dependency dependency, AvailableVersion updateVersion)
        {
            SetState(() =>
            {
                dependency.UpdateLatestVersion(updateVersion);
                var availableDependency = new AvailableDependency(dependency);
                AvailableDependencies =
                    new AvailableDependencies(sdk: availableDependency, adapters: AvailableDependencies.Adapters);
            });
        }

        internal void UpdateDependencyLastVersion(Dependency dependency, AvailableVersion updateVersion)
        {
            SetState(() =>
            {
                var list = AvailableDependencies.Adapters.ToList();
                list.Remove(list.Find(x => x.Name.Equals(dependency.Name)));
                dependency.UpdateLatestVersion(updateVersion);
                var availableDependency = new AvailableDependency(dependency);
                list.Add(availableDependency);
                AvailableDependencies = new AvailableDependencies(sdk: AvailableDependencies.Sdk, adapters: list);
            });
        }

        internal void ApplyTurnKey(string dependencyName)
        {
            Debug.Log("Cleaning dependencies for " + dependencyName);
            UtilsMetaMediation.CleanDependencies(dependencyName);
        }

        internal bool IsATurnKeyInstalled()
        {
            return InstalledVersions.Adapters.Any(dependency => dependency.Name.StartsWith("TurnKey"));
        }

        private void _refreshInstalledVersions(Dependency installed = null)
        {
            SetState(() =>
            {
                InstalledVersionsStatusMessage = null;
                ShowRetryLoadInstalledVersions = false;
            });
            DependencyManagerRepository.LoadInstalledVersions(
                installed: installed,
                onSuccess: versions =>
                {
                    SetState(() =>
                    {
                        InstalledVersions = versions;
                        InstalledVersionsStatusMessage =
                            !versions.Adapters.Any() ? "No installed versions detected." : null;
                    });
                    UpdateAndroidDependenciesToPublic.Execute();
                },
                onError: _ =>
                {
                    SetState(() =>
                    {
                        InstalledVersions = InstalledVersions.Blank;
                        InstalledVersionsStatusMessage = "Failed to load installed versions.";
                        ShowRetryLoadInstalledVersions = true;
                    });
                }
            );
        }

        private void _performInstall(Dependency dependency)
        {
            SetState(() => { IsDownloading = true; });
            XMediatorSettingsService.Instance.ReloadSettings();
            DependencyManagerRepository.DownloadFile(
                downloadUrl: DependencyLatestVersionDownloadUrl(dependency) ??
                             Error("dependency.LatestVersionDownloadUrl != null"),
                fileName: dependency.LatestVersionDownloadFilename ??
                          Error("dependency.LatestVersionDownloadFilename != null"),
                onSuccess: path =>
                {
                    if (dependency.Name == "XMediator")
                    {
                        Directory.Delete(Path.Combine(Application.dataPath, "XMediator"), true);
                    }

                    AssetRepository.ImportPackage(path, (isSuccess, reason) =>
                    {
                        SetState(() => IsDownloading = false);
                        if (isSuccess)
                        {
                            if (dependency.Name == "XMediator")
                            {
                                XMediatorSettingsService.Instance.UpdateXMediatorDependenciesFile();
                            }

                            if (ShouldSynchronizeSkAdNetworkIds)
                            {
                                SkAdNetworkIdsService.SynchronizeSkAdNetworkIds(list =>
                                    ShouldSynchronizeSkAdNetworkIds = false);
                            }

                            View.DisplayDialog(
                                title: "Package installed",
                                message: $"{dependency.Name} {dependency.LatestVersion} installed successfully.");
                        }
                        else
                        {
                            View.DisplayDialog(
                                title: "Install failed",
                                message:
                                $"Could not install {dependency.LatestVersionDownloadFilename}. Reason: {reason}");
                        }

                        _refreshInstalledVersions(installed: dependency);
                    });
                },
                onError: exception =>
                {
                    SetState(() => { IsDownloading = false; });
                    View.DisplayDialog(
                        title: "Download failed",
                        message: $"Could not download {dependency.LatestVersionDownloadFilename}. " +
                                 $"Exception: {exception.Message}");
                }
            );
        }

        private string DependencyLatestVersionDownloadUrl(Dependency dependency)
        {
            return _settingsRepository.GetSettingValue("publisher", "default") == StagingAccount
                ? dependency.LatestVersionDownloadUrl?.Replace("/unity/", "/unity-snapshot/")
                : dependency.LatestVersionDownloadUrl;
        }

        private void SetState(Action action)
        {
            action();
            View.SetViewState(DependencyManagerViewState.Build(
                installedVersions: InstalledVersions,
                availableDependencies: AvailableDependencies,
                isDownloading: IsDownloading,
                installedVersionsStatusMessage: InstalledVersionsStatusMessage,
                availableVersionsStatusMessage: AvailableVersionsStatusMessage,
                showRetryLoadInstalledVersions: ShowRetryLoadInstalledVersions,
                showRetryFetchAvailableVersions: ShowRetryFetchAvailableVersions
            ));
        }

        private static string Error(string assertion)
        {
            throw new Exception(assertion + " assertion failed.");
        }
    }
}