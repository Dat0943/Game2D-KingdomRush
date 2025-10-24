using Coffee.UIEffects;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject ruinPrefab;
    [SerializeField] private int buildCost;
    [SerializeField] private float offset;

    [SerializeField] private string nameTower;
    [SerializeField] private string infoTower;
    [SerializeField] private string damage;
    [SerializeField] private string attackSpeed;
    [SerializeField] private Sprite damageSprite;
    [SerializeField] private Sprite attackSpeedSprite;

    public void OnPointerEnter(PointerEventData eventData)
    {
        TowerManager.Ins.infoPanel.SetActive(true);
        TowerManager.Ins.UpdateTowerUI(nameTower, infoTower, damageSprite, damage, attackSpeedSprite, attackSpeed);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TowerManager.Ins.infoPanel.SetActive(false);
    }

    public void BuildTower()
    {
        if (GameManager.Ins.currentGoldInGame < buildCost) return;

        GameManager.Ins.currentGoldInGame -= buildCost;
        GuiManager.Ins.UpdateGoldInGame(GameManager.Ins.currentGoldInGame);

        TowerManager.Ins.infoPanel.SetActive(false);
        TowerManager.Ins.BuildTower(ruinPrefab, offset);
    }
}
