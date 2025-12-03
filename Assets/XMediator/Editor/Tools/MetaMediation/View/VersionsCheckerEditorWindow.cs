using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMediator.Editor.Tools.MetaMediation.View
{
    internal class VersionsCheckerEditorWindow : EditorWindow
    {
        private VersionsCheckerPresenter _presenter;
        private Vector2 _scrollPosition;
        private string _searchTerm = "";

        [MenuItem("XMediator/Versions Checker", false, 2)]
        public static void ShowWindow()
        {
            GetWindow<VersionsCheckerEditorWindow>("Versions Checker");
        }

        private void OnEnable()
        {
            _presenter = new VersionsCheckerPresenter(Repaint);
        }

        private void OnGUI()
        {
            if (_presenter == null || !_presenter.IsReady)
            {
                EditorGUILayout.LabelField("Loading dependencies...");
                return;
            }

            _searchTerm = EditorGUILayout.TextField("Search", _searchTerm);
            EditorGUILayout.Space();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            EditorGUILayout.LabelField("Mediators", EditorStyles.boldLabel);
            DrawHeader();
            RenderEntries(_presenter.BuildEntries(_presenter.Mediators, _searchTerm));

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Ad Networks", EditorStyles.boldLabel);
            DrawHeader();
            RenderEntries(_presenter.BuildEntries(_presenter.Networks, _searchTerm));

            // Only show Tools section if there are tools available
            if (_presenter.Tools.Any())
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Additional Tools", EditorStyles.boldLabel);
                DrawHeader();
                RenderEntries(_presenter.BuildEntries(_presenter.Tools, _searchTerm));
            }

            EditorGUILayout.EndScrollView();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Export", GUILayout.Width(80)))
            {
                ExportVersions();
            }
            GUILayout.EndHorizontal();
        }

        private void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name", EditorStyles.miniBoldLabel, GUILayout.Width(200));
            EditorGUILayout.LabelField("Android", EditorStyles.miniBoldLabel, GUILayout.Width(200));
            EditorGUILayout.LabelField("iOS", EditorStyles.miniBoldLabel, GUILayout.Width(150));
            EditorGUILayout.EndHorizontal();
        }

        private void RenderEntries(IEnumerable<(string name, string android, string ios)> entries)
        {
            foreach (var (name, android, ios) in entries)
            {
                EditorGUILayout.BeginHorizontal("box");
                EditorGUILayout.LabelField(name, GUILayout.Width(200));
                EditorGUILayout.LabelField(android, GUILayout.Width(200));
                EditorGUILayout.LabelField(ios, GUILayout.Width(150));
                EditorGUILayout.EndHorizontal();
            }
        }

        private void ExportVersions()
        {
            var mediatorEntries = _presenter.BuildEntries(_presenter.Mediators, _searchTerm).ToList();
            var networkEntries = _presenter.BuildEntries(_presenter.Networks, _searchTerm).ToList();
            var toolEntries = _presenter.BuildEntries(_presenter.Tools, _searchTerm).ToList();

            var sb = new StringBuilder();
            sb.AppendLine("Mediator Versions:\n");
            foreach (var (name, android, ios) in mediatorEntries)
            {
                sb.AppendLine($"{name} | Android: {android} | iOS: {ios}");
            }
            sb.AppendLine("\nAd Network Versions:\n");
            foreach (var (name, android, ios) in networkEntries)
            {
                sb.AppendLine($"{name} | Android: {android} | iOS: {ios}");
            }
            if (toolEntries.Any())
            {
                sb.AppendLine("\nAdditional Tools:\n");
                foreach (var (name, android, ios) in toolEntries)
                {
                    sb.AppendLine($"{name} | Android: {android} | iOS: {ios}");
                }
            }

            var path = EditorUtility.SaveFilePanel("Export Versions", Application.dataPath, "XMediatorVersions", "txt");
            if (!string.IsNullOrEmpty(path))
            {
                System.IO.File.WriteAllText(path, sb.ToString());
                EditorUtility.RevealInFinder(path);
            }
        }
    }
}