using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZJM_JsonTool.Runtime;
public class Exmple : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // 读取英雄数据
        var hero = ConfigManager<HeroTemplate, HeroTemplateCollection>.GetTemplate("战士", "HeroTemplates.json");

        // 读取技能数据  
        var ability = ConfigManager<AbilityTemplate, AbilityTemplateCollection>.GetTemplate("寒冰箭", "AbilityTemplates.json");

        // 直接用
        Debug.Log(hero.heroId);
        Debug.Log(ability.abilityId);
    }
}
