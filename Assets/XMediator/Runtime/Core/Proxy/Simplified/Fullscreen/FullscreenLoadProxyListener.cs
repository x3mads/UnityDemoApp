using XMediator.Api;

namespace XMediator
{
    internal interface FullscreenLoadProxyListener
    {
        void OnLoaded(string placementId, LoadResult loadResult);
    }
}