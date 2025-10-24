using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInfoUI : InfoUI
{
    public GameObject selectionCircle;
    Enemy enemy;
    [HideInInspector] public bool isActive;

    void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    void Start()
    {
        UpdateHpInfoUI();
    }

    public void UpdateHpInfoUI()
    {
        nameInfo = enemy.enemyStats.nameEnemy;
        power[0].powerIndex = (enemy.currentHealth + "/" + enemy.enemyStats.health).ToString();
        power[1].powerIndex = (enemy.enemyStats.minDamage + "-" + enemy.enemyStats.maxDamage).ToString();
        power[2].powerIndex = enemy.enemyStats.descriptionArmor;
        power[3].powerIndex = enemy.enemyStats.life.ToString();
    }

    public override void CloseInfoUI()
    {
        base.CloseInfoUI();
        isActive = false;
    }

    public override void OnMouseDown()
    {
        isActive = true;
        GameManager.Ins.currentIdEnemy = enemy.idEnemy;
        UpdateHpInfoUI();
        base.OnMouseDown();
    }

    public override GameObject GetSelectionCircle() => selectionCircle;
}
