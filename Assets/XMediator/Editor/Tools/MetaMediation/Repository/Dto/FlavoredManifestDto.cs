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

            var flavor = FindFlavorForTag(tag) ??
                         GetExactFlavorMatch(mediators, tools) ??
                         GetBestFlavorMatch(mediators, tools);
            return flavor?.versions ?? GetDefaultManifest();
        }
        
        internal List<string> GetAllTagsInFlavors() => flavors.SelectMany(f => f.tags).ToList();

        private FlavorDto<TDependencyDto> FindFlavorForTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                return null;
            }
            return flavors.FirstOrDefault(f => f.tags?.Contains(tag) ?? false);
        }

        private List<FlavorDto<TDependencyDto>> FilterFlavorsByToolsAndTags(List<string> selectedTools)
        {
            // First, filter tagless flavors
            var taglessFlavors = flavors.Where(f => f.tags == null || !f.tags.Any()).ToList();
            
            // If no tools are selected, return all tagless flavors
            if (selectedTools.Count == 0)
            {
                return taglessFlavors;
            }
            
            // Filter tagless flavors by matching tools
            var matchingFlavors = taglessFlavors.Where(flavor =>
            {
                var flavorTools = flavor.additional_tools ?? new List<string>();
                return selectedTools.All(tool => flavorTools.Contains(tool));
            }).ToList();
            
            // If tools are selected but no flavors match, return the tagless flavors
            if (matchingFlavors.Count == 0)
            {
                return taglessFlavors;
            }
            
            return matchingFlavors;
        }

        private FlavorDto<TDependencyDto> GetExactFlavorMatch(List<string> selectedMediators, List<string> selectedTools)
        {
            var filteredFlavors = FilterFlavorsByToolsAndTags(selectedTools);
            return filteredFlavors.FirstOrDefault(flavor =>
                flavor.mediators.Count == selectedMediators.Count &&
                flavor.mediators.All(mediator => selectedMediators.Contains(mediator))
            );
        }

        private FlavorDto<TDependencyDto> GetBestFlavorMatch(List<string> selectedMediators, List<string> selectedTools)
        {
            var filteredFlavors = FilterFlavorsByToolsAndTags(selectedTools);
            return filteredFlavors.FirstOrDefault(flavor =>
                selectedMediators.All(mediator => flavor.mediators.Contains(mediator))
            );
        }
    }
}