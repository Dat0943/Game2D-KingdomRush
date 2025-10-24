using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerStats : ScriptableObject
{
    public string nameTower;
    public int level;
    public int buildCost;
    public int upgradeBuildCost;

    public int minPhysicalDamage;
    public int maxPhysicalDamage;

    public int minMagicalDamage;
    public int maxMagicalDamage;

    public int PhysicalDamage { get => Random.Range(minPhysicalDamage, maxPhysicalDamage + 1); }
    public int MagicalDamage { get => Random.Range(minMagicalDamage, maxMagicalDamage + 1); }
}
