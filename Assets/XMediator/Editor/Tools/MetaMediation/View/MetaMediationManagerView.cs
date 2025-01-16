using XMediator.Editor.Tools.MetaMediation.Entities;

namespace XMediator.Editor.Tools.MetaMediation.View
{
    internal interface MetaMediationManagerView
    {
        void SetViewState(MetaMediationViewState state);
        void OnGeneratedDependencies(bool isError, string message = null);
        void OnDialogWithCloseWindow(string dialogTitle,  string message);
        bool OnConfirmationDialog(string dialogTitle,  string message);
        void OnRefreshUi();
    }
}