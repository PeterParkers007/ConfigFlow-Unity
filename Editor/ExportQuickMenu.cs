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
            // 在Unity启动时自动加载配置
            RefreshConfigCache();

            // 监听资产变化，自动更新缓存
            EditorApplication.projectChanged += RefreshConfigCache;
        }

        // 核心方法：快速导出菜单
        [MenuItem("ZJM/快速导出 _%#e")] // Ctrl+Shift+E 快捷键
        public static void ShowQuickExportMenu()
        {
            RefreshConfigCache(); // 确保使用最新配置

            if (_cachedConfigs.Count == 0)
            {
                EditorUtility.DisplayDialog("提示", "没有找到任何导出配置！\n请先创建 ExportConfig 资产。", "确定");
                return;
            }

            GenericMenu menu = new GenericMenu();

            // 动态添加所有配置项到菜单
            for (int i = 0; i < _cachedConfigs.Count; i++)
            {
                var config = _cachedConfigs[i];

                // 为每个配置创建菜单项
                menu.AddItem(
                    new GUIContent(config.menuPath),
                    false,
                    () => ExecuteExport(config) // 点击时执行导出
                );
            }

            // 添加分隔线
            menu.AddSeparator("");

            // 添加管理选项
            menu.AddItem(new GUIContent("打开导出管理器"), false, ShowExportManager);
            menu.AddItem(new GUIContent("创建新配置"), false, CreateNewConfig);
            menu.AddItem(new GUIContent("刷新配置缓存"), false, RefreshConfigCache);

            // 显示菜单
            menu.ShowAsContext();
        }

        // 执行导出
        public static void ExecuteExport(ExportConfig config)
        {
            if (config == null)
            {
                Debug.LogError("配置为空！");
                return;
            }

            if (string.IsNullOrEmpty(config.configTypeName) ||
                string.IsNullOrEmpty(config.templateTypeName) ||
                string.IsNullOrEmpty(config.collectionTypeName))
            {
                Debug.LogError($"配置 {config.menuPath} 的类型设置不完整！");
                return;
            }

            Debug.Log($"开始导出: {config.menuPath}");

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
                Debug.LogError($"导出失败: {e.Message}");
                EditorUtility.DisplayDialog("导出错误", $"导出失败: {e.Message}", "确定");
            }
        }

        // 刷新配置缓存
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

            Debug.Log($"已加载 {_cachedConfigs.Count} 个导出配置");
        }

        // 打开管理器窗口
        private static void ShowExportManager()
        {
            ExportManagerWindow.ShowWindow();
        }

        // 创建新配置
        public static void CreateNewConfig()
        {
            // 创建新的ExportConfig资产
            var config = ScriptableObject.CreateInstance<ExportConfig>();
            string path = "Assets/NewExportConfig.asset";
            path = AssetDatabase.GenerateUniqueAssetPath(path);

            AssetDatabase.CreateAsset(config, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // 选中新创建的配置
            Selection.activeObject = config;
            EditorUtility.FocusProjectWindow();
        }
    }
}