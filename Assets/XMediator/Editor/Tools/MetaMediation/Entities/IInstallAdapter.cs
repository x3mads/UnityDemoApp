using System;
using XMediator.Editor.Tools.DependencyManager.Entities;

namespace XMediator.Editor.Tools.MetaMediation.Entities
{
    internal interface IInstallAdapter
    {
        void Invoke(string path, Action onSuccess, Action<string> onError);
    }
}