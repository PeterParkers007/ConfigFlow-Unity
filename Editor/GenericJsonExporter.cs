// Editor/GenericJsonExporter.cs
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;
using System.Reflection;
using ZJM_JsonTool.Runtime;

namespace ZJM_JsonTool.Editor
{
    public static class GenericJsonExporter
    {
        // 原有的强类型方法
        public static void ExportConfigsToJson<TConfig, TTemplate, TCollection>(
            string jsonFileName, string assetSearchFilter)
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
            collection.targets = templates.ToArray();

            // 3. 序列化并保存文件
            string jsonString = JsonUtility.ToJson(collection, true);
            Debug.Log($"序列化后的JSON内容: {(string.IsNullOrEmpty(jsonString) ? "空字符串" : jsonString)}");

            string filePath = Path.Combine(Application.streamingAssetsPath, "Config", jsonFileName);

            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            // 在 File.WriteAllText 之前添加这些调试行：
            Debug.Log($"准备序列化的集合类型: {collection.GetType()}");
            Debug.Log($"集合中的数据数量: {collection.targets.Length}");
            Debug.Log($"集合中第一个元素: {JsonUtility.ToJson(collection.targets[0])}");


            File.WriteAllText(filePath, jsonString);
            AssetDatabase.Refresh();

            Debug.Log($"✅ 导出完成: {filePath}");
        }

        private static TTemplate ConvertConfigToTemplate<TConfig, TTemplate>(TConfig config)
        {
            TTemplate template = System.Activator.CreateInstance<TTemplate>();
            System.Type configType = typeof(TConfig);
            System.Type templateType = typeof(TTemplate);

            Debug.Log($"=== 开始转换 {configType.Name} -> {templateType.Name} ===");

            var configFields = configType.GetFields();
            var templateFields = templateType.GetFields();

            // 打印所有字段信息
            Debug.Log("Config字段列表:");
            foreach (var field in configFields)
            {
                object value = field.GetValue(config);
                Debug.Log($"  {field.Name} ({field.FieldType}) = {value}");
            }

            Debug.Log("Template字段列表:");
            foreach (var field in templateFields)
            {
                Debug.Log($"  {field.Name} ({field.FieldType})");
            }

            // 字段复制过程
            int copiedCount = 0;
            foreach (var configField in configFields)
            {
                var templateField = templateType.GetField(configField.Name);

                if (templateField != null)
                {
                    if (templateField.FieldType == configField.FieldType)
                    {
                        object value = configField.GetValue(config);
                        templateField.SetValue(template, value);
                        Debug.Log($"✅ 成功复制: {configField.Name} = {value}");
                        copiedCount++;
                    }
                    else
                    {
                        Debug.LogWarning($"❌ 类型不匹配: {configField.Name} ({configField.FieldType} -> {templateField.FieldType})");
                    }
                }
                else
                {
                    Debug.LogWarning($"❌ 在Template中找不到字段: {configField.Name}");
                }
            }

            Debug.Log($"=== 转换完成: 复制了 {copiedCount} 个字段 ===");
            return template;
        }

        // 新增：通过类型名调用的方法
        public static void ExportConfigsToJsonByNames(
            string configTypeName,
            string templateTypeName,
            string collectionTypeName,
            string jsonFileName,
            string assetSearchFilter)
        {
            // 通过反射解析类型
            System.Type configType = FindTypeInProject(configTypeName);
            System.Type templateType = FindTypeInProject(templateTypeName);
            System.Type collectionType = FindTypeInProject(collectionTypeName);

            if (configType == null || templateType == null || collectionType == null)
            {
                Debug.LogError("找不到指定的类型，请检查类型名称！");
                return;
            }

            // 使用反射调用泛型方法
            MethodInfo genericMethod = typeof(GenericJsonExporter)
                .GetMethod("ExportConfigsToJson")
                .MakeGenericMethod(configType, templateType, collectionType);

            genericMethod.Invoke(null, new object[] { jsonFileName, assetSearchFilter });
        }

        private static System.Type FindTypeInProject(string typeName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                // 只在用户程序集中查找（排除系统、Unity、包程序集）
                if (!assembly.FullName.StartsWith("System") &&
                    !assembly.FullName.StartsWith("Unity") &&
                    !assembly.FullName.Contains("ZJM_JsonTool"))
                {
                    System.Type type = assembly.GetType(typeName);
                    if (type != null) return type;
                }
            }
            return null;
        }
    }
}