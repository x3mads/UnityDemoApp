using System.IO;
using System.Xml;
using UnityEngine;
using JetBrains.Annotations;
using UnityEngine;

namespace XMediator.Editor.Tools.Settings
{
    internal class XMediatorDependenciesXMLRepository : XMediatorDependenciesRepository
    {
        private static string XMediatorDependenciesPath => Path.Combine(
            Application.dataPath,
            "XMediatorSettings",
            "Editor",
            "Main",
            "XMediatorDependencies.xml"
        );
        
        internal static class DependenciesKeys
        {
            internal const string RootElement = "dependencies";
            internal const string DetailsElement = "details";
            internal const string Version = "version";
        }
        
        private static XMediatorDependenciesXMLRepository _instance;
        private XmlDocument DependenciesDocument { get; set; }

        internal static XMediatorDependenciesXMLRepository Instance =>
            _instance ??= new XMediatorDependenciesXMLRepository();

        public void UpdateXMediatorDependenciesFile()
        {
            var path = XMediatorDependenciesPath;
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path) ?? throw new System.InvalidOperationException($"Directory for path {path} not found."));
            }

            var template = Resources.Load<TextAsset>("com.x3mads.xmediator/XMediatorDependencies.xml-TEMPLATE");
            File.WriteAllText(path, template.text);
            Resources.UnloadAsset(template);
        }
        
        public string GetVersionValue()
        {
            var element = GetElement(DependenciesKeys.DetailsElement);
            var value = GetValue(DependenciesKeys.Version, string.Empty, element);
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value;
        }
        
        private string GetValue(string name, string defaultValue, XmlElement element)
        {
            var value = element?[name]?.InnerText;
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        }
        
        private XmlElement GetElement(string name) {
            return GetRootElement()?.GetElementsByTagName(name)?.Item(0) as XmlElement;
        }
        
        [CanBeNull]
        private XmlElement GetRootElement()
        {
            ReloadFromDisk();
            var rootElement = DependenciesDocument[DependenciesKeys.RootElement];

            if (rootElement == null)
            {
                Debug.LogError("[Dependencies] Failed to get XMediatorDependencies.xml root element.");
            }

            return rootElement;
        }
        
        private void ReloadFromDisk()
        {
            DependenciesDocument = new XmlDocument();
            
            DependenciesDocument.LoadXml(ReadAllText(XMediatorDependenciesPath));
        }
        
        private string ReadAllText(string file)
        {
            if (File.Exists(file))
            {
                return File.ReadAllText(file);
            }
            else
            {
                return string.Empty;
            }
        }
    }
}