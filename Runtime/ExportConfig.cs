// Runtime/ExportConfig.cs
using UnityEngine;

namespace ZJM_JsonTool.Runtime
{
    [CreateAssetMenu(fileName = "New Export Config", menuName = "ZJM/Export Config")]
    public class ExportConfig : ScriptableObject
    {
        [Header("��������")]
        public string menuPath = "ZJM_JsonTool/����/������";
        public string jsonFileName = "NewTemplates.json";

        [Header("������������")]
        public string configTypeName;    // �� "HeroConfig"
        public string templateTypeName;  // �� "HeroTemplate"  
        public string collectionTypeName; // �� "HeroTemplateCollection"

        [Header("�ʲ���������")]
        public string assetFilter = "t:xxxConfig";
    }
}