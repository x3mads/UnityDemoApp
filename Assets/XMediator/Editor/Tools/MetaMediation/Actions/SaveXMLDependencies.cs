using System.IO;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

namespace XMediator.Editor.Tools.MetaMediation.Actions
{
    internal abstract class SaveXMLDependencies
    {
        public static void SaveXMLToFile(XDocument xmlDoc, string fileName)
        {
            var folderPath = Path.Combine(Application.dataPath, "XMediatorSettings", "Editor");
            
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            
            var filePath = Path.Combine(folderPath, fileName);
            
            try
            {
                if (CheckForChanges(xmlDoc, filePath))
                {
                    xmlDoc.Save(filePath);
                    AssetDatabase.Refresh();
                }
                else
                {
                    Debug.Log("No new changes in dependencies.");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to save XML: " + e.Message);
            }
        }

        private static bool CheckForChanges(XDocument xmlDoc, string filePath)
        {
            try
            {
                XDocument previousXml = XDocument.Load(filePath);
                var previousInstalledAt = previousXml.Root?.Element("details")?.Element("installedAt");
                var currentInstalledAt = xmlDoc.Root?.Element("details")?.Element("installedAt");
                if (previousInstalledAt == null || currentInstalledAt == null)
                {
                    return true;
                }
            
                previousInstalledAt.SetValue(currentInstalledAt.Value);
                
                return !XNode.DeepEquals(previousXml, xmlDoc);
            }
            catch (System.Exception e)
            {
                return true;
            }
        }
    }

}