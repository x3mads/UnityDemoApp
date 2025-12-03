using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using XMediator.Api;
using XMediator.Unity.Views;

namespace XMediator.Unity
{
    internal class MockFullScreenAd
    {
        private MockToBackgroundAdListener _listener;
        private MockAdStatus _status = MockAdStatus.Created;
        private readonly string _uuid = Guid.NewGuid().ToString().Split('-').Last();
        private UnityFullScreenView _view;
        private readonly string _adType;
        private string _viewType;
        private string _placementId;

        public MockFullScreenAd(string adType, string viewType)
        {
            _adType = adType;
            _viewType = viewType;
        }

        public void Create(string placementId, MockToBackgroundAdListener listener)
        {
            Log($"{_adType} created for placement: {placementId}");
            _placementId = placementId;
            _listener = listener;
        }

        public async void Load(CustomProperties customProperties)
        {
            Log($"Called {_adType}#Load()");
            
            if (MockAdsConfiguration.FailToLoad)
            {
                _listener.OnFailedToLoad(MockAdsConfiguration.LoadError, MockMotherObject.EmptyLoadResult);
                return;
            }
            
            switch (_status)
            {
                case MockAdStatus.Created:
                    break;
                case MockAdStatus.Loading:
                case MockAdStatus.Loaded:
                case MockAdStatus.Presenting:
                    return;
                case MockAdStatus.Completed:
                    Log($"{_adType} failed to load: Already used. Please create a new instance to trigger a new load");
                    _listener.OnFailedToLoad(MockMotherObject.AlreadyUsedLoadError, MockMotherObject.EmptyLoadResult);
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            await Task.Delay(100);
            Log($"{_adType} prebidding finished");
            _listener.OnPrebiddingFinished(MockMotherObject.PrebiddingResults);
            await Task.Delay(1000);
            _status = MockAdStatus.Loaded;
            Log($"{_adType} loaded");
            _listener.OnLoaded(MockMotherObject.SuccessfulLoadResult);
        }

        public bool IsReady()
        {
            Log($"Called {_adType}#IsReady()");
            return _status == MockAdStatus.Loaded;
        }

        public bool IsAdSpaceCapped(string adSpace)
        {
            Log($"Called {_adType}#IsAdSpaceCapped()");
            return _status == MockAdStatus.Loaded;
        }

        public async void Show(string adSpace = null)
        {
            var adSpaceLog = adSpace != null ? $" from adSpace {adSpace}" : "";
            Log($"Called {_adType}#Show(){adSpaceLog}");
            
            if (MockAdsConfiguration.FailToShow)
            {
                _listener.OnFailedToShow(MockAdsConfiguration.ShowError);
                return;
            }
            
            switch (_status)
            {
                case MockAdStatus.Created:
                    Log($"{_adType} failed to show: Not requested. Please call Load() before");
                    _listener.OnFailedToShow(MockMotherObject.NotRequestedShowError);
                    return;
                case MockAdStatus.Loading:
                    Log($"{_adType} failed to show: Ad is loading. Please wait until it's loaded");
                    _listener.OnFailedToShow(MockMotherObject.LoadingShowError);
                    return;
                case MockAdStatus.Loaded:
                    break;
                case MockAdStatus.Presenting:
                case MockAdStatus.Completed:
                    Log($"{_adType} failed to show: Already used. Please create a new instance to trigger a new load");
                    _listener.OnFailedToShow(MockMotherObject.AlreadyUsedShowError);
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _status = MockAdStatus.Presenting;
            _view = UnityFullScreenView.Instantiate(_viewType);
            _view.OnClicked += () =>
            {
                Log("Received OnClicked");
                _listener.OnClicked();
            };
            _view.OnDismissed += () =>
            {
                _status = MockAdStatus.Completed;
                Log("Received OnDismissed");
                _listener.OnDismissed();
            };
            if (_viewType == "rew")
            {
                _view.OnEarnedReward += () =>
                {
                    Log("Received OnEarnedReward");
                    _listener.OnEarnedReward();
                }; 
            }

            _view.Show();
            await Task.Delay(200);
            Log($"{_adType} shown");
            _listener.OnShowed();
            
            await Task.Delay(100);
            var impressionData = MockMotherObject.ImpressionData(_placementId, adSpace);
            Log($"Impression with revenue {impressionData.Revenue}");
            _listener.OnImpression(impressionData);
        }

        public void Dispose()
        {
            Log($"Called {_adType}#Dispose()");
            _status = MockAdStatus.Completed;
        }
        
        private void Log(string message)
        {
            Debug.Log($"[XMed] ({_adType}: {_uuid}) {message}");
        }
    }
}