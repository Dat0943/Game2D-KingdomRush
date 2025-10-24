using Coffee.UIEffects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MilitiaSpawnDirection
{
    public string name;             
    public Vector2[] localOffsets;
    public Vector2[] currentLocalOffsets;
    public Vector2 checkDirection;
    public Vector3 centerCoordinates;
}

public class BarrackTower : Tower
{
    BarrackTowerStats[] barrackTowerStats;
    [HideInInspector] public BarrackTowerStats barrackTowerCurrentStats;
    int currentLevel;

    [Header("Barracks")]
    [SerializeField] private Animator doorAnim;
    [SerializeField] private float spawnDelay = 0.15f;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int maxMilitias = 3;

    [Header("Custom Spawn Directions")]
    [SerializeField] private MilitiaSpawnDirection[] spawnDirections;
    [SerializeField] private GameObject militiasObj;
    public List<Militia> militias = new List<Militia>();
    [HideInInspector] public GameObject militiasContainer;
    MilitiaSpawnDirection chosenDir;

    [Header("Raycast")]
    [SerializeField] private float raycastDistance;
    [SerializeField] private LayerMask battlefieldLayer;

    [Header("Militia Move")]
    [SerializeField] private GameObject flagObj;
    [SerializeField] private GameObject cursorControlMilitia;
    [SerializeField] private GameObject cursorError;
    [SerializeField] private GameObject controlLimit;
    [SerializeField] private GameObject controlMilitiaButton;
    [SerializeField] private float selectRadius;
    [HideInInspector] public bool isMoveGroup;
    bool isSelecting;

    MeleeInfoUI meleeInfoUI;

    void Awake()
    {
        meleeInfoUI = GetComponent<MeleeInfoUI>();
    }

    public override void Start()
    {
        base.Start();

        barrackTowerStats = statData.OfType<BarrackTowerStats>().ToArray();

        currentLevel = 1;
        ApplyStats();

        UpdateCostText(barrackTowerStats[currentLevel].buildCost);

        UpdateUpgradeUI();

        TowerManager.Ins.spawnCountBarrack++;
        idTower = "BarrackTower" + TowerManager.Ins.spawnCountBarrack.ToString();

        StartCoroutine(SpawnOnBuild());
    }

