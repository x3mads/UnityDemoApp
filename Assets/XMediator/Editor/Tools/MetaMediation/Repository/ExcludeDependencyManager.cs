using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using XMediator.Editor.Android;
using XMediator.Editor.Tools.MetaMediation.Repository.Dto;

namespace XMediator.Editor.Tools.MetaMediation.Repository
{
    [Serializable]
    internal class ExcludeDependencyWrapper
    {
        public List<ExcludeDependency> excludeDependencies;
    }

    public static class ExcludeDependencyManager
    {
        private static readonly string ExcludeDependenciesJsonPath =
            Path.Combine(Application.dataPath, "XMediatorSettings", "android_exclude_dependencies.json");

        /// <summary>
        /// Extracts exclude dependencies from the Android manifest and saves them to a JSON file.
        /// </summary>
        /// <param name="androidManifest">The Android manifest containing dependency information</param>
        internal static void ExtractAndSaveExcludeDependencies(ManifestDto<AndroidDependencyDto> androidManifest)
        {
            var excludesDependencies = new List<AndroidDependencyDto>();
            foreach (var versionsNetwork in androidManifest.networks)
            {
                foreach (var versionsNetworkAdapter in versionsNetwork.adapters)
                {
                    excludesDependencies.AddRange(versionsNetworkAdapter.dependencies.FindAll(dependency => dependency.excludes is { Count: > 0 }));
                }

                excludesDependencies.AddRange(androidManifest.core.FindAll(dependency => dependency.excludes is { Count: > 0 }));
            }

            if (excludesDependencies.Count <= 0) return;

            // Filter out nulls by using Where and making sure the excludes are not null
            var excludeList = excludesDependencies
                .Where(dto => dto.excludes != null && dto.excludes.Count > 0)
                .Select(dto => new ExcludeDependency
                {
                    group = dto.group_id,
                    module = dto.artifact_name,
                    version = dto.suggested_version,
                    excludes = dto.excludes.Select(exclude => new ExcludeItem
                    {
                        group = exclude.group,
                        module = exclude.module
                    }).ToList()
                })
                .ToList();

            // If no valid exclude dependencies were found, don't create the file
            if (excludeList.Count <= 0)
            {
                Debug.Log("No valid exclude dependencies found. JSON file not created.");
                return;
            }

            // Convert excludeList to JSON
            var wrapper = new ExcludeDependencyWrapper { excludeDependencies = excludeList };
            string json = JsonUtility.ToJson(wrapper, true);

            // Create directory if it doesn't exist
            string directoryPath = Path.Combine(Application.dataPath, "XMediatorSettings");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Save to JSON file
            File.WriteAllText(ExcludeDependenciesJsonPath, json);
            Debug.Log($"Saved {excludeList.Count} exclude dependencies to {ExcludeDependenciesJsonPath}");
        }

        /// <summary>
        /// Loads the exclude dependencies from the JSON file.
        /// </summary>
        /// <returns>A list of exclude dependencies, or an empty list if the file doesn't exist or is invalid</returns>
        public static List<ExcludeDependency> LoadExcludeDependencies()
        {
            if (!File.Exists(ExcludeDependenciesJsonPath))
            {
                Debug.Log("No exclude dependencies JSON found at " + ExcludeDependenciesJsonPath);
                return new List<ExcludeDependency>();
            }

            try
            {
                string json = File.ReadAllText(ExcludeDependenciesJsonPath);
                var wrapper = JsonUtility.FromJson<ExcludeDependencyWrapper>(json);

                // Filter out any potentially empty or null objects in the loaded list
                return wrapper.excludeDependencies?.Where(dep =>
                           dep != null &&
                           !string.IsNullOrEmpty(dep.group) &&
                           !string.IsNullOrEmpty(dep.module) &&
                           dep.excludes != null &&
                           dep.excludes.Count > 0).ToList()
                       ?? new List<ExcludeDependency>();
            }
            catch (Exception e)
            {
                Debug.LogError("Error reading exclude dependencies JSON: " + e.Message);
                return new List<ExcludeDependency>();
            }
        }
    }
}