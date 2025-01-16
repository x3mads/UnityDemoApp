using System;

namespace XMediator.Editor.Tools.DependencyManager.Repository.Dto
{
    [Serializable]
    internal class DependencyVersionDto
    {
        public string version;
        public string android_version;
        public string ios_version;
        public string download_url;
        public string download_filename;
        public bool preferred;
        public bool is_compliant;
        public DependencyRuleDto[] non_compliant_rules;
    }
}