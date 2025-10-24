using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeInfoUI : InfoUI
{
    RangeTower rangeTower;

    void Awake()
    {
        rangeTower = GetComponent<RangeTower>();
    }

    void Start()
    {
        ShowInfoUI();
    }

    public void ShowInfoUI()
    {
        nameInfo = rangeTower.rangeTowerCurrentStats.nameTower;
        if (rangeTower.rangeTowerCurrentStats.isPhysical)
            power[0].powerIndex = (rangeTower.rangeTowerCurrentStats.minPhysicalDamage + "-" + rangeTower.rangeTowerCurrentStats.maxPhysicalDamage).ToString();
        else
            power[0].powerIndex = (rangeTower.rangeTowerCurrentStats.minMagicalDamage + "-" + rangeTower.rangeTowerCurrentStats.maxMagicalDamage).ToString();
        power[1].powerIndex = rangeTower.rangeTowerCurrentStats.descriptionAttackSpeed;
        power[2].powerIndex = rangeTower.rangeTowerCurrentStats.descriptionRange;
    }
}
