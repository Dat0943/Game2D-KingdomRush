using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeInfoUI : InfoUI
{
    BarrackTower barrackTower;

    void Awake()
    {
        barrackTower = GetComponent<BarrackTower>();
    }

    void Start()
    {
        ShowInfoUI();
    }

    public void ShowInfoUI()
    {
        nameInfo = barrackTower.barrackTowerCurrentStats.nameTower;
        power[0].powerIndex = barrackTower.barrackTowerCurrentStats.hp.ToString();
        power[1].powerIndex = (barrackTower.barrackTowerCurrentStats.minPhysicalDamage + "-" + barrackTower.barrackTowerCurrentStats.maxPhysicalDamage).ToString();
        power[2].powerIndex = barrackTower.barrackTowerCurrentStats.descriptionArmor;
        power[3].powerIndex = barrackTower.barrackTowerCurrentStats.respawnTime.ToString() + "s";
    }
}