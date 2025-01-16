using System;
using XMediator.Editor.Tools.DependencyManager.Entities;

namespace XMediator.Editor.Tools.DependencyManager.Repository
{
    internal interface DependencyManagerRepository
    {
        void LoadInstalledVersions(Action<InstalledVersions> onSuccess, Action<Exception> onError, Dependency installed = null);

        void DownloadFile(string downloadUrl, string fileName, Action<string> onSuccess, Action<Exception> onError);
    }
}