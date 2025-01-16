using System;
using System.Collections.Generic;
using System.Linq;
using XMediator.Editor.Tools.DependencyManager.Repository;
using XMediator.Editor.Tools.MetaMediation.Repository;
using XMediator.Editor.Tools.Settings;

namespace XMediator.Editor.Tools.SkAdNetworkIds
{
    internal static class SkAdNetworkIdsServiceFactory
    {
        public static SkAdNetworkIdsService CreateInstance()
        {
            return new SkAdNetworkIdsService(
                new DefaultNetworkIdsProvider(),
                new RemoteSkAdNetworkIdsRepository(),
                new JsonFileSkAdNetworkIdsStorage());
        }
    }

    class DefaultNetworkIdsProvider : NetworkIdsProvider
    {
        public void GetIds(Action<IEnumerable<string>> onSuccess, Action<Exception> onError)
        {
            var settingsService = XMediatorSettingsService.Instance;
            settingsService.ReloadSettings();

            if (settingsService.IsMetaMediation)
            {
                var settingsRepository = new SettingsRepository();
                onSuccess(settingsRepository.RetrieveNetworkIds());
            }
            else
            {
                var repository = new DefaultDependencyManagerRepository();
                repository.LoadInstalledVersions(versions =>
                        onSuccess(versions.Adapters.Select(version => version.Name)),
                    onError: onError);
            }
        }
    }
}