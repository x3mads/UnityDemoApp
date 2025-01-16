using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using XMediator.Editor.Tools.MetaMediation;

namespace XMediator.Editor.Tools.DependencyManager.Actions
{
    public class UpdateAndroidDependenciesToPublic
    {
        private const string DirectoryPath = "Assets/XMediatorSettings/Editor/";
        private const string FilePattern = @"^XMediator\w+Dependencies\.xml$";
        
        private static readonly string LocalDependenciesFolder = Path.Combine(Application.dataPath, "XMediatorSettings", "Editor");

        public void Execute()
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
                var fileName = Path.GetFileName(Path.Combine(LocalDependenciesFolder, file));

                // Check if the file name matches the pattern for XMediator[Network]Dependencies.xml
                if (!regex.IsMatch(fileName)) continue;

                var packageReplacer = new AndroidPackageReplacer(filePath: file);
                packageReplacer.ReplaceAndroidPackageSpecAndRepository(false);
            }
        }
    }
}