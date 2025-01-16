using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using XMediator.Editor.Tools.MetaMediation.Entities;
using static XMediator.Editor.Tools.MetaMediation.Entities.MetaMediationDependencies;

namespace XMediator.Editor.Tools.MetaMediation.Repository
{
    internal interface IMetaMediationDependenciesRepository
    {
        MetaMediationDependencies Invoke();
    }

    internal class MetaMediationDependenciesRepository : IMetaMediationDependenciesRepository
    {
        private const string XMLFilePath = "Assets/XMediatorSettings/Editor/MetaMediationDependencies.xml";

        public MetaMediationDependencies Invoke()
        {
            if (!File.Exists(XMLFilePath))
            {
                Debug.Log($"Not found: {XMLFilePath}");
                return null;
            }

            var doc = XDocument.Load(XMLFilePath);

            var androidPackages = doc.Descendants("androidPackage")
                .Select(p =>
                {
                    var spec = p.Attribute("spec").Value.Split(':');
                    return new AndroidPackage
                    {
                        Group = spec[0],
                        Artifact = spec[1],
                        Version = spec[2]
                    };
                })
                .ToList();

            var iosPods = doc.Descendants("iosPod")
                .Select(p => new IosPod
                {
                    Name = p.Attribute("name").Value,
                    Version = p.Attribute("version").Value
                })
                .ToList();

            return new MetaMediationDependencies(androidPackages, iosPods);
        }
    }
}