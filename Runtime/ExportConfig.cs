// Runtime/ExportConfig.cs
using UnityEngine;

namespace ZJM_JsonTool.Runtime
{
    [CreateAssetMenu(fileName = "New Export Config", menuName = "ZJM/Export Config")]
    public class ExportConfig : ScriptableObject
    {
        [Header("导出设置")]
        public string menuPath = "ZJM_JsonTool/导出/新配置";
        public string jsonFileName = "NewTemplates.json";

        [Header("数据类型配置")]
        public string configTypeName;    // 如 "HeroConfig"
        public string templateTypeName;  // 如 "HeroTemplate"  
        public string collectionTypeName; // 如 "HeroTemplateCollection"

        [Header("资产搜索过滤")]
        public string assetFilter = "t:xxxConfig";
    }
}