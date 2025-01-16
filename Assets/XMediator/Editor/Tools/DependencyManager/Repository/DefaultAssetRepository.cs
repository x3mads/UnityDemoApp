using System;
using System.IO;
using UnityEditor;

namespace XMediator.Editor.Tools.DependencyManager.Repository
{
    internal class DefaultAssetRepository : AssetRepository
    {
        private event Action<string, bool, string> OnComplete;

        internal DefaultAssetRepository()
        {
            AssetDatabase.importPackageCompleted += name => { OnComplete?.Invoke(name, true, null); };
            AssetDatabase.importPackageCancelled += name => { OnComplete?.Invoke(name, false, "Import package cancelled."); };
            AssetDatabase.importPackageFailed += (name, reason) => { OnComplete?.Invoke(name, false, reason); };
        }

        public void ImportPackage(string path, Action<bool, string> onComplete)
        {
            var installPackageName = Path.GetFileNameWithoutExtension(path);
            OnComplete = (packageName, isSuccess, errorReason) =>
            {
                if (!packageName.Equals(installPackageName) && !path.Contains(packageName)) return;
                OnComplete = null;
                onComplete(isSuccess, errorReason);
            };
            AssetDatabase.ImportPackage(path, false);
        }
    }
}