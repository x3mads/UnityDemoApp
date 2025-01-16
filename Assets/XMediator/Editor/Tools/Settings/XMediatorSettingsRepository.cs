namespace XMediator.Editor.Tools.Settings
{
    internal interface XMediatorSettingsRepository
    {
        void ReloadFromDisk();

        string GetSettingValue(string name, string defaultValue);

        void SetSettingValue(string name, string value);
    }
}