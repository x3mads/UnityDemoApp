using System.IO;
using System.Xml;
using JetBrains.Annotations;
using UnityEngine;

namespace XMediator.Editor.Tools.Settings
{
    internal class XMediatorSettingsXMLRepository : XMediatorSettingsRepository
    {
        private static string XMediatorSettingsPath => Path.Combine(
            Application.dataPath,
            "XMediatorSettings",
            "Editor",
            "XMediatorSettings.xml"
        );

        private const string RootElement = "buildSettings";

        private XmlDocument SettingsDocument { get; set; }

        private static XMediatorSettingsXMLRepository _instance;

        internal static XMediatorSettingsXMLRepository Instance =>
            _instance ??= new XMediatorSettingsXMLRepository();

        public void ReloadFromDisk()
        {
            SettingsDocument = new XmlDocument();
            
            SettingsDocument.LoadXml(ReadAllTextOrCreate(XMediatorSettingsPath));
        }

        public string GetSettingValue(string name, string defaultValue)
        {
            var value = GetRootElement()?[name]?.InnerText;
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        }

        public void SetSettingValue(string name, string value)
        {
            var rootElement = GetRootElement();

            if (rootElement == null) return;

            var settingElement = rootElement[name];

            if (settingElement == null)
            {
                settingElement = SettingsDocument.CreateElement(name);
                rootElement.AppendChild(settingElement);
            }

            settingElement.InnerText = value.Trim();
            SettingsDocument.Save(XMediatorSettingsPath);
        }

        [CanBeNull]
        private XmlElement GetRootElement()
        {
            var rootElement = SettingsDocument[RootElement];

            if (rootElement == null)
            {
                Debug.LogError("[Settings] Failed to get XMediatorSettings.xml root element.");
            }

            return rootElement;
        }

        private static string ReadAllTextOrCreate(string file)
        {
            if (File.Exists(file)) return File.ReadAllText(file);
            Directory.CreateDirectory(Path.GetDirectoryName(file) ?? throw new System.InvalidOperationException($"Directory for file {file} not found."));
            var settingsTemplateContent = Resources.Load<TextAsset>("com.x3mads.xmediator/XMediatorSettings-TEMPLATE");
            File.WriteAllText(file, settingsTemplateContent.text);
            return File.ReadAllText(file);
        }
    }
}