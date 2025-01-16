using XMediator.Editor.Tools.DependencyManager.Entities;

namespace XMediator.Editor.Tools.MetaMediation
{
    using System.Xml;

    internal abstract class DependencyVersionResolver
    {
        public class XmlDependencyVersions
        {
            public SemanticVersion Version { get; set; }
            public SemanticVersion AndroidVersion { get; set; }
            public SemanticVersion IOSVersion { get; set; }

            public override string ToString()
            {
                return
                    $"DependencyVersions {{ Version = {Version}, AndroidVersion = {AndroidVersion}, IOSVersion = {IOSVersion} }}";
            }
        }

        public static XmlDependencyVersions RetrieveXmlVersions(string xmlFilePath)
        {
            var doc = new XmlDocument();
            doc.Load(xmlFilePath);

            var versions = new XmlDependencyVersions();

            var detailsNode = doc.SelectSingleNode("//details");
            if (detailsNode == null) return versions;
            versions.Version = new SemanticVersion(GetElementValue(detailsNode, "version"));
            versions.AndroidVersion = new SemanticVersion(GetElementValue(detailsNode, "androidVersion"));
            versions.IOSVersion = new SemanticVersion(GetElementValue(detailsNode, "iOSVersion"));

            return versions;
        }

        private static string GetElementValue(XmlNode parent, string elementName)
        {
            var node = parent.SelectSingleNode(elementName);
            return node?.InnerText;
        }
    }
}