using System;
using XMediator.Editor.Tools.DependencyManager.DI;
using XMediator.Editor.Tools.DependencyManager.Entities;
using XMediator.Editor.Tools.DependencyManager.Repository;
using XMediator.Editor.Tools.MetaMediation.Entities;

namespace XMediator.Editor.Tools.MetaMediation.Actions
{
    internal class RetrieveDependencyAdapter : IRetrieveDependencyAdapter
    {
        private const string StagingAccount = "x3m_developer";
        private DependencyManagerRepository DependencyManagerRepository { get; }

        public RetrieveDependencyAdapter(DependencyManagerRepository dependencyManagerRepository)
        {
            DependencyManagerRepository = dependencyManagerRepository;
        }

        public void Invoke(AvailableVersion dependency, Action<string> onSuccess, Action<Exception> onError)
        {
            DependencyManagerRepository.DownloadFile(
                downloadUrl: DependencyDownloadUrl(dependency),
                fileName: dependency.DownloadFilename,
                onSuccess: onSuccess,
                onError: onError
            );
        }

        private string DependencyDownloadUrl(AvailableVersion dependency)
        {
            return Instancies.settingRepository.GetSettingValue("publisher", "default") == StagingAccount
                ? dependency.DownloadUrl?.Replace("/unity/", "/unity-snapshot/")
                : dependency.DownloadUrl;
        }
    }
}