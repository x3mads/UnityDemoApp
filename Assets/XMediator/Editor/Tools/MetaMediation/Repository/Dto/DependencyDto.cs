using System;
using System.Collections.Generic;

namespace XMediator.Editor.Tools.MetaMediation.Repository.Dto
{
    [Serializable]
    public class DependencyDto
    {
        public string group_id;
        public string artifact_name;
        public List<string> repositories;
        public string suggested_version;
        public List<AvailableVersionDto> available_versions;
    }
}