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

        [MenuItem("Tech-Cosmos/Serialization/导出管理器")]
        public static void ShowWindow()
        {
            GetWindow<ExportManagerWindow>("导出管理器");
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
            GUILayout.Label("导出配置管理器", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("刷新", GUILayout.Width(60)))
            {
                RefreshConfigs();
            }
            if (GUILayout.Button("创建新配置", GUILayout.Width(100)))
            {
                ExportQuickMenu.CreateNewConfig();
                RefreshConfigs();
            }
            if (GUILayout.Button("快速导出", GUILayout.Width(80)))
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
            EditorGUILayout.LabelField("菜单路径:", config.menuPath, GUILayout.Width(300));
            EditorGUILayout.LabelField("输出文件:", config.jsonFileName, GUILayout.Width(200));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("导出", GUILayout.Width(60)))
            {
                ExportQuickMenu.ExecuteExport(config);
            }
            if (GUILayout.Button("选择", GUILayout.Width(60)))
            {
                Selection.activeObject = config;
                EditorGUIUtility.PingObject(config);
            }
            if (GUILayout.Button("删除", GUILayout.Width(60)))
            {
                if (EditorUtility.DisplayDialog("确认删除", $"确定要删除配置 {config.menuPath} 吗？", "删除", "取消"))
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