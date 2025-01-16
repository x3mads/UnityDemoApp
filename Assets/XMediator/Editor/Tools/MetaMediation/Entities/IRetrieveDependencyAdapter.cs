using System;
using XMediator.Editor.Tools.DependencyManager.Entities;

namespace XMediator.Editor.Tools.MetaMediation.Entities
{
    internal interface IRetrieveDependencyAdapter
    {
        void Invoke(AvailableVersion dependency, Action<string> onSuccess, Action<Exception> onError);
    }
}