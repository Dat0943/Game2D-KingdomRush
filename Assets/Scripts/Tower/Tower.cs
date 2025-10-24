using SpriteGlow;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{
    [Header("Common: ")]
    public TowerStats[] statData;

    [HideInInspector] public string idTower;

    [SerializeField] protected GameObject menu;
    [SerializeField] protected GameObject bgMenu;
    [SerializeField] protected SpriteGlowEffect glow;
    [SerializeField] protected TMP_Text costText;
    [SerializeField] protected GameObject costPanel;
    [SerializeField] protected Button upgradeTowerButton;

    [HideInInspector] public GameObject currentSpot;

    public virtual void Start()
    {
        if (glow != null)
            glow.OutlineWidth = 0;
    }

    public virtual void Update()
    {

    }

    protected void HandleMenuClick()
    {
        if (menu.activeSelf && Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Collider2D hit = Physics2D.OverlapPoint(mousePos);

                if (hit == null)
                {
                    StartCoroutine(HideMenu());
                }
                else
                {
                    Tower clickedTower = hit.GetComponent<Tower>();
                    if (clickedTower == null)
                    {
                        StartCoroutine(HideMenu());
                    }
                    else
                    {
                        if (clickedTower != null && clickedTower.idTower != idTower)
                        {
                            StartCoroutine(HideMenu());
                        }
                    }
                }
            }
        }
    }

    void OnMouseEnter()
    {
        if (TowerManager.Ins.towerIsOpenMenu) return;
        if (TowerManager.Ins.barrackTowerIsActive) return;

        if (glow != null)
            glow.OutlineWidth = 1;
    }

    void OnMouseExit()
    {
        if (TowerManager.Ins.barrackTowerIsActive) return;

        if (glow != null)
            glow.OutlineWidth = 0;
    }

    void OnMouseDown()
    {
        if (TowerManager.Ins.towerIsOpenMenu) return;
        if (TowerManager.Ins.barrackTowerIsActive) return;

        menu.SetActive(true);
        LeanTweenManager.Ins.OpenDialog(bgMenu);
        TowerManager.Ins.towerIsOpenMenu = true;
    }

    public IEnumerator HideMenu()
    {
        TowerManager.Ins.towerIsOpenMenu = false;
        LeanTweenManager.Ins.CloseDialog(bgMenu);
        yield return new WaitForSeconds(0.2f);
        menu.SetActive(false);
    }

    public void UpdateCostText(int buildCost)
    {
        costText.text = buildCost.ToString();
    }
}
