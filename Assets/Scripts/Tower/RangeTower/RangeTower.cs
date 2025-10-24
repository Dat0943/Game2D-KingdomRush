using Coffee.UIEffects;
using SpriteGlow;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public abstract class RangeTower : Tower
{
    protected RangeTowerStats[] rangeTowerStats;
    [HideInInspector] public RangeTowerStats rangeTowerCurrentStats;
    int currentLevel;

    [SerializeField] private GameObject range;
    [SerializeField] protected float delayShoot;
   
    public Enemy currentTarget;
    RangeInfoUI rangeInfoUI;

    void Awake()
    {
        rangeInfoUI = GetComponent<RangeInfoUI>();
    }

    public override void Start()
    {
        base.Start();

        rangeTowerStats = statData.OfType<RangeTowerStats>().ToArray();

        currentLevel = 1;
        ApplyStats();

        UpdateCostText(rangeTowerStats[currentLevel].buildCost);
        UpdateRangeUI();
        UpdateUpgradeUI();
    }

    void ApplyStats()
    {
        rangeTowerCurrentStats = rangeTowerStats[currentLevel - 1];
    }

    void UpdateUpgradeUI()
    {
        if (currentLevel >= TowerManager.Ins.maxTowerUpgradeLevel)
        {
            upgradeTowerButton.GetComponent<Image>().sprite = TowerManager.Ins.lockSprite;
            upgradeTowerButton.enabled = false;
            upgradeTowerButton.GetComponent<UIEffect>().enabled = false;
            upgradeTowerButton.GetComponent<OutlineUI>().enabled = false;
            costPanel.SetActive(false);
        }
        else
        {
            if(GameManager.Ins.currentGoldInGame >= rangeTowerStats[currentLevel].buildCost)
            {
                upgradeTowerButton.GetComponent<Image>().sprite = TowerManager.Ins.upgradeSprite;
                upgradeTowerButton.enabled = true;
                upgradeTowerButton.GetComponent<OutlineUI>().enabled = true;
                costText.color = TowerManager.Ins.normalcostTextColor;
            }
            else
            {
                upgradeTowerButton.GetComponent<Image>().sprite = TowerManager.Ins.unUpgradeSprite;
                upgradeTowerButton.enabled = false;
                upgradeTowerButton.GetComponent<OutlineUI>().enabled = false;
                costText.color = TowerManager.Ins.unnormalcostTextColor;
            }
        }
    }

    public void Upgrade()
    {
        if (currentLevel < rangeTowerStats.Length && GameManager.Ins.currentGoldInGame >= rangeTowerStats[currentLevel].buildCost)
        {
            currentLevel++;
            ApplyStats();

            GameManager.Ins.currentGoldInGame -= rangeTowerCurrentStats.buildCost;
            GuiManager.Ins.UpdateGoldInGame(GameManager.Ins.currentGoldInGame);

            Instantiate(GameManager.Ins.smokePrefab, new Vector3(transform.position.x, transform.position.y - 0.15f, transform.position.z), Quaternion.identity);
            UpdateSprite();

            UpdateRangeUI();
            UpdateCostText(rangeTowerStats[currentLevel - 1].upgradeBuildCost);

            rangeInfoUI.CloseInfoUI();
            rangeInfoUI.ShowInfoUI();
            StartCoroutine(HideMenu());
        }
    }

    public void Sell()
    {
        int saleAmount = (rangeTowerCurrentStats.buildCost * 6) / 10;
        GameManager.Ins.currentGoldInGame += saleAmount;
        GuiManager.Ins.UpdateGoldInGame(GameManager.Ins.currentGoldInGame);

        Instantiate(GameManager.Ins.smokePrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        LeanTweenManager.Ins.CloseInfoPanel();
        TowerManager.Ins.towerIsOpenMenu = false;
        currentSpot.SetActive(true);
        Destroy(gameObject);
    }

    protected abstract void UpdateSprite();

    public override void Update()
    {
        UpdateUpgradeUI();

        HandleMenuClick();

        if (!IsValidTarget(currentTarget))
        {
            currentTarget = GetNearestEnemy();
        }
    }

    protected virtual IEnumerator Attack()
    {
        while (true)
        {
            if (!IsValidTarget(currentTarget))
            {
                currentTarget = GetNearestEnemy();
            }

            if (IsValidTarget(currentTarget))
            {
                yield return ExecuteAttack();
            }
            else
            {
                yield return null;
            }
        }
    }

    protected abstract IEnumerator ExecuteAttack();

    protected virtual Enemy GetNearestEnemy()
    {
        List<Enemy> enemies = GameManager.Ins.activeEnemies;
        float shortestDistance = Mathf.Infinity;
        Enemy nearestEnemy = null;

        foreach (var enemy in enemies)
        {
            if (!IsTargetInRange(enemy)) continue;

            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);

            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }
        return nearestEnemy;
    }

    protected bool IsValidTarget(Enemy e)
    {
        if (e == null) return false;
        if (e.isDead) return false;
        if (!IsTargetInRange(e)) return false;
        return true;
    }

    protected virtual bool IsTargetInRange(Enemy enemy)
    {
        Vector2 towerPos = transform.position;
        Vector2 enemyPos = enemy.transform.position;
        Vector2 diff = enemyPos - towerPos;

        float a = 0.5f * rangeTowerCurrentStats.rangeOfLength;
        float b = 0.5f * rangeTowerCurrentStats.rangeOfHeight;

        // Phương trình elip: (x^2 / a^2) + (y^2 / b^2) <= 1
        return (diff.x * diff.x) / (a * a) + (diff.y * diff.y) / (b * b) <= 1f;
    }

    void UpdateRangeUI()
    {
        range.transform.localScale = new Vector3(rangeTowerCurrentStats.rangeOfLength, rangeTowerCurrentStats.rangeOfHeight, 0f);
    }
}
