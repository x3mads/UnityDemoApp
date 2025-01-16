using System;
using System.Collections.Generic;

namespace XMediator.Editor.Tools.MetaMediation.Repository.Dto
{
    [Serializable]
    public class IOSDependencyDto
    {
        public string pod;
        public string suggested_version;
        public List<AvailableVersionDto> available_versions;
    }
}