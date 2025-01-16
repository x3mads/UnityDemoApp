using System;

namespace XMediator.Editor.Tools.DependencyManager.Repository
{
    internal interface AssetRepository
    {
        void ImportPackage(string path, Action<bool, string> onComplete);
    }
}