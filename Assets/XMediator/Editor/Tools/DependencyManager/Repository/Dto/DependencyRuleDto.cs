using System;

namespace XMediator.Editor.Tools.DependencyManager.Repository.Dto
{
    [Serializable]
    internal class DependencyRuleDto
    {
        public string dependency;
        public string version_type;
        public string from;
        public string to;
    }
}