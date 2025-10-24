using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Militia : Melee
{
    [HideInInspector] public string nameMilitia;

    [Header("Healing")]
    [SerializeField] private float timeToStartHealing = 3f; 
    [SerializeField] private float healPercent = 0.1f; 
    Coroutine healingCo;
    float idleTimer;

    [HideInInspector] public BarrackTower barrackTower;
    [HideInInspector] public MilitiaInfoUI militiaInfoUI;

    protected override void Awake()
    {
        base.Awake();
        militiaInfoUI = GetComponent<MilitiaInfoUI>();
    }

    protected override void Start()
    {
        currentHealth = barrackTower.barrackTowerCurrentStats.hp;

        base.Start();
    }

    protected override void Update()
    {
        if(isDead) return;

        if (barrackTower != null && barrackTower.isMoveGroup)
        {
            HandleRunningAnim(true);
            return; 
        }

        HandleHealing();

        base.Update();
    }

    #region Attack
    protected override void MilitiaAttack()
    {
        if (isDead) return;

        if (Time.time - lastAttackTime >= barrackTower.barrackTowerCurrentStats.attackSpeed)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    public void DealDamage()
    {
        if (blockedEnemy != null)
        {
            blockedEnemy.TakeDamage(barrackTower.barrackTowerCurrentStats.PhysicalDamage, DamageType.Physical);
        }
    }

    protected override void ReturnToOriginalPos()
    {
        Vector3 worldHomePos = barrackTower.militiasContainer.transform.TransformPoint(originalPos);

        float distToOrigin = Vector2.Distance(transform.position, worldHomePos);
        if (distToOrigin > 0.03f && moveCo == null)
        {
            RunTo(worldHomePos);
        }
    }
    #endregion

    #region Health
    public override void TakeDamage(int damage)
    {
        if (isDead) return;

        int finalDamage = CalculateFinalDamage(damage);
        currentHealth -= finalDamage;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (nameMilitia == GuiManager.Ins.nameInfoText.text.ToString())
        {
            militiaInfoUI.UpdateHpInfoUI();
            militiaInfoUI.UpdateInfoUI();
        }

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    int CalculateFinalDamage(int rawDamage)
    {
        float damageReductionPercent = barrackTower.barrackTowerCurrentStats.armor / 100f;
        int damageAfterArmor = Mathf.Max((int)(rawDamage * (1f - damageReductionPercent)), 1);
        return damageAfterArmor;
    }

    void UpdateHealthBar()
    {
        if (healthBarSlider == null) return;

        float healthPercent = (float)currentHealth / barrackTower.barrackTowerCurrentStats.hp;
        healthBarSlider.value = Mathf.Clamp01(healthPercent);
    }

    void Die()
    {
        isDead = true;
        isAttack = false;
        isRunning = false;
        isIdle = false;

        myCollider.enabled = false;
        anim.SetTrigger(AnimConsts.DEAD_ANIM);
        healthBarSlider.gameObject.SetActive(false);

        TowerManager.Ins.nameMilitias.Add(nameMilitia);
        if (nameMilitia == GuiManager.Ins.nameInfoText.text.ToString())
        {
            LeanTweenManager.Ins.CloseInfoPanel();
            militiaInfoUI.selectionCircle.SetActive(false);
        }

        barrackTower.OnMilitiaDied(this);

        Destroy(gameObject, 1f);
    }

    void HandleHealing()
    {
        if (isIdle && currentHealth < barrackTower.barrackTowerCurrentStats.hp)
        {
            idleTimer += Time.deltaTime;

            if (idleTimer >= timeToStartHealing && healingCo == null)
            {
                healingCo = StartCoroutine(Healing());
            }
        }
        else
        {
            idleTimer = 0f;
            if (healingCo != null)
            {
                StopCoroutine(healingCo);
                healingCo = null;
            }
        }
    }

    IEnumerator Healing()
    {
        while (true)
        {
            int maxHP = barrackTower.barrackTowerCurrentStats.hp;
            int healingAmount = Mathf.CeilToInt(maxHP * healPercent);

            currentHealth = Mathf.Min(currentHealth + healingAmount, maxHP);

            if (nameMilitia == GuiManager.Ins.nameInfoText.text.ToString())
            {
                militiaInfoUI.UpdateHpInfoUI();
                militiaInfoUI.UpdateInfoUI();
            }

            UpdateHealthBar();

            yield return new WaitForSeconds(1f); 
        }
    }
    #endregion
}
