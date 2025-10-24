using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Default Skill 1 Stats", menuName = "KR/Create Default Skill 1 Stats")]
public class DefaultSkill1Stats : ScriptableObject
{
    public int level;
    public int numberOfMeteors;
    public int minMeteorDamage;
    public int maxMeteorDamage;
    public bool isScorchedEarth;
    public int scorchedEarthDuration;
    public int minScorchedEarthDamage;
    public int maxScorchedEarthDamage;
    public float coolDown;
    public int MeteorDamage { get => Random.Range(minMeteorDamage, maxMeteorDamage + 1); }
    public int ScorchedEarthDamage { get => Random.Range(minScorchedEarthDamage, maxScorchedEarthDamage + 1); }
}
