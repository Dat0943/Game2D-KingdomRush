using HPC;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerManager : Singleton<TowerManager>
{
    public GameObject menuTower;
    public GameObject bgMenuTower;
    public GameObject infoPanel;

    [SerializeField] private TMP_Text nameTowerText;
    [SerializeField] private TMP_Text infoTowerText;
    [SerializeField] private Image damageImage;
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private Image attackSpeedImage;
    [SerializeField] private TMP_Text attackSpeedText;

    TowerSpot currentSpot;

    public int spawnCountArcher;
    public int spawnCountMage;
    public int spawnCountBombard;
    public int spawnCountBarrack;

    [HideInInspector] public bool barrackTowerIsActive;
    [HideInInspector] public bool towerIsOpenMenu;

    public List<string> nameMilitias;
    public List<string> nameReinforcements;

    [Header("Upgrade")]
    public int maxTowerUpgradeLevel;
    public Sprite upgradeSprite;
    public Sprite unUpgradeSprite;
    public Sprite lockSprite;
    public Color normalcostTextColor;
    public Color unnormalcostTextColor;

    protected override void Awake()
    {
        MakeSingleton(false);
    }

    void Update()
    {
        if (menuTower.activeSelf && Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Collider2D hit = Physics2D.OverlapPoint(mousePos);

                if (hit == null || hit.GetComponent<TowerSpot>() == null)
                {
                    StartCoroutine(HideMenu());
                }
            }
        }
    }

    public void ShowMenu(TowerSpot spot, Vector3 position)
    {
        EventSystem.current.SetSelectedGameObject(null);

        if (!menuTower.activeSelf)
        {
            currentSpot = spot;
            currentSpot.isShowMenu = true;

            if (currentSpot.glow != null)
                currentSpot.glow.OutlineWidth = 1;

            menuTower.SetActive(true);
            LeanTweenManager.Ins.OpenDialog(bgMenuTower);
            menuTower.transform.position = Camera.main.WorldToScreenPoint(position);
        }
        else
        {
            StartCoroutine(ChangeMenuPosition(spot, position));
        }
    }

    public void BuildTower(GameObject towerPrefab, float offset)
    {
        if (currentSpot != null)
        {
            currentSpot.BuildTower(towerPrefab, offset);
            menuTower.SetActive(false);
        }
    }

    IEnumerator ChangeMenuPosition(TowerSpot spot, Vector3 position)
    {
        StartCoroutine(HideMenu());
        yield return new WaitForSeconds(0.2f);
        currentSpot = spot;
        currentSpot.isShowMenu = true;

        if (currentSpot.glow != null)
            currentSpot.glow.OutlineWidth = 1;

        menuTower.SetActive(true);
        LeanTweenManager.Ins.OpenDialog(bgMenuTower);
        menuTower.transform.position = Camera.main.WorldToScreenPoint(position);
    }

    public IEnumerator HideMenu()
    {
        currentSpot.isShowMenu = false;

        if (currentSpot.glow != null)
            currentSpot.glow.OutlineWidth = 0;

        LeanTweenManager.Ins.CloseDialog(bgMenuTower);
        yield return new WaitForSeconds(0.2f);
        menuTower.SetActive(false);
    }

    public void UpdateTowerUI(string nameTower, string infoTower, Sprite damageSprite, string damage, Sprite attackSpeedSprite, string attackSpeed)
    {
        nameTowerText.text = nameTower;
        infoTowerText.text = infoTower;
        damageImage.sprite = damageSprite;
        damageText.text = damage;
        attackSpeedImage.sprite = attackSpeedSprite;
        attackSpeedText.text = attackSpeed;
    }
}
