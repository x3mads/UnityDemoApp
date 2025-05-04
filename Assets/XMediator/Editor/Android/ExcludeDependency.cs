using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace XMediator.Editor.Android
{
    [Serializable]
    public class ExcludeDependency
    {
        public string group;
        public string module;
        public string version;
        [CanBeNull] public List<ExcludeItem> excludes;
    }

    [Serializable]
    public class ExcludeItem
    {
        public string group;
        public string module;
    }
    
    [Serializable]
    internal class ExcludeDependencyWrapper
    {
        public List<ExcludeDependency> excludeDependencies;
    }
}