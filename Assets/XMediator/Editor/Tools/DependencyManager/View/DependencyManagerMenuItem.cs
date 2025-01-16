using UnityEditor;

namespace XMediator.Editor.Tools.DependencyManager.View
{
    public class DependencyManagerMenuItem : UnityEditor.Editor
    {
        [MenuItem("XMediator/X3M Dependency Manager", false, 0)]
        public static void DependencyManager()
        {
            EditorWindow.GetWindow(typeof(DependencyManagerEditorWindow), true, "XMediator Dependency Manager").Show();
        }
    }
}