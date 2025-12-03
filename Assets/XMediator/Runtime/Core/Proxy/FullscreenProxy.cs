using System;
using XMediator.Api;

namespace XMediator
{
    internal interface FullscreenProxy<FullscreenListener>: IDisposable
    {
        void Create(string placementId, FullscreenListener listener, bool test, bool verbose);
        void Load(CustomProperties customProperties);
        bool IsReady();
        bool IsAdSpaceCapped(string adSpace);
        void Show();
        void Show(string adSpace);
    }
}