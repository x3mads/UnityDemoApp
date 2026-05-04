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
        
        internal ManifestDto<TDependencyDto> GetManifestForMediators(List<string> mediators, List<string> tools = null, string tag = null)
        {
            tools ??= new List<string>();
            var taggedFlavors = FindFlavorsForTag(tag);
            
            FlavorDto<TDependencyDto> flavor;
            if (taggedFlavors != null)
            {
                // Tag was provided and flavors were found
                flavor = GetExactFlavorMatch(taggedFlavors, mediators, tools) ??
                         GetBestFlavorMatch(taggedFlavors, mediators, tools) ??
                         taggedFlavors[0]; // First tagged flavor as fallback
            }
            else
            {
                // No tag provided or no flavors matched the tag
                var taglessFlavors = FindTaglessFlavors();
                flavor = GetExactFlavorMatch(taglessFlavors, mediators, tools) ??
                         GetBestFlavorMatch(taglessFlavors, mediators, tools);
            }

            return flavor?.versions ?? GetDefaultManifest();
        }
        
        internal List<string> GetAllTagsInFlavors() => flavors.SelectMany(f => f.tags).ToList();

        private List<FlavorDto<TDependencyDto>> FindFlavorsForTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                return null;
            }
            
            var flavorsWithTag = flavors.Where(f => f.tags?.Contains(tag) ?? false).ToList();
            return flavorsWithTag.Count > 0 ? flavorsWithTag : null;
        }

        private List<FlavorDto<TDependencyDto>> FindTaglessFlavors()
        {
            return flavors.Where(f => f.tags == null || !f.tags.Any()).ToList();
        }

        private List<FlavorDto<TDependencyDto>> FilterFlavorsByTools(List<FlavorDto<TDependencyDto>> flavorList, List<string> selectedTools)
        {
            // If no tools are selected, return all flavors
            if (selectedTools.Count == 0)
            {
                return flavorList;
            }
            
            // Filter by matching tools
            var matchingFlavors = flavorList.Where(flavor =>
            {
                var flavorTools = flavor.additional_tools ?? new List<string>();
                return selectedTools.All(tool => flavorTools.Contains(tool));
            }).ToList();
            
            // If tools are selected but no flavors match, return all flavors
            if (matchingFlavors.Count == 0)
            {
                return flavorList;
            }
            
            return matchingFlavors;
        }

        private FlavorDto<TDependencyDto> GetExactFlavorMatch(List<FlavorDto<TDependencyDto>> flavorList, List<string> selectedMediators, List<string> selectedTools)
        {
            var filteredFlavors = FilterFlavorsByTools(flavorList, selectedTools);
            return filteredFlavors.FirstOrDefault(flavor =>
                flavor.mediators.Count == selectedMediators.Count &&
                flavor.mediators.All(mediator => selectedMediators.Contains(mediator))
            );
        }

        private FlavorDto<TDependencyDto> GetBestFlavorMatch(List<FlavorDto<TDependencyDto>> flavorList, List<string> selectedMediators, List<string> selectedTools)
        {
            var filteredFlavors = FilterFlavorsByTools(flavorList, selectedTools);
            return filteredFlavors.FirstOrDefault(flavor =>
                selectedMediators.All(mediator => flavor.mediators.Contains(mediator))
            );
        }
    }
}