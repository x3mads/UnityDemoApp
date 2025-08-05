namespace XMediator.Editor.Tools.Settings
{
    internal interface XMediatorDependenciesRepository
    {
        void UpdateXMediatorDependenciesFile();
        
        public string GetVersionValue();
    }
}