using System;
using System.Collections.Generic;
using System.Linq;

namespace XMediator.Editor.Tools.SkAdNetworkIds
{
    internal class SkAdNetworkIdsService
    {
        private readonly NetworkIdsProvider _idsProvider;
        private readonly SkAdNetworkIdsRepository _skAdNetworkIdsRepository;
        private readonly SkAdNetworkIdsStorage _skAdNetworkIdsStorage;

        internal SkAdNetworkIdsService(NetworkIdsProvider idsProvider, SkAdNetworkIdsRepository skAdNetworkIdsRepository, SkAdNetworkIdsStorage skAdNetworkIdsStorage)
        {
            _idsProvider = idsProvider;
            _skAdNetworkIdsRepository = skAdNetworkIdsRepository;
            _skAdNetworkIdsStorage = skAdNetworkIdsStorage;
        }

        public void SynchronizeSkAdNetworkIds(Action<List<string>> onSuccess = null, Action<Exception> onError = null)
        {
            _idsProvider.GetIds(versions =>
            {
                var networks = versions.ToList();
                _skAdNetworkIdsRepository.GetIds(networks, ids =>
                {
                    _skAdNetworkIdsStorage.SaveIds(ids);
                    onSuccess?.Invoke(ids);
                }, exception =>
                {
                    onError?.Invoke(exception);
                });
            }, exception =>
            {
                onError?.Invoke(exception);
            });
        }
    }
    
    internal interface NetworkIdsProvider
    { 
        void GetIds(Action<IEnumerable<string>> onSuccess, Action<Exception> onError);
    }
}