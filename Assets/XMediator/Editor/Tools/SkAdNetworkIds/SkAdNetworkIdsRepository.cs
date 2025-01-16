using System;
using System.Collections.Generic;

namespace XMediator.Editor.Tools.SkAdNetworkIds
{
    internal interface SkAdNetworkIdsRepository
    {
        void GetIds(List<string> networks, Action<List<string>> onSuccess, Action<Exception> onError);
    }
}