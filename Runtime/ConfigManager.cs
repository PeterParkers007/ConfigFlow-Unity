using UnityEngine;
using System.IO;
using System.Collections.Generic;
using ZJM_JsonTool.Runtime.Interfaces;
namespace ZJM_JsonTool.Runtime
{
    public static class ConfigManager<U, T> where T : class, IJsonDataCollection<U>, new()
    {
        private static Dictionary<string, U> _templateCache;

        // 静态构造函数必须在这里！
        static ConfigManager()
        {
            Application.quitting += ClearCache;
        }

        public static U GetTemplate(string dataId, string jsonName)
        {
            if (_templateCache == null || _templateCache.Count == 0)
            {
                LoadTemplate(jsonName);
            }

            if (_templateCache.TryGetValue(dataId, out U template))
            {
                return template;
            }
            else
            {
                Debug.LogWarning($"Template with ID '{dataId}' not found in {jsonName}");
                return default(U);
            }
        }

        private static void LoadTemplate(string jsonName)
        {
            _templateCache = new Dictionary<string, U>();
            string filePath = Path.Combine(Application.streamingAssetsPath, "Config", jsonName);

            if (File.Exists(filePath))
            {
                string jsonContent = File.ReadAllText(filePath);
                T collection = JsonUtility.FromJson<T>(jsonContent);

                // 添加空值检查
                if (collection?.targets == null)
                {
                    Debug.LogError($"Invalid JSON data in {filePath}");
                    return;
                }

                // 在 LoadTemplate 方法中，修复字段名匹配问题
                foreach (var target in collection.targets)
                {
                    // 尝试多种可能的ID字段名
                    var idField = typeof(U).GetField("id") ??
                                  typeof(U).GetField("heroId") ??
                                  typeof(U).GetField("abilityId");

                    if (idField != null)
                    {
                        string id = idField.GetValue(target) as string;
                        if (!string.IsNullOrEmpty(id))
                        {
                            _templateCache[id] = target;
                        }
                    }
                }

                Debug.Log($"Successfully loaded {_templateCache.Count} items from {jsonName}");
            }
            else
            {
                Debug.LogError($"JSON file not found: {filePath}");
            }
        }

        public static IEnumerable<U> GetAllTemplates(string jsonName)
        {
            if (_templateCache == null || _templateCache.Count == 0)
            {
                LoadTemplate(jsonName);
            }
            return _templateCache.Values;
        }

        public static void Reload(string jsonName)
        {
            ClearCache();
            LoadTemplate(jsonName);
        }

        public static void ClearCache()
        {
            _templateCache?.Clear();
            _templateCache = null;
        }
    }
}
