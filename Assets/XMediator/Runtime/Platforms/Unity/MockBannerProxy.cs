using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using XMediator.Api;
using XMediator.Unity.Views;

namespace XMediator.Unity
{
    internal class MockBannerProxy : BannerProxy
    {
        private MockToBackgroundAdListener _listener;
        private Banner.Size _size;
        private Banner.Position? _position;
        private int _positionX;
        private int _positionY;
        private string _adSpace;
        private bool _loading;
        private readonly string _uuid = Guid.NewGuid().ToString().Split('-').Last();
        private UnityEmbeddedScreenView _view;
        private bool _isHidden = true;

        public void Create(string placementId, BannerProxyListener listener, Banner.Position position, Banner.Size size, bool test, bool verbose)
        {
            Log($"Banner created for placement: {placementId}");
            _listener = new MockToBackgroundAdListener(listener, listener);
            _size = size;
            _position = position;
        }

        public void Create(string placementId, BannerProxyListener listener, Banner.Size size, int x, int y, bool test, bool verbose)
        {
            Log($"Banner created for placement: {placementId}");
            _listener = new MockToBackgroundAdListener(listener, listener);
            _size = size;
            _positionX = x;
            _positionY = y;
        }

        public async void Load(CustomProperties customProperties)
        {
            Log("Called Banner#Load()");
            
            _isHidden = false;
            
            if (MockAdsConfiguration.FailToLoad)
            {
                _listener.OnFailedToLoad(MockAdsConfiguration.LoadError, MockMotherObject.EmptyLoadResult);
                return;
            }

            if (_loading)
            {
                return;
            }

            _loading = true;
            await Task.Delay(100);
            _listener.OnPrebiddingFinished(MockMotherObject.PrebiddingResults);
            await Task.Delay(1000);
            _loading = false;

            if (_isHidden) return;
            
            Log("Banner loaded");
            _listener.OnLoaded(MockMotherObject.SuccessfulLoadResult);

            if (!_isHidden)
            {
                Show();
            }
        }

        public void SetPosition(int x, int y)
        {
            _positionX = x;
            _positionY = y;
        }

        public void SetAdSpace(string adSpace)
        {
            _adSpace = adSpace;
        }

        public void SetPosition(Banner.Position position)
        {
            _position = position;
        }

        public void Show()
        {
            var adSpaceLog = _adSpace != null ? $" from adSpace {_adSpace}" : "";
            Log($"Called Banner#Show(){adSpaceLog}");
            _isHidden = false;
            var position = _position;
            if (position == null)
            {
                InternalShow(_positionX, _positionY);
            }
            else
            {
                InternalShow((Banner.Position) position);
            }
        }

        public void Show(Banner.Position position)
        {
            _isHidden = false;
            SetPosition(position);
            Show();
        }

        public void Hide()
        {
            Log("Called Banner#Hide()");
            _isHidden = true;
            if (_view != null) _view.Hide();
        }
        
        public void Dispose()
        {
            Log("Called Banner#Dispose()");
            
            if (_view != null) _view.Hide();
        }
        
        private void InternalShow(Banner.Position position)
        {
            if (_view != null) _view.Hide();
            _view = UnityEmbeddedScreenView.Instantiate(position, _size);
            _view.OnClicked += () =>
            {
                Log("Received OnClicked");
                _listener.OnClicked();
            };
        }
        
        private void InternalShow(int x, int y)
        {
            if (_view != null) _view.Hide();
            _view = UnityEmbeddedScreenView.Instantiate(x, y, _size);
            _view.OnClicked += () =>
            {
                Log("Received OnClicked");
                _listener.OnClicked();
            };
        }

        private void Log(string message)
        {
            Debug.Log($"[XMed] (Banner: {_uuid}) {message}");
        }
    }
}