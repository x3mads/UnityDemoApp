using System.Collections.Generic;

namespace XMediator.Editor.Tools.MetaMediation.Entities
{
    internal class MetaMediationViewState
    {
        internal bool IsDownloading { get; }
        internal Dependencies Dependencies {get;}
        internal HashSet<string> SavedNetworks { get; } 
        internal HashSet<string> SavedMediations { get; } 
        internal HashSet<string> SavedPlatforms { get; } 

        internal MetaMediationViewState(Dependencies dependencies, bool isDownloading = false, HashSet<string> savedMediations = null, HashSet<string> savedNetworks = null, HashSet<string> savedPlatforms = null)
        {
            Dependencies = dependencies;
            IsDownloading = isDownloading;
            SavedNetworks = savedNetworks;
            SavedMediations = savedMediations;
            SavedPlatforms = savedPlatforms;
        }
    }
}