// Runtime/ExportConfig.cs
using UnityEngine;

namespace TechCosmos.Serialization.Runtime
{
    [CreateAssetMenu(fileName = "New Export Config", menuName = "Tech-Cosmos/Config/Export Config")]
    public class ExportConfig : ScriptableObject
    {
        [Header("��������")]
        public string menuPath = "Serialization/����/������";
        public string jsonFileName = "New ExportTemplates.json";

        [Header("������������")]
        public string configTypeName;    // �� "HeroConfig"
        public string templateTypeName;  // �� "HeroTemplate"  
        public string collectionTypeName; // �� "HeroTemplateCollection"

        [Header("�ʲ���������")]
        public string assetFilter = "t:xxxConfig";
    }
}