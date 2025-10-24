using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Melee Tower Stats", menuName = "KR/Create Melee Tower Stats")]
public class BarrackTowerStats : TowerStats
{
    public int hp;
    public float attackSpeed;
    public int armor;
    public string descriptionArmor;
    public int respawnTime;
    public Sprite barrackTowerUpgradeSprite;
    public RuntimeAnimatorController militiaUpgradeAnim;
    public Militia militiaPrefab;
    public Sprite iconMilitia;
}
