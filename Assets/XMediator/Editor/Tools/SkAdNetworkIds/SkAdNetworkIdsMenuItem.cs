using UnityEditor;

namespace XMediator.Editor.Tools.SkAdNetworkIds
{
    public class SkAdNetworkIdsMenuItem : UnityEditor.Editor
    {
        [MenuItem("XMediator/Update SkAdNetwork Ids", false, 1)]
        public static void UpdateSkAdNetworkIds()
        {
            SkAdNetworkIdsServiceFactory.CreateInstance().SynchronizeSkAdNetworkIds(
                list => DisplayDialog("Update was successful!"),
                exception => DisplayDialog("Failed to update"));
        }
        
        private static void DisplayDialog(string message)
        {
            EditorUtility.DisplayDialogComplex(
                title: "SkAdNetwork Ids",
                message: message,
                ok: "Close", "", "");
        }
    }
}