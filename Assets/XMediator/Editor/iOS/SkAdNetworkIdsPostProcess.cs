#if UNITY_IOS

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using XMediator.Editor.Tools.Settings;
using XMediator.Editor.Tools.SkAdNetworkIds;

namespace XMediator.Editor.iOS
{
    public static class SkAdNetworkIdsPostProcess
    {
        private const string SkAdNetworkItemsKey = "SKAdNetworkItems";
        private const string SkAdNetworkIDKey = "SKAdNetworkIdentifier";
        
        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
        {
            if (buildTarget != BuildTarget.iOS) return;
            
            XMediatorSettingsService.Instance.ReloadSettings();
            if (XMediatorSettingsService.Instance.IsSkipSkAdNetworkIdsConfiguration) return;
            
            var plistPath = Path.Combine(buildPath, "Info.plist");
            var plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            var ids = new JsonFileSkAdNetworkIdsStorage().GetIds();
            var adNetworkItems = plist.root[SkAdNetworkItemsKey] as PlistElementArray ??
                                 plist.root.CreateArray(SkAdNetworkItemsKey);

            AddIdsToPlistArray(ids, adNetworkItems);

            plist.WriteToFile(plistPath);
        }
        
        private static void AddIdsToPlistArray(List<string> ids, PlistElementArray idsInPlist)
        {
            var existingIds = idsInPlist.values
                .OfType<PlistElementDict>()
                .Select(dict => (dict[SkAdNetworkIDKey] as PlistElementString)?.value)
                .Where(id => !string.IsNullOrEmpty(id))
                .ToList();
            
            foreach (string id in ids)
            {
                if (!existingIds.Contains(id))
                {
                    idsInPlist.AddDict().SetString(SkAdNetworkIDKey, id);
                }
            }
        }
    }
}

#endif