using System;
using System.Net.Http;
using UnityEngine;
using XMediator.Editor.Tools.MetaMediation.Repository.Dto;

namespace XMediator.Editor.Tools.MetaMediation.Repository
{
    internal class DependencyRepository
    {
        private const string AndroidManifestUrl = "https://android-artifact-registry.x3mads.com/android-versions-manifest.json";

        private const string IOSManifestUrl = "https://ios-artifact-registry.x3mads.com/ios-versions-manifest.json";

        internal class PlatformsDependency
        {
            public AndroidManifestDto AndroidManifest { get; set; }
            public IOSManifestDto IOSManifest { get; set; }
        }


        internal async void DownloadAndParseJson(Action<PlatformsDependency> onSuccess,
            Action<Exception> onError)
        {
            var now = DateTime.Now.Ticks.ToString();
            var httpClient = new HttpClient();
            var platformsDependency = new PlatformsDependency();
            try
            {
                var androidJson = await httpClient.GetStringAsync($"{AndroidManifestUrl}?{now}");
                var androidManifest = JsonUtility.FromJson<AndroidManifestDto>(androidJson);
                Debug.Log("Android JSON successfully downloaded and parsed.");
                Debug.Log("Generated: " + androidManifest);
                platformsDependency.AndroidManifest = androidManifest;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error downloading or parsing Android JSON: {e.Message}");
                onError.Invoke(e);
                return;
            }

            try
            {
                var iosJson = await httpClient.GetStringAsync($"{IOSManifestUrl}?{now}");
                Debug.Log("iOS JSON successfully downloaded: " + iosJson);
                var iosManifest = JsonUtility.FromJson<IOSManifestDto>(iosJson);
                Debug.Log("iOS JSON successfully parsed.");
                platformsDependency.IOSManifest = iosManifest;
                onSuccess.Invoke(platformsDependency);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error downloading or parsing iOS JSON: {e.Message}");
                onError.Invoke(e);
            }
        }
    }
}