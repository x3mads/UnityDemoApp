using XMediator.Editor.Tools.DependencyManager.Repository;
using XMediator.Editor.Tools.Settings;

namespace XMediator.Editor.Tools.DependencyManager.DI
{
    internal static class Instancies
    {
        internal static XMediatorSettingsRepository settingRepository = XMediatorSettingsXMLRepository.Instance;

        internal static InstalledDependenciesRepository installedDependenciesRepository = XmlInstalledDependenciesRepository.Instance;

        internal static DependenciesRepository apiDependenciesRepository = ApiDependenciesRepository.Instance;
    }
}