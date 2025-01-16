using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace XMediator.Editor.Tools.SkAdNetworkIds
{
    public class JsonFileSkAdNetworkIdsStorage : SkAdNetworkIdsStorage
    {
        private static readonly string SkAdNetworkIdsJsonPath =
            Path.Combine(Application.dataPath, "XMediatorSettings", "Editor", "SKAdNetworkIds.json");

        public List<String> GetIds()
        {
            var ids = new List<string>(); 
            if (File.Exists(SkAdNetworkIdsJsonPath))
            {
                try
                {
                    var fileContents = File.ReadAllText(SkAdNetworkIdsJsonPath);
                    var jsonIds = JsonUtility.FromJson<SkAdNetworkIdsDto>(fileContents);
                    ids = jsonIds.ids;
                }
                catch (Exception)
                {
                    // default to empty ids
                }
            }

            return ids;
        }

        public void SaveIds(List<string> ids)
        {
            var jsonIds = new SkAdNetworkIdsDto
            {
                ids = ids
            };

            string jsonString = JsonUtility.ToJson(jsonIds);
            File.WriteAllText(SkAdNetworkIdsJsonPath, jsonString);
        }
        
        [Serializable]
        internal class SkAdNetworkIdsDto
        {
            [SerializeField] internal List<string> ids;
        }
    }
}