using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZJM_JsonTool.Runtime;
[CreateAssetMenu(fileName = "New Hero Config", menuName = "Data/Hero Config")]
public class HeroConfig : ScriptableObject,IConfig<HeroTemplate>
{
    public string heroId;
    public float moveSpeed;
    public float baseHealth;
}
