using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;
using XMediator.Editor.Tools.DependencyManager.Entities;

namespace XMediator.Editor.Tools.DependencyManager.Repository
{
    internal class XmlInstalledDependenciesRepository : InstalledDependenciesRepository
    {
        
        private static readonly string SdkXmlPath =
            Path.Combine(Application.dataPath, "XMediatorSettings", "Editor", "Main","XMediatorDependencies.xml");
        
        private static readonly string LocalDependenciesFolder =
            Path.Combine(Application.dataPath, "XMediatorSettings", "Editor");
        
        private static XmlInstalledDependenciesRepository _instance;

        internal static XmlInstalledDependenciesRepository Instance =>
            _instance ?? (_instance = new XmlInstalledDependenciesRepository());
        
        private XmlInstalledDependenciesRepository(){}
        
        public InstalledVersion GetXMediatorDependency()
        {
            EnsureDirectory(LocalDependenciesFolder);
			return GetInstalledSDKVersion();
        }

        public IEnumerable<InstalledVersion> GetAdaptersDependencies()
        {
            EnsureDirectory(LocalDependenciesFolder);
            return GetMediationDependencies();
        }
        
        public bool RemoveDependency(string filename)
        {
            var localXMlPath = Path.Combine(LocalDependenciesFolder,filename);
            var localMetaPath = Path.Combine(LocalDependenciesFolder,filename+".meta");
            if (!File.Exists(localXMlPath)) return false;
            File.Delete(localXMlPath);
            File.Delete(localMetaPath);
            return true;
        }
        
        private static InstalledVersion GetInstalledSDKVersion()
        {
            return !File.Exists(SdkXmlPath) ? null : XmlToInstalledVersion(SdkXmlPath);
        }
        
        private static InstalledVersion XmlToInstalledVersion(string filename)
        {
            var document = new XmlDocument();
            document.Load(filename);
            var dependencies = document["dependencies"] ?? MissingTagException(filename, "dependencies");
            var details = dependencies["details"] ?? MissingTagException(filename, "details");
            var name = details["name"] ?? MissingTagException(filename, "name");
            var description = details["description"] ?? MissingTagException(filename, "description");
            var version = details["version"] ?? MissingTagException(filename, "version");
            var installedAt = details["installedAt"] ?? details.AppendChild(document.CreateElement("installedAt"));
            var androidDependencies = details["androidDependencies"];
            var iOSDependencies = details["iOSDependencies"];

            return new InstalledVersion(
                name: name.InnerText,
                description: description.InnerText,
                version: new SemanticVersion(version.InnerText),
                installedAt: long.Parse(installedAt.InnerText),
                installedAndroidVersion: details["androidVersion"]?.InnerText,
                installedIOSVersion: details["iOSVersion"]?.InnerText,
                androidConstraints: BuildDependencyConstraints(androidDependencies, false),
                iOSConstraints: BuildDependencyConstraints(iOSDependencies, MrecConstraintFixApplies(name.InnerText))
            );
        }
        
        private static List<DependencyConstraint> BuildDependencyConstraints(XmlElement dependencies,
            bool mrecConstraintFixApplies)
        {
            if (dependencies == null)
            {
                return new List<DependencyConstraint>();
            }
            
            return dependencies.ChildNodes
                .Cast<XmlNode>()
                .Select(dep => ToDependencyConstraint(dep, mrecConstraintFixApplies))
                .Where(dep => dep != null)
                .ToList();
        }

        private static DependencyConstraint ToDependencyConstraint(XmlNode dependency, bool mrecConstraintFixApplies)
        {
            var name = dependency.Attributes?["name"].Value;
            var minVersion = SafelyCreateVersionFromString(dependency.Attributes?["min"].Value);
            var maxVersion = SafelyCreateVersionFromString(dependency.Attributes?["max"].Value);

            maxVersion = FixMrecConstraints(maxVersion, minVersion, mrecConstraintFixApplies);
            
            return new DependencyConstraint(name, minVersion, maxVersion);
        }

        private static Version FixMrecConstraints(Version maxVersion, Version minVersion, bool applies)
        {
            var fixedVersion = new Version(1, 3, 0);
            if (applies &&
                maxVersion == new Version(2, 0, 0)
                && minVersion != null
                && minVersion < fixedVersion)
            {
                maxVersion = fixedVersion;
            }

            return maxVersion;
        }

        private static bool MrecConstraintFixApplies(string name)
        {
            var unaffectedAdapters = new List<string> { "Criteo", "Tapjoy", "IronSource",
                "Ogury", "Mintegral" };
            return !unaffectedAdapters.Contains(name);
        }
        
        private static Version SafelyCreateVersionFromString(string version)
        {
            try
            {
                return new Version(version);
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        private static void EnsureDirectory(string path)
        {
            try
            {
                Directory.CreateDirectory(path);
            }
            catch (IOException)
            {
                // ignored
            }
        }
        
        private static XmlElement MissingTagException(string filename, string tag)
        {
            throw new Exception(Path.GetFileName(filename) + $" is missing tag \"{tag}\".");
        }

        private static IOrderedEnumerable<InstalledVersion> GetMediationDependencies()
        {
            return Directory.GetFiles(LocalDependenciesFolder, "*.xml")
                .Where(filename => !filename.Equals(Path.Combine(LocalDependenciesFolder, "XMediatorSettings.xml")))
                .Where(File.Exists)
                .Select(filename => XmlToNullableInstalledVersion(filename))
                .Where(filename => filename != null)
                .ToList()
                .OrderBy(version => version.InstalledAt);
        }
        
        
        private static InstalledVersion XmlToNullableInstalledVersion(string filename)
        {
            try
            {
                return XmlToInstalledVersion(filename);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                return null;
            }
        }
}
}