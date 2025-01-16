using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEngine;

namespace XMediator.Editor.Tools.MetaMediation
{
    internal class AndroidPackageReplacer
    {
        private readonly string _filePath;
        private const string OldSpecPrefix = "com.etermax.android";
        private const string NewSpecPrefix = "com.x3mads.android";
        private const string NewRepositoryUrl = "https://android-artifact-registry.x3mads.com/maven";
        private const string RepositoryUrlPattern = @"https://[a-zA-Z0-9\-]+\.artifact-registry\.x3mads\.com/maven";

        public AndroidPackageReplacer(string filePath)
        {
            _filePath = filePath;
        }

        public bool ReplaceAndroidPackageSpecAndRepository(bool undo)
        {
            if (!File.Exists(_filePath))
            {
                Debug.LogError($"File not found: {_filePath}");
                return false;
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(_filePath);

            var androidPackageUpdated = ReplaceAndroidPackageSpec(xmlDoc, undo);
            var repositoryUpdated = ReplaceRepositoryUrl(xmlDoc, undo);

            if (androidPackageUpdated || repositoryUpdated)
            {
                xmlDoc.Save(_filePath);
                Debug.Log("XML file updated and saved successfully.");
            }
            else
            {
                Debug.Log("No changes were made.");
            }

            return androidPackageUpdated || repositoryUpdated;
        }

        private bool ReplaceAndroidPackageSpec(XmlDocument xmlDoc, bool undo)
        {
            var oldPrefix = OldSpecPrefix;
            var newPrefix = NewSpecPrefix;
            if (undo)
            {
                oldPrefix = NewSpecPrefix;
                newPrefix = OldSpecPrefix;
            }

            var androidPackages = xmlDoc.GetElementsByTagName("androidPackage");
            var updated = false;

            foreach (XmlNode package in androidPackages)
            {
                if (package.Attributes != null && package.Attributes["spec"] == null) continue;
                if (package.Attributes == null) continue;
                var spec = package.Attributes["spec"].Value;
                
                if (!spec.StartsWith(OldSpecPrefix)) continue;
                var newSpec = spec.Replace(oldPrefix, newPrefix);
                package.Attributes["spec"].Value = newSpec;
                updated = true;
                Debug.Log($"Updated androidPackage spec to: {newSpec}");
            }

            return updated;
        }

        private static bool ReplaceRepositoryUrl(XmlDocument xmlDoc, bool undo)
        {
            if (undo)
            {
                return false;
            }

            var repositories = xmlDoc.GetElementsByTagName("repository");
            var updated = false;

            foreach (XmlNode repo in repositories)
            {
                if (!Regex.IsMatch(repo.InnerText, RepositoryUrlPattern)) continue;
                repo.InnerText = NewRepositoryUrl;
                updated = true;
                Debug.Log($"Updated repository URL to: {NewRepositoryUrl}");
            }

            return updated;
        }
    }
}