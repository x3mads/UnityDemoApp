using XMediator.Editor.Tools.DependencyManager.Entities;

namespace XMediator.Editor.Tools.DependencyManager.View
{
    internal interface DependencyManagerView
    {
        void SetViewState(DependencyManagerViewState state);
        void DisplayDialog(string title, string message);
        int DisplayDialogOptions(string dialogTitle, string message, string ok, string cancel);
        void SaveJsonFile(string viewTitle, string fileName, string json);
    }
}