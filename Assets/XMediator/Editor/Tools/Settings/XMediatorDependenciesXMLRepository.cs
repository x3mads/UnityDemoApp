using System.IO;
using System.Text.RegularExpressions;
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

        private static XMediatorDependenciesXMLRepository _instance;

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
    }
}