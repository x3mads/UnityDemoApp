using System.Collections.Generic;
using XMediator.Editor.Tools.DependencyManager.DI;
using XMediator.Editor.Tools.Settings;

namespace XMediator.Editor.Tools.MetaMediation.Repository
{
    internal class SettingsRepository
    {
        private static string MmMediations => "mm_mediations";
        private static string MmNetworkIds => "mm_network_ids";
        private static string MmNetworks => "mm_networks";
        private static string MmPlatforms => "mm_platforms";

        public static void Init()
        {
            Instancies.settingRepository.ReloadFromDisk();
        }

        public static void Save(IEnumerable<string> networks, IEnumerable<string> networkIds,
            IEnumerable<string> mediations, IEnumerable<string> platforms)
        {
            Instancies.settingRepository.SetSettingValue(MmMediations, ListToString(mediations));
            Instancies.settingRepository.SetSettingValue(MmNetworkIds, ListToString(networkIds));
            Instancies.settingRepository.SetSettingValue(MmNetworks, ListToString(networks));
            Instancies.settingRepository.SetSettingValue(MmPlatforms, ListToString(platforms));
        }
        
        public static void ApplyMetaMediation()
        {
            Instancies.settingRepository.SetSettingValue(XMediatorSettingsService.SettingsKeys.IsMetaMediation, "True");
        }

        public static IEnumerable<string> RetrieveMediations()
        {
            return StringToList(Instancies.settingRepository.GetSettingValue(MmMediations, ""));
        }

        public IEnumerable<string> RetrieveNetworks()
        {
            return StringToList(Instancies.settingRepository.GetSettingValue(MmNetworks, ""));
        }
        
        public IEnumerable<string> RetrieveNetworkIds()
        {
            return StringToList(Instancies.settingRepository.GetSettingValue(MmNetworkIds, ""));
        }
        
        public IEnumerable<string> RetrievePlatforms()
        {
            return StringToList(Instancies.settingRepository.GetSettingValue(MmPlatforms, ""));
        }

        private static string ListToString(IEnumerable<string> setList)
        {
            return string.Join(",", setList);
        }

        private static IEnumerable<string> StringToList(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return new HashSet<string>();
            }
            return new HashSet<string>(str.Split(','));
        }
    }
}