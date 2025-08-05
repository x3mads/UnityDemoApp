using XMediator.Editor.Tools.MetaMediation;

namespace XMediator.Editor.Tools.Settings
{
    public class XMediatorSettingsService
    {
        private readonly XMediatorSettingsRepository _settingsRepository;
        private readonly XMediatorDependenciesRepository _dependenciesRepository;
        private readonly MetaMediationService _metaMediationService;

        internal static class SettingsKeys
        {
            internal const string SkipSwiftConfiguration = "skipSwiftConfiguration";
            internal const string SkipSkAdNetworkIdsConfiguration = "skipSkAdNetworkIdsConfiguration";
            internal const string GoogleAdsIOSAppId = "googleAdsIOSAppId";
            internal const string DisableGoogleAdsAndroidAppId = "disableGoogleAdsAndroidAppId";
            internal const string GmaPropertiesTagFixEnabled = "gmaPropertiesTagFixEnabled";
            internal const string GoogleAdsAndroidAppId = "googleAdsAndroidAppId";
            internal const string DependencyManagerToken = "dependencyManagerToken";
            internal const string IsMetaMediation = "isMetamediation";
            internal const string Publisher = "publisher";
        }

        private static XMediatorSettingsService _instance;

        public static XMediatorSettingsService Instance => _instance ??= new XMediatorSettingsService();

        internal XMediatorSettingsService(
            XMediatorSettingsRepository settingsRepository = null,
            XMediatorDependenciesRepository dependenciesRepository = null,
            MetaMediationService metaMediationService = null
        )
        {
            _settingsRepository = settingsRepository ?? XMediatorSettingsXMLRepository.Instance;
            _dependenciesRepository = dependenciesRepository ?? XMediatorDependenciesXMLRepository.Instance;
            _metaMediationService = metaMediationService ?? new DefaultMetaMediationService();
        }

        public void ReloadSettings()
        {
            _settingsRepository.ReloadFromDisk();
        }

        internal void UpdateXMediatorDependenciesFile()
        {
            _dependenciesRepository.UpdateXMediatorDependenciesFile();
            if (IsMetaMediation)
            {
                _metaMediationService.AdjustXMediatorDependenciesSettings();
            }
        }

        public string DependencyManagerToken
        {
            get => _settingsRepository.GetSettingValue(SettingsKeys.DependencyManagerToken, "");
            set => _settingsRepository.SetSettingValue(SettingsKeys.DependencyManagerToken, value);
        }

        public string AndroidGoogleAdsAppId
        {
            get => _settingsRepository.GetSettingValue(SettingsKeys.GoogleAdsAndroidAppId, "");
            set => _settingsRepository.SetSettingValue(SettingsKeys.GoogleAdsAndroidAppId, value);
        }

        public string IOSGoogleAdsAppId
        {
            get => _settingsRepository.GetSettingValue(SettingsKeys.GoogleAdsIOSAppId, "");
            set => _settingsRepository.SetSettingValue(SettingsKeys.GoogleAdsIOSAppId, value);
        }

        public bool IsDisableGoogleAdsAndroidAppId
        {
            get => bool.Parse(_settingsRepository.GetSettingValue(SettingsKeys.DisableGoogleAdsAndroidAppId, "False"));
            set => _settingsRepository.SetSettingValue(SettingsKeys.DisableGoogleAdsAndroidAppId, value.ToString());
        }
        
        public bool IsGmaPropertiesTagFixEnabled
        {
            get => bool.Parse(_settingsRepository.GetSettingValue(SettingsKeys.GmaPropertiesTagFixEnabled, "False"));
            set => _settingsRepository.SetSettingValue(SettingsKeys.GmaPropertiesTagFixEnabled, value.ToString());
        }

        public bool IsSkipSwiftConfiguration
        {
            get => bool.Parse(_settingsRepository.GetSettingValue(SettingsKeys.SkipSwiftConfiguration, "False"));
            set => _settingsRepository.SetSettingValue(SettingsKeys.SkipSwiftConfiguration, value.ToString());
        }

        public bool IsSkipSkAdNetworkIdsConfiguration
        {
            get => bool.Parse(_settingsRepository.GetSettingValue(SettingsKeys.SkipSkAdNetworkIdsConfiguration, "False"));
            set => _settingsRepository.SetSettingValue(SettingsKeys.SkipSkAdNetworkIdsConfiguration, value.ToString());
        }

        public string Publisher
        {
            get => _settingsRepository.GetSettingValue(SettingsKeys.Publisher, "default");
            set => _settingsRepository.SetSettingValue(SettingsKeys.Publisher, value.ToString());
        }
        
        public bool IsMetaMediation
        {
            get =>  bool.Parse(_settingsRepository.GetSettingValue(SettingsKeys.IsMetaMediation, "False"));
            set => _settingsRepository.SetSettingValue(SettingsKeys.IsMetaMediation, value.ToString());
        }
        
        public string PackageVersion
        {
            get => _dependenciesRepository.GetVersionValue();
        }
    }
}