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
        private static string MmTagIOS => "mm_tag_ios";
        private static string MmTagAndroid => "mm_tag_android";
        private static string MmAndroidVersionsUrl => "mm_android_versions_url";
        private static string MmIOSVersionsUrl => "mm_ios_versions_url";

        internal static void Init()
        {
            Instancies.settingRepository.ReloadFromDisk();
        }

        internal static void Save(IEnumerable<string> networks, IEnumerable<string> networkIds,
            IEnumerable<string> mediations, IEnumerable<string> platforms, MetaMediationTags tags)
        {
            Instancies.settingRepository.SetSettingValue(MmMediations, ListToString(mediations));
            Instancies.settingRepository.SetSettingValue(MmNetworkIds, ListToString(networkIds));
            Instancies.settingRepository.SetSettingValue(MmNetworks, ListToString(networks));
            Instancies.settingRepository.SetSettingValue(MmPlatforms, ListToString(platforms));
            Instancies.settingRepository.SetSettingValue(MmTagIOS, tags.IOS);
            Instancies.settingRepository.SetSettingValue(MmTagAndroid, tags.Android);
        }
        
        internal static void ApplyMetaMediation()
        {
            Instancies.settingRepository.SetSettingValue(XMediatorSettingsService.SettingsKeys.IsMetaMediation, "True");
        }

        internal static IEnumerable<string> RetrieveMediations()
        {
            return StringToList(Instancies.settingRepository.GetSettingValue(MmMediations, ""));
        }

        internal IEnumerable<string> RetrieveNetworks()
        {
            return StringToList(Instancies.settingRepository.GetSettingValue(MmNetworks, ""));
        }
        
        internal IEnumerable<string> RetrieveNetworkIds()
        {
            return StringToList(Instancies.settingRepository.GetSettingValue(MmNetworkIds, ""));
        }
        
        internal IEnumerable<string> RetrievePlatforms()
        {
            return StringToList(Instancies.settingRepository.GetSettingValue(MmPlatforms, ""));
        }
        
        internal MetaMediationTags RetrieveTags()
        {
            return new MetaMediationTags(
                Instancies.settingRepository.GetSettingValue(MmTagIOS, "").Trim(),
                Instancies.settingRepository.GetSettingValue(MmTagAndroid, "").Trim()
                );
        }
        
        internal string RetrieveAndroidVersionsUrl()
        {
            return Instancies.settingRepository.GetSettingValue(MmAndroidVersionsUrl, "");
        }
        
        internal string RetrieveIOSVersionsUrl()
        {
            return Instancies.settingRepository.GetSettingValue(MmIOSVersionsUrl, "");
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