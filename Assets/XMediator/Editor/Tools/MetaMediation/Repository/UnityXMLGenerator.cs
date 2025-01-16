using System;
using System.Collections.Generic;
using System.Xml.Linq;
using XMediator.Editor.Tools.MetaMediation.Entities;

namespace XMediator.Editor.Tools.MetaMediation.Repository
{
    internal static class UnityXMLGenerator
    {
        private static XElement GenerateDetailsTag(
            string name,
            string version,
            string description,
            string androidVersion,
            string iOSVersion)
        {
            XElement details = new XElement("details",
                new XElement("name", name),
                new XElement("version", version),
                new XElement("description", description),
                new XElement("androidVersion", androidVersion),
                new XElement("iOSVersion", iOSVersion),
                new XElement("androidDependencies"),
                new XElement("iOSDependencies"),
                new XElement("installedAt", DateTime.Now.Ticks)
            );

            return details;
        }

        public static XDocument BuildUnityXMLSnippet(ProcessedDependencies processedDependencies)
        {
            var androidRepositories = processedDependencies.AndroidRepositories;
            var androidArtifacts = processedDependencies.Artifacts;
            var iosPods = processedDependencies.Pods;
            XElement dependencies = new XElement("dependencies");

            XElement detailsTag = GenerateDetailsTag(
                name: "Metamediation",
                version: "1.0.0",
                description: "Metamediation Adapter for XMediator Unity.",
                androidVersion: "1.0.0",
                iOSVersion: "1.0.0"
            );
            dependencies.Add(detailsTag);

            var androidPackages = new XElement("androidPackages",
                new XElement("repositories", GenerateAndroidRepositories(androidRepositories))
            );

            androidPackages.Add(new XComment("Mediation Libraries"));
            foreach (var artifact in androidArtifacts)
            {
                androidPackages.Add(
                    new XElement("androidPackage",
                        new XAttribute("spec", artifact)
                    )
                );
            }

            dependencies.Add(androidPackages);

            var iosPodsElement = new XElement("iosPods");

            iosPodsElement.Add(new XComment("Mediation Libraries"));
            foreach (var iosPod in iosPods)
            {
                iosPodsElement.Add(
                    new XElement("iosPod",
                        new XAttribute("name", iosPod.Name),
                        new XAttribute("version", iosPod.Version)
                    )
                );
            }

            dependencies.Add(iosPodsElement);

            var xmlDoc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                dependencies
            );

            return xmlDoc;
        }

        private static XElement GenerateAndroidRepositories(HashSet<string> androidRepositories)
        {
            var repositoriesElement = new XElement("repositories");

            foreach (var repo in androidRepositories)
            {
                repositoriesElement.Add(new XElement("repository", repo));
            }

            return repositoriesElement;
        }
    }
}