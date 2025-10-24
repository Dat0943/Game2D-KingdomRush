using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RallyFlagUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject statisticsPanel;
    
    [SerializeField] private TMP_Text currentWaveText;
    [SerializeField] private TMP_Text numOfEnemyText;

    void OnEnable()
    {
        UpdateUI();
        if (statisticsPanel != null)
            statisticsPanel.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UpdateUI();
        if(statisticsPanel != null)
            statisticsPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (statisticsPanel != null)
            statisticsPanel.SetActive(false);
    }

    void UpdateUI()
    {
        currentWaveText.text = "Wave " + (GameManager.Ins.currentWaveIndex + 1).ToString();
        numOfEnemyText.text = GameManager.Ins.enemyStatistic;
    }

    public void OnClickHideStartImage()
    {
        if (!GameManager.Ins.isCalledEnemy)
            StartCoroutine(GuiManager.Ins.HideStartBattleImage());
    }
}
