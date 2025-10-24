using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReinforcementInfoUI : InfoUI
{
    public GameObject selectionCircle;
    Reinforcement reinforcement;

    void Awake()
    {
        reinforcement = GetComponent<Reinforcement>();
    }

    void Start()
    {
        nameInfo = reinforcement.nameReinforcement;
        UpdateHpInfoUI();
    }

    public void UpdateHpInfoUI()
    {
        power[0].powerIndex = (reinforcement.currentHealth + "/" + reinforcement.reinforcementStats.hp).ToString();
        power[1].powerIndex = (reinforcement.reinforcementStats.minPhysicalDamage + "-" + reinforcement.reinforcementStats.maxPhysicalDamage).ToString();
        power[2].powerIndex = reinforcement.reinforcementStats.descriptionArmor.ToString();
        power[3].powerIndex = "--";
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
