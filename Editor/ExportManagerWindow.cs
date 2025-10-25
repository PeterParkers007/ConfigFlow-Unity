// Editor/ExportManagerWindow.cs
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using ZJM_JsonTool.Runtime;

namespace ZJM_JsonTool.Editor
{
    public class ExportManagerWindow : EditorWindow
    {
        private List<ExportConfig> _configs;
        private Vector2 _scrollPosition;

        [MenuItem("Tech-Cosmos/Serialization/����������")]
        public static void ShowWindow()
        {
            GetWindow<ExportManagerWindow>("����������");
        }

        private void OnEnable()
        {
            RefreshConfigs();
        }

        private void RefreshConfigs()
        {
            _configs = new List<ExportConfig>();
            string[] guids = AssetDatabase.FindAssets("t:ExportConfig");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                ExportConfig config = AssetDatabase.LoadAssetAtPath<ExportConfig>(path);
                if (config != null) _configs.Add(config);
            }
        }

        private void OnGUI()
        {
            GUILayout.Label("�������ù�����", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("ˢ��", GUILayout.Width(60)))
            {
                RefreshConfigs();
            }
            if (GUILayout.Button("����������", GUILayout.Width(100)))
            {
                ExportQuickMenu.CreateNewConfig();
                RefreshConfigs();
            }
            if (GUILayout.Button("���ٵ���", GUILayout.Width(80)))
            {
                ExportQuickMenu.ShowQuickExportMenu();
            }
            EditorGUILayout.EndHorizontal();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            foreach (var config in _configs)
            {
                DrawConfigItem(config);
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawConfigItem(ExportConfig config)
        {
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("�˵�·��:", config.menuPath, GUILayout.Width(300));
            EditorGUILayout.LabelField("����ļ�:", config.jsonFileName, GUILayout.Width(200));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("����", GUILayout.Width(60)))
            {
                ExportQuickMenu.ExecuteExport(config);
            }
            if (GUILayout.Button("ѡ��", GUILayout.Width(60)))
            {
                Selection.activeObject = config;
                EditorGUIUtility.PingObject(config);
            }
            if (GUILayout.Button("ɾ��", GUILayout.Width(60)))
            {
                if (EditorUtility.DisplayDialog("ȷ��ɾ��", $"ȷ��Ҫɾ������ {config.menuPath} ��", "ɾ��", "ȡ��"))
                {
                    string path = AssetDatabase.GetAssetPath(config);
                    AssetDatabase.DeleteAsset(path);
                    RefreshConfigs();
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }
    }
}