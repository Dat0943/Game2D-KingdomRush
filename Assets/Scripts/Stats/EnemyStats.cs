using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Stats", menuName = "KR/Create Enemy Stats")]
public class EnemyStats : ScriptableObject
{
    public string nameEnemy;
    public int health;
    public int minDamage;
    public int maxDamage;
    public float attackRate;
    public int armor;
    public string descriptionArmor;
    public int magicResistance;
    public string descriptionMagicResistance;
    public float speed;
    public string descriptionSpeed;
    public int life;
    public int bounty;

    public int Damage { get => Random.Range(minDamage, maxDamage + 1); }
}