    void ApplyStats()
    {
        barrackTowerCurrentStats = barrackTowerStats[currentLevel - 1];
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
            if (GameManager.Ins.currentGoldInGame >= barrackTowerStats[currentLevel].buildCost)
            {
                upgradeTowerButton.GetComponent<Image>().sprite = TowerManager.Ins.upgradeSprite;
                upgradeTowerButton.enabled = true;
                upgradeTowerButton.GetComponent<UIEffect>().enabled = true;
                upgradeTowerButton.GetComponent<OutlineUI>().enabled = true;
                costText.color = TowerManager.Ins.normalcostTextColor;
            }
            else
            {
                upgradeTowerButton.GetComponent<Image>().sprite = TowerManager.Ins.unUpgradeSprite;
                upgradeTowerButton.enabled = false;
                upgradeTowerButton.GetComponent<UIEffect>().enabled = false;
                upgradeTowerButton.GetComponent<OutlineUI>().enabled = false;
                costText.color = TowerManager.Ins.unnormalcostTextColor;
            }
        }
    }

    public void Upgrade()
    {
        if (currentLevel < barrackTowerStats.Length)
        {
            currentLevel++;
            ApplyStats();

            GameManager.Ins.currentGoldInGame -= barrackTowerCurrentStats.buildCost;
            GuiManager.Ins.UpdateGoldInGame(GameManager.Ins.currentGoldInGame);

            Instantiate(GameManager.Ins.smokePrefab, new Vector3(transform.position.x, transform.position.y - 0.15f, transform.position.z), Quaternion.identity);
            UpdateSprite();

            UpdateCostText(barrackTowerStats[currentLevel - 1].upgradeBuildCost);

            foreach (var militia in militias)
            {
                militia.militiaInfoUI.icon = barrackTowerCurrentStats.iconMilitia;
            }

            meleeInfoUI.CloseInfoUI();
            meleeInfoUI.ShowInfoUI();
            StartCoroutine(HideMenu());
        }
    }

    public void Sell()
    {
        int saleAmount = (barrackTowerCurrentStats.buildCost * 6) / 10;
        GameManager.Ins.currentGoldInGame += saleAmount;
        GuiManager.Ins.UpdateGoldInGame(GameManager.Ins.currentGoldInGame);

        Instantiate(GameManager.Ins.smokePrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        LeanTweenManager.Ins.CloseInfoPanel();
        TowerManager.Ins.towerIsOpenMenu = false;
        currentSpot.SetActive(true);

        foreach (var militia in militias)
        {
            TowerManager.Ins.nameMilitias.Add(militia.nameMilitia);
        }

        Destroy(militiasContainer);
        Destroy(gameObject);
    }

    void UpdateSprite()
    {
        GetComponent<SpriteRenderer>().sprite = barrackTowerCurrentStats.barrackTowerUpgradeSprite;
        foreach (var militia in militias)
        {
            militia.GetComponent<Animator>().runtimeAnimatorController = barrackTowerCurrentStats.militiaUpgradeAnim;
        }
    }

    #region Militia Starter
    IEnumerator SpawnOnBuild()
    {
        if (doorAnim != null) doorAnim.SetTrigger(AnimConsts.OPENDOOR_ANIM);

        yield return new WaitForSeconds(spawnDelay);

        StartCoroutine(SpawnInitialMilitias());
    }

    IEnumerator SpawnInitialMilitias()
    {
        List<MilitiaSpawnDirection> validDirs = new List<MilitiaSpawnDirection>();

        foreach (var dir in spawnDirections)
        {
            RaycastHit2D hit = Physics2D.Raycast(spawnPoint.position, dir.checkDirection, raycastDistance, battlefieldLayer);
            if (hit.collider != null)
            {
                validDirs.Add(dir);
            }
        }

        chosenDir = validDirs[Random.Range(0, validDirs.Count)];

        for (int i = 0; i < maxMilitias; i++)
        {
            Vector2 localOffset = chosenDir.localOffsets[Mathf.Clamp(i, 0, chosenDir.localOffsets.Length - 1)];
            Vector2 worldTarget = transform.TransformPoint(localOffset);

            Militia militiaClone = Instantiate(barrackTowerCurrentStats.militiaPrefab, spawnPoint.position, Quaternion.identity);

            int nameMilitiaIndex = Random.Range(0, TowerManager.Ins.nameMilitias.Count);
            militiaClone.nameMilitia = TowerManager.Ins.nameMilitias[nameMilitiaIndex];
            TowerManager.Ins.nameMilitias.RemoveAt(nameMilitiaIndex);

            militiaClone.barrackTower = this;
            militias.Add(militiaClone);
            militiaClone.RunTo(worldTarget);

            yield return new WaitForSeconds(0.03f);
        }

        militiasContainer = Instantiate(militiasObj, transform.position + chosenDir.centerCoordinates, Quaternion.identity);
        for (int i = 0; i < militias.Count; i++)
        {
            militias[i].transform.SetParent(militiasContainer.transform);
            militias[i].originalPos = chosenDir.currentLocalOffsets[i];
        }
    }
    #endregion

    public override void Update()
    {
        UpdateUpgradeUI();

        HandleMenuClick();
        ControlMilitia();
    }

    #region Control Militia
    void ControlMilitia()
    {
        if(isSelecting && Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;

            bool insideCircle = (mouseWorldPos - transform.position).magnitude <= selectRadius;
            Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos, battlefieldLayer);

            if (insideCircle && hit != null)
            {
                if (militiasContainer != null)
                {
                    StopCoroutine("MoveGroup");
                    StopCoroutine("FlagFly");

                    StartCoroutine(MoveGroup(mouseWorldPos));
                    StartCoroutine(FlagFly(mouseWorldPos));
                }

                UnActiveMilitiaMove();
            }
            else
            {
                StartCoroutine(Error());
            }
        }
    }

    public void ActiveMilitiaMove()
    {
        TowerManager.Ins.barrackTowerIsActive = true;
        isSelecting = true;
        controlLimit.SetActive(true);
        cursorControlMilitia.SetActive(true);
        StartCoroutine(HideMenu());
    }

    void UnActiveMilitiaMove()
    {
        TowerManager.Ins.barrackTowerIsActive = false;
        isSelecting = false;
        controlLimit.SetActive(false);
        cursorControlMilitia.SetActive(false);
    }

    IEnumerator Error()
    {
        cursorControlMilitia.SetActive(false);
        cursorError.SetActive(true);

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        cursorError.transform.position = mouseWorldPos;

        yield return new WaitForSeconds(0.3f);
        cursorError.SetActive(false);
        cursorControlMilitia.SetActive(true);
    }

    IEnumerator MoveGroup(Vector3 target)
    {
        foreach (var militia in militias)
        {
            if (militia != null && militia.blockedEnemy != null)
            {
                militia.ReleaseEnemy(); 
            }
        }

        while ((militiasContainer.transform.position - target).sqrMagnitude > 0.01f)
        {
            isMoveGroup = true;

            militiasContainer.transform.position = Vector3.MoveTowards(militiasContainer.transform.position, target, 1.2f * Time.deltaTime);

            foreach (var militia in militias)
            {
                militia.HandleRunningAnim(true);
                militia.MeleeDirection(target, militiasContainer.transform.position);
            }

            yield return null;
        }

        foreach (var militia in militias)
        {
            militia.HandleRunningAnim(false);
        }

        isMoveGroup = false;

        militiasContainer.transform.position = target;
    }
    #endregion

    #region RespawnMilitia
    public void OnMilitiaDied(Militia militia)
    {
        militias.Remove(militia);
        StartCoroutine(RespawnMilitia(militia.originalPos));
    }

    IEnumerator RespawnMilitia(Vector2 originalPos)
    {
        yield return new WaitForSeconds(barrackTowerCurrentStats.respawnTime);

        if (militias.Count >= maxMilitias) yield break;

        if (doorAnim != null) doorAnim.SetTrigger(AnimConsts.OPENDOOR_ANIM);
        yield return new WaitForSeconds(spawnDelay);

        Vector3 worldPos = militiasContainer.transform.TransformPoint(originalPos);

        Militia militiaClone = Instantiate(barrackTowerCurrentStats.militiaPrefab, spawnPoint.position, Quaternion.identity);

        int nameMilitiaIndex = Random.Range(0, TowerManager.Ins.nameMilitias.Count);
        militiaClone.nameMilitia = TowerManager.Ins.nameMilitias[nameMilitiaIndex];
        TowerManager.Ins.nameMilitias.RemoveAt(nameMilitiaIndex);

        militiaClone.barrackTower = this;
        militiaClone.originalPos = originalPos;

        militiaClone.RunTo(worldPos);

        militias.Add(militiaClone);
        militiaClone.transform.SetParent(militiasContainer.transform);
    }
    #endregion

    IEnumerator FlagFly(Vector3 target)
    {
        GameObject flagClone = Instantiate(flagObj, target + new Vector3(0.07f, 0.13f, 0f), Quaternion.identity);
        yield return new WaitForSeconds(0.6f);
        Destroy(flagClone);
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    foreach (var dir in spawnDirections)
    //    {
    //        Gizmos.color = Color.green;
    //        Gizmos.DrawLine(spawnPoint.position, spawnPoint.position + (Vector3)(dir.checkDirection * raycastDistance));
    //    }
    //}
}
