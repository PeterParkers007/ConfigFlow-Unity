using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZJM_JsonTool.Runtime;
public class Exmple : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // ��ȡӢ������
        var hero = ConfigManager<HeroTemplate, HeroTemplateCollection>.GetTemplate("սʿ", "HeroTemplates.json");

        // ��ȡ��������  
        var ability = ConfigManager<AbilityTemplate, AbilityTemplateCollection>.GetTemplate("������", "AbilityTemplates.json");

        // ֱ����
        Debug.Log(hero.heroId);
        Debug.Log(ability.abilityId);
    }
}
