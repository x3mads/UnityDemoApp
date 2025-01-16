using System.Threading.Tasks;
using XMediator.Api;

namespace XMediator.Unity
{
    internal class MockToBackgroundAdListener: LoadProxyListener, ShowProxyListener, RewardProxyListener
    {
        private readonly LoadProxyListener _loadListener;
        private readonly ShowProxyListener _showListener;
        private readonly RewardedProxyListener _rewardedListener;

        internal MockToBackgroundAdListener(LoadProxyListener loadListener, ShowProxyListener showListener, RewardedProxyListener rewardedListener = null)
        {
            _loadListener = loadListener;
            _showListener = showListener;
            _rewardedListener = rewardedListener;
        }

        public void OnPrebiddingFinished(PrebiddingResults result)
        {
            Task.Run(() => _loadListener.OnPrebiddingFinished(result));
        }

        public void OnLoaded(LoadResult loadResult)
        {
            Task.Run(() => _loadListener.OnLoaded(loadResult));
        }

        public void OnFailedToLoad(LoadError loadError, LoadResult loadResult)
        {
            Task.Run(() => _loadListener.OnFailedToLoad(loadError, loadResult));
        }

        public void OnFailedToShow(ShowError showError)
        {
            Task.Run(() => _showListener.OnFailedToShow(showError));
        }

        public void OnShowed()
        {
            Task.Run(() => _showListener.OnShowed());
        }

        public void OnImpression(ImpressionData impressionData)
        {
            Task.Run(() => _showListener.OnImpression(impressionData));
        }

        public void OnClicked()
        {
            Task.Run(() => _showListener.OnClicked());
        }

        public void OnDismissed()
        {
            Task.Run(() => _showListener.OnDismissed());
        }

        public void OnEarnedReward()
        {
            Task.Run(() => _rewardedListener?.OnEarnedReward());
        }
    }
}