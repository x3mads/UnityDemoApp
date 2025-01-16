using XMediator.Api;

namespace XMediator
{
    internal interface LoadProxyListener
    {
        void OnPrebiddingFinished(PrebiddingResults result);
        void OnLoaded(LoadResult loadResult);
        void OnFailedToLoad(LoadError loadError, LoadResult loadResult);
    }
}