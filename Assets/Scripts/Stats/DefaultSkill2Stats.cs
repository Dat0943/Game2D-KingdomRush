using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Default Skill 2 Stats", menuName = "KR/Create Default Skill 2 Stats")]
public class DefaultSkill2Stats : ScriptableObject
{
    public int level;
    public int hp;
    public float attackSpeed;
    public int armor;
    public string descriptionArmor;
    public int minPhysicalDamage;
    public int maxPhysicalDamage;
    public int PhysicalDamage { get => Random.Range(minPhysicalDamage, maxPhysicalDamage + 1); }
    public float coolDown;
}
