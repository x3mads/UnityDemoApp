using System;
using System.IO;
using UnityEngine;
using XMediator.Editor.Tools.DependencyManager.Entities;
using XMediator.Editor.Tools.DependencyManager.Repository;
using XMediator.Editor.Tools.MetaMediation.Entities;
using XMediator.Editor.Tools.Settings;

namespace XMediator.Editor.Tools.MetaMediation.Actions
{
    internal class InstallAdapter : IInstallAdapter
    {
        private AssetRepository AssetRepository { get; }

        public InstallAdapter(AssetRepository assetRepository)
        {
            AssetRepository = assetRepository;
        }

        public void Invoke(string path, Action onSuccess, Action<string> onError)
        {
            Directory.Delete(Path.Combine(Application.dataPath, "XMediator"), true);
            AssetRepository.ImportPackage(path,
                (isSuccess, reason) =>
                {
                    if (isSuccess)
                    {
                        XMediatorSettingsService.Instance.UpdateXMediatorDependenciesFile();
                        onSuccess();
                    }
                    else
                    {
                        Debug.Log("Failed to import package: " + reason);
                        onError("Failed to import package: " + reason);
                    }
                }
            );
        }
    }
}