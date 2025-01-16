using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using UnityEditor;
using UnityEngine;
using XMediator.Editor.Tools.DependencyManager.Entities;

namespace XMediator.Editor.Tools.DependencyManager.Repository
{
    internal class DefaultDependencyManagerRepository : DependencyManagerRepository
    {

        private static readonly string TempFolder = FileUtil.GetUniqueTempPathInProject();

        private static readonly string LocalDependenciesFolder =
            Path.Combine(Application.dataPath, "XMediatorSettings", "Editor");

        private static readonly string SdkXmlPath =
            Path.Combine(Application.dataPath, "XMediatorSettings", "Editor", "Main","XMediatorDependencies.xml");

        public async void LoadInstalledVersions(Action<InstalledVersions> onSuccess, Action<Exception> onError,
            Dependency installed = null)
        {
            try
            {
                var installedVersions = await LoadInstalledVersionsFromXMLFiles(installed);
                onSuccess(installedVersions);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                onError(exception);
            }
        }

        public async void DownloadFile(string downloadUrl, string filename, Action<string> onSuccess,
            Action<Exception> onError)
        {
            try
            {
                var path = await WebClientDownloadFile(downloadUrl, filename);
                onSuccess(path);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                onError(exception);
            }
        }

        private static Task<string> WebClientDownloadFile(string downloadUrl, string filename) => Task.Run(() =>
        {
            using (var client = new WebClient())
            {
                EnsureDirectory(TempFolder);
                client.DownloadFile(downloadUrl, Path.Combine(TempFolder, filename));
                return Path.Combine("Temp", Path.GetFileName(TempFolder), filename);
            }
        });

        private static Task<InstalledVersions> LoadInstalledVersionsFromXMLFiles(Dependency installed) => Task.Run(() =>
        {
            EnsureDirectory(LocalDependenciesFolder);
            return new InstalledVersions(
                sdk: GetInstalledSDKVersion(installed),
                adapters: GetMediationDependencies(installed)
            );
        });

        private static InstalledVersion GetInstalledSDKVersion(Dependency installed)
        {
            return !File.Exists(SdkXmlPath) ? null : XmlToInstalledVersion(SdkXmlPath, installed);
        }

        private static IOrderedEnumerable<InstalledVersion> GetMediationDependencies(Dependency installed)
        {
            return Directory.GetFiles(LocalDependenciesFolder, "*.xml")
                .Where(s => !s.Equals(Path.Combine(LocalDependenciesFolder, "XMediatorSettings.xml")))
                .Where(File.Exists)
                .Select(s => XmlToNullableInstalledVersion(installed, s))
                .Where(s => s != null)
                .ToList()
                .OrderBy(version => version.InstalledAt);
        }

        private static InstalledVersion XmlToNullableInstalledVersion(Dependency installed, string s)
        {
            try
            {
                return XmlToInstalledVersion(s, installed);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                return null;
            }
        }

        private static InstalledVersion XmlToInstalledVersion(string filename, Dependency installed)
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

            if (string.IsNullOrEmpty(installedAt.InnerText))
            {
                installedAt.InnerText = installed?.InstalledAt == null
                    ? (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond).ToString()
                    : installed.InstalledAt.ToString();
                document.Save(filename);
            }

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
            var unaffectedAdapters = new List<string> { "Criteo", "IronSource",
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
    }
}