// 在 Editor/GenericJsonExporter.cs
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using ZJM_JsonTool.Runtime;
namespace ZJM_JsonTool.Editor
{
    public static class GenericJsonExporter
    {
        // 编辑期专用的泛型导出方法
        // 在ExportConfigsToJson方法中添加调试信息
        public static void ExportConfigsToJson<TConfig, TTemplate, TCollection>(
            string jsonFileName,
            string assetSearchFilter)
            where TConfig : ScriptableObject
            where TCollection : IJsonDataCollection<TTemplate>, new()
        {
            // 1. 查找配置资产
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(TConfig).Name}");
            List<TTemplate> templates = new List<TTemplate>();

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                TConfig config = AssetDatabase.LoadAssetAtPath<TConfig>(path);

                if (config != null)
                {
                    TTemplate template = ConvertConfigToTemplate<TConfig, TTemplate>(config);
                    if (template != null)
                    {
                        templates.Add(template);
                    }
                }
            }

            Debug.Log($"找到 {templates.Count} 个配置资产");

            // 2. 创建集合并调试
            TCollection collection = new TCollection();
            Debug.Log($"集合类型: {collection.GetType().Name}");

            // 检查接口实现
            var interfaceType = typeof(IJsonDataCollection<TTemplate>);
            bool implementsInterface = interfaceType.IsAssignableFrom(collection.GetType());
            Debug.Log($"是否实现接口: {implementsInterface}");

            // 设置targets
            collection.targets = templates.ToArray();

            // 检查设置后的值
            var targetsAfterSet = collection.targets;
            Debug.Log($"设置后targets数量: {(targetsAfterSet != null ? targetsAfterSet.Length : 0)}");

            // 3. 序列化前检查
            string jsonString = JsonUtility.ToJson(collection, true);
            Debug.Log($"生成的JSON: {jsonString}");

            // 4. 保存文件
            string filePath = Path.Combine(Application.streamingAssetsPath, "Config", jsonFileName);

            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            File.WriteAllText(filePath, jsonString);
            AssetDatabase.Refresh();

            Debug.Log($"✅ 导出完成: {filePath}");
        }

        // 编辑期反射工具方法：将ScriptableObject转换为数据模板
        private static TTemplate ConvertConfigToTemplate<TConfig, TTemplate>(TConfig config)
        {
            // 使用反射动态复制字段
            TTemplate template = System.Activator.CreateInstance<TTemplate>();
            System.Type configType = typeof(TConfig);
            System.Type templateType = typeof(TTemplate);

            var configFields = configType.GetFields();
            foreach (var configField in configFields)
            {
                var templateField = templateType.GetField(configField.Name);
                if (templateField != null && templateField.FieldType == configField.FieldType)
                {
                    object value = configField.GetValue(config);
                    templateField.SetValue(template, value);
                }
            }

            return template;
        }

        // 菜单项 - 这些只在编辑期显示
        [MenuItem("Proteus/导出/英雄配置")]
        public static void ExportHeroConfigs()
        {
            //ExportConfigsToJson<HeroConfig, HeroTemplate, HeroTemplateCollection>(
            //    "HeroTemplates.json",
            //    "t:HeroConfig"
            //);
        }

        [MenuItem("Proteus/导出/技能配置")]
        public static void ExportAbilityConfigs()
        {
            //未来添加技能系统时使用
            //ExportConfigsToJson<AbilityConfig, AbilityTemplate, AbilityTemplateCollection>(
            //    "AbilityTemplates.json",
            //    "t:AbilityConfig"
            //);
        }

        [MenuItem("Proteus/导出/所有配置")]
        public static void ExportAllConfigs()
        {
            ExportHeroConfigs();
            ExportAbilityConfigs();
            // ExportItemConfigs();
            // ... 其他配置
        }
    }
}
