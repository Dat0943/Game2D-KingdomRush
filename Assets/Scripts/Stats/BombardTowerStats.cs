using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bombard Tower Stats", menuName = "KR/Create Bombard Tower Stats")]
public class BombardTowerStats : RangeTowerStats
{
    public Sprite gunMountUpgradeSprite;
    public Sprite bomb3UpgradeSprite;
    public RuntimeAnimatorController gunner1UpgradeAnim;
    public GameObject bombPrefab;
}
