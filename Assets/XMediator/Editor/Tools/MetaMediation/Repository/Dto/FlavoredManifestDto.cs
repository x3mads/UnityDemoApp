using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;

namespace XMediator.Editor.Tools.MetaMediation.Repository.Dto
{
    [Serializable]
    internal class FlavoredManifestDto<TDependencyDto>
    {
        [SerializeField] internal List<FlavorDto<TDependencyDto>> flavors;
        
        internal ManifestDto<TDependencyDto> GetDefaultManifest() => flavors.Find(f => f.name == "Default").versions;
        
        internal ManifestDto<TDependencyDto> GetManifestForMediators(List<string> mediators, string tag = null)
        {
            return GetManifestForTag(tag) ??
                   GetBestManifestMatchForMediators(mediators) ?? 
                   GetDefaultManifest();
        }
        
        internal List<string> GetAllTagsInFlavors() => flavors.SelectMany(f => f.tags).ToList();

        private ManifestDto<TDependencyDto> GetBestManifestMatchForMediators(List<string> mediators)
        {
            var taglessFlavors = flavors.Where(f => f.tags == null || !f.tags.Any());
            foreach (var flavor in taglessFlavors)
            {
                if (flavor.mediators.OrderBy(m => m).SequenceEqual(mediators.OrderBy(m => m)))
                {
                    return flavor.versions;
                }
            }

            return null;
        }

        private ManifestDto<TDependencyDto> GetManifestForTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                return null;
            }
            return flavors.FirstOrDefault(f => f.tags?.Contains(tag) ?? false)?.versions;
        }
    }
}