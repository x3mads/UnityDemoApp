using System;
using XMediator.Api;

namespace XMediator
{
    internal interface BannerProxy : IDisposable
    {
        void Create(
            string placementId,
            BannerProxyListener listener,
            Banner.Position position,
            Banner.Size size,
            bool test,
            bool verbose
        );
        
        void Create(
            string placementId,
            BannerProxyListener listener,
            Banner.Size size,
            int x,
            int y,
            bool test,
            bool verbose
        );

        void Load(CustomProperties customProperties);
        void SetPosition(int x, int y);
        void SetAdSpace(string adSpace);
        void Show();
        void Show(Banner.Position position);
        void Hide();
    }
}