using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MilitiaInfoUI : InfoUI
{
    public GameObject selectionCircle;
    Militia militia;
    
    void Awake()
    {
        militia = GetComponent<Militia>();    
    }

    void Start()
    {
        nameInfo = militia.nameMilitia;
        UpdateHpInfoUI();
    }

    public void UpdateHpInfoUI()
    {
        power[0].powerIndex = (militia.currentHealth + "/" + militia.barrackTower.barrackTowerCurrentStats.hp).ToString();
        power[1].powerIndex = militia.barrackTower.GetComponent<MeleeInfoUI>().power[1].powerIndex;
        power[2].powerIndex = militia.barrackTower.GetComponent<MeleeInfoUI>().power[2].powerIndex;
        power[3].powerIndex = militia.barrackTower.GetComponent<MeleeInfoUI>().power[3].powerIndex;
    }

    public override void CloseInfoUI()
    {
        base.CloseInfoUI();
    }

    public override void OnMouseDown()
    {
        UpdateHpInfoUI();
        base.OnMouseDown();
    }

    public override GameObject GetSelectionCircle() => selectionCircle;
}
