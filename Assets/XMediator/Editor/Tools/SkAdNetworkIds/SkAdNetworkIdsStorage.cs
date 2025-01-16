using System;
using System.Collections.Generic;

namespace XMediator.Editor.Tools.SkAdNetworkIds
{
    internal interface SkAdNetworkIdsStorage
    {
        List<String> GetIds();
        void SaveIds(List<string> ids);
    }
}