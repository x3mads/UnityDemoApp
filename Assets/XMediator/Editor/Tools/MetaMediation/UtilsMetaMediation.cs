using UnityEngine;
using XMediator.Editor.Tools.Settings;
using static XMediator.Editor.Tools.MetaMediation.DependencyVersionResolver;

namespace XMediator.Editor.Tools.MetaMediation
{
    internal static class UtilsMetaMediation
    {
        private const string XMLCoreFilePath = "Assets/XMediatorSettings/Editor/Main/XMediatorDependencies.xml";
        
        internal static void ApplyMetaMediation(bool undo)
        {
            AdjustXMediatorDependenciesSettings(undo);

            if (undo) return;
            CleanDependencies();
        }

        internal static void AdjustXMediatorDependenciesSettings(bool undo)
        {
            var replacer = new AndroidPackageReplacer(XMLCoreFilePath);

            Debug.Log(replacer.ReplaceAndroidPackageSpecAndRepository(undo)
                ? "Android package spec updated."
                : "No changes were made or the file was not found.");
        }

        internal static void CleanDependencies(string excludePrefix = null)
        {
            var cleaner = new XMediatorDependenciesCleaner();
            cleaner.RemoveDependenciesFiles(excludePrefix);
        }

        internal static XmlDependencyVersions GetCurrentCoreDependencies()
        {
            XMediatorSettingsService.Instance.ReloadSettings();
            XMediatorSettingsService.Instance.UpdateXMediatorDependenciesFile();
            
            return RetrieveXmlVersions(XMLCoreFilePath);
        }
    }

    internal interface MetaMediationService
    {
        void AdjustXMediatorDependenciesSettings();
    }

    class DefaultMetaMediationService: MetaMediationService
    {
        public void AdjustXMediatorDependenciesSettings()
        {
            UtilsMetaMediation.AdjustXMediatorDependenciesSettings(false);
        }
    }
}