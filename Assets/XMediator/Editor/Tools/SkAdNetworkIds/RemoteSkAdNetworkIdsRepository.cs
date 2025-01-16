using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using UnityEngine;

namespace XMediator.Editor.Tools.SkAdNetworkIds
{
    internal class RemoteSkAdNetworkIdsRepository : SkAdNetworkIdsRepository
    {
        private static string BaseUrl => "https://api.x3mads.com/general-config/skadnetworks";
        private readonly HttpClient _httpClient = new HttpClient();

        internal RemoteSkAdNetworkIdsRepository()
        {
            _httpClient.DefaultRequestHeaders.Add("User-Agent", $"XMediator Unity IDE Tool/1.0 Unity/{Application.unityVersion} OS/{SystemInfo.operatingSystem}");
        }

        public async void GetIds(List<string> networks, Action<List<string>> onSuccess, Action<Exception> onError)
        {
            try
            {
                var uriBuilder = new UriBuilder(BaseUrl);
                if (networks.Count > 0)
                {
                    uriBuilder.Query += $"partners={string.Join(",", networks)}";                    
                }

                var json = await _httpClient.GetStringAsync(uriBuilder.Uri);
                var ids = JsonUtility.FromJson<SkAdNetworkIdsDto>(json).ids
                    .Select(id => id.ToLower())
                    .Distinct()
                    .OrderBy(id => id)
                    .ToList();
                ids.Sort();
                
                onSuccess(ids);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                onError(exception);
            }
        }

        [Serializable]
        internal class SkAdNetworkIdsDto
        {
            [SerializeField] internal List<string> ids;
        }
    }
}