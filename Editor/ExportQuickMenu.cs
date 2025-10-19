// Editor/ExportQuickMenu.cs
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using ZJM_JsonTool.Runtime;

namespace ZJM_JsonTool.Editor
{
    public static class ExportQuickMenu
    {
        private static List<ExportConfig> _cachedConfigs;

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            // ��Unity����ʱ�Զ���������
            RefreshConfigCache();

            // �����ʲ��仯���Զ����»���
            EditorApplication.projectChanged += RefreshConfigCache;
        }

        // ���ķ��������ٵ����˵�
        [MenuItem("ZJM/���ٵ��� _%#e")] // Ctrl+Shift+E ��ݼ�
        public static void ShowQuickExportMenu()
        {
            RefreshConfigCache(); // ȷ��ʹ����������

            if (_cachedConfigs.Count == 0)
            {
                EditorUtility.DisplayDialog("��ʾ", "û���ҵ��κε������ã�\n���ȴ��� ExportConfig �ʲ���", "ȷ��");
                return;
            }

            GenericMenu menu = new GenericMenu();

            // ��̬�������������˵�
            for (int i = 0; i < _cachedConfigs.Count; i++)
            {
                var config = _cachedConfigs[i];

                // Ϊÿ�����ô����˵���
                menu.AddItem(
                    new GUIContent(config.menuPath),
                    false,
                    () => ExecuteExport(config) // ���ʱִ�е���
                );
            }

            // ��ӷָ���
            menu.AddSeparator("");

            // ��ӹ���ѡ��
            menu.AddItem(new GUIContent("�򿪵���������"), false, ShowExportManager);
            menu.AddItem(new GUIContent("����������"), false, CreateNewConfig);
            menu.AddItem(new GUIContent("ˢ�����û���"), false, RefreshConfigCache);

            // ��ʾ�˵�
            menu.ShowAsContext();
        }

        // ִ�е���
        public static void ExecuteExport(ExportConfig config)
        {
            if (config == null)
            {
                Debug.LogError("����Ϊ�գ�");
                return;
            }

            if (string.IsNullOrEmpty(config.configTypeName) ||
                string.IsNullOrEmpty(config.templateTypeName) ||
                string.IsNullOrEmpty(config.collectionTypeName))
            {
                Debug.LogError($"���� {config.menuPath} ���������ò�������");
                return;
            }

            Debug.Log($"��ʼ����: {config.menuPath}");

            try
            {
                GenericJsonExporter.ExportConfigsToJsonByNames(
                    config.configTypeName,
                    config.templateTypeName,
                    config.collectionTypeName,
                    config.jsonFileName,
                    config.assetFilter
                );
            }
            catch (System.Exception e)
            {
                Debug.LogError($"����ʧ��: {e.Message}");
                EditorUtility.DisplayDialog("��������", $"����ʧ��: {e.Message}", "ȷ��");
            }
        }

        // ˢ�����û���
        private static void RefreshConfigCache()
        {
            _cachedConfigs = new List<ExportConfig>();

            string[] guids = AssetDatabase.FindAssets("t:ExportConfig");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                ExportConfig config = AssetDatabase.LoadAssetAtPath<ExportConfig>(path);
                if (config != null && !string.IsNullOrEmpty(config.menuPath))
                {
                    _cachedConfigs.Add(config);
                }
            }

            Debug.Log($"�Ѽ��� {_cachedConfigs.Count} ����������");
        }

        // �򿪹���������
        private static void ShowExportManager()
        {
            ExportManagerWindow.ShowWindow();
        }

        // ����������
        public static void CreateNewConfig()
        {
            // �����µ�ExportConfig�ʲ�
            var config = ScriptableObject.CreateInstance<ExportConfig>();
            string path = "Assets/NewExportConfig.asset";
            path = AssetDatabase.GenerateUniqueAssetPath(path);

            AssetDatabase.CreateAsset(config, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // ѡ���´���������
            Selection.activeObject = config;
            EditorUtility.FocusProjectWindow();
        }
    }
}