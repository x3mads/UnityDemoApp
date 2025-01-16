using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace XMediator.Editor.Tools.MetaMediation
{
    internal class XMediatorDependenciesCleaner
    {
        private const string DirectoryPath = "Assets/XMediatorSettings/Editor/";
        private const string FilePattern = @"^XMediator\w+Dependencies\.xml$";

        public void RemoveDependenciesFiles(string excludePrefix = null)
        {
            if (!Directory.Exists(DirectoryPath))
            {
                Debug.LogError($"Directory not found: {DirectoryPath}");
                return;
            }

            var files = Directory.GetFiles(DirectoryPath);

            var regex = new Regex(FilePattern);

            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);

                // Check if the file name matches the pattern for XMediator[Network]Dependencies.xml
                if (!regex.IsMatch(fileName)) continue;
                if (excludePrefix != null && fileName.StartsWith("XMediator" + excludePrefix)) continue;
            
                Debug.Log("Deleting " + fileName);
                DeleteFile(file);
                var metaFile = file + ".meta";
                if (!File.Exists(metaFile)) continue;
                Debug.Log("Deleting " + metaFile);
                DeleteFile(metaFile);
            }

            Debug.Log("Dependencies cleanup complete.");
        }

        private static void DeleteFile(string filePath)
        {
            try
            {
                File.Delete(filePath);
                Debug.Log($"Deleted file: {filePath}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error deleting file {filePath}: {ex.Message}");
            }
        }
    }
}