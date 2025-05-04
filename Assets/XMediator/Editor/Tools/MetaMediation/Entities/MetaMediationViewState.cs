namespace XMediator.Editor.Tools.MetaMediation.Entities
{
    internal class MetaMediationViewState
    {
        internal bool IsDownloading { get; }
        internal SelectableDependencies SelectableDependencies {get;}

        internal MetaMediationViewState(SelectableDependencies selectableDependencies, bool isDownloading = false)
        {
            SelectableDependencies = selectableDependencies;
            IsDownloading = isDownloading;
        }
    }
}