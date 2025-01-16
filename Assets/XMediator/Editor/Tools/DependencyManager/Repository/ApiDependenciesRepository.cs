using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using UnityEngine;
using XMediator.Editor.Tools.DependencyManager.Entities;
using XMediator.Editor.Tools.DependencyManager.Repository.Dto;

namespace XMediator.Editor.Tools.DependencyManager.Repository
{
    internal class ApiDependenciesRepository : DependenciesRepository
    {
        private static string BaseUrl = "https://api.x3mads.com/general-config";
        private const string DependenciesEndpoint = "/plugin/dependencies";

        private readonly HttpClient _httpClient = new HttpClient();
        
        private static ApiDependenciesRepository _instance;

        internal static ApiDependenciesRepository Instance =>
            _instance ??= new ApiDependenciesRepository(); 

        private ApiDependenciesRepository()
        {
            _httpClient.DefaultRequestHeaders
                .Add("User-Agent", $"XMediator Unity IDE Tool/1.0 Unity/{Application.unityVersion} OS/{SystemInfo.operatingSystem}");
        }
        
        public async void FindDependencies(string publisher, IEnumerable<ClientDependencyDto> clientDependencies, Action<AvailableDependencies> onSuccess, Action<Exception> onError)
        {
            try
            {
                var requestJson = JsonUtility.ToJson(new GetDependenciesRequestDto(clientDependencies.ToArray()));
                var uriBuilder = new UriBuilder(BaseUrl);
                uriBuilder.Path += DependenciesEndpoint;
                uriBuilder.Query += $"publisher={publisher}";

                var httpResponseMessage = (await _httpClient.PostAsync(uriBuilder.Uri,
                        new StringContent(requestJson, Encoding.UTF8, "application/json")))
                    .EnsureSuccessStatusCode();
                var responseJson = await httpResponseMessage.Content.ReadAsStringAsync();
                var dto = JsonUtility.FromJson<GetDependenciesResponseDto>(responseJson);
                var dependencies = dto.ToDependenciesModel();
                onSuccess(dependencies);
            }
            catch (Exception exception)
            {
                onError(exception);
            }
        }
        
    }
}