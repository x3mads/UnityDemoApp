using System;
using System.Collections.Generic;
using System.Linq;

namespace XMediator.Editor.Tools.MetaMediation.Repository.Dto
{
    [Serializable]
    internal class IOSManifestDto: FlavoredManifestDto<IOSDependencyDto>
    {
        internal List<string> GetLegacySupportTags()
        {
            return GetAllTagsInFlavors().Where(t => t.ToLower().StartsWith("xcode_")).ToList();
        }
    }
}