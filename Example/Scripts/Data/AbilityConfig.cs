using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZJM_JsonTool.Runtime;
[CreateAssetMenu(fileName = "New Ability Config", menuName = "Data/Ability Config")]
public class AbilityConfig : ScriptableObject,IConfig<AbilityTemplate>
{
    public string abilityId;
    public float coolDown;
    public float abilityPower;
}
