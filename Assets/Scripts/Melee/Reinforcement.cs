using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Reinforcement : Melee
{
    [Header("Stats")]
    [HideInInspector] public DefaultSkill2Stats reinforcementStats;
    [HideInInspector] public string nameReinforcement;
    ReinforcementInfoUI reinforcementInfoUI;

    protected override void Awake()
    {
        base.Awake();

        reinforcementInfoUI = GetComponent<ReinforcementInfoUI>();
    }
    protected override void Start()
    {
        HandleRunningAnim(false);
        currentHealth = reinforcementStats.hp;

        base.Start();

        StartCoroutine(FadeIn(100f / 255f, 1f, 0.4f));
        StartCoroutine(AutoDespawn());
    }

    IEnumerator AutoDespawn()
    {
        yield return new WaitForSeconds(15f);

        if (!isDead)
        {
            Die();
        }
    }

    IEnumerator FadeIn(float begin, float end, float duration)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) yield break;

        Color c = sr.color;
        c.a = begin; 
        sr.color = c;

        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(begin, end, t / duration);
            c.a = alpha;
            sr.color = c;
            yield return null;
        }

        c.a = end;
        sr.color = c;
    }

    protected override void Update()
    {
        if (isDead) return;
        base.Update();
    }

    protected override void MilitiaAttack()
    {
        if (isDead) return;

        if (Time.time - lastAttackTime >= reinforcementStats.attackSpeed)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    public void DealDamage()
    {
        if (blockedEnemy != null)
        {
            blockedEnemy.TakeDamage(reinforcementStats.PhysicalDamage, DamageType.Physical);
        }
    }

    protected override void ReturnToOriginalPos()
    {
        float distToOrigin = Vector2.Distance(transform.position, originalPos);
        if (distToOrigin > 0.03f && moveCo == null)
        {
            RunTo(originalPos);
        }
    }

    public override void TakeDamage(int damage)
    {
        if (isDead) return;

        int finalDamage = Mathf.Max(damage - reinforcementStats.armor, 1);
        currentHealth -= finalDamage;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (nameReinforcement == GuiManager.Ins.nameInfoText.text.ToString())
        {
            reinforcementInfoUI.UpdateHpInfoUI();
            reinforcementInfoUI.UpdateInfoUI();
        }

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBarSlider == null) return;

        float healthPercent = (float)currentHealth / reinforcementStats.hp;
        healthBarSlider.value = Mathf.Clamp01(healthPercent);
    }

    void Die()
    {
        if (isDead) return; 

        isDead = true;
        isAttack = false;
        isRunning = false;
        isIdle = false;

        myCollider.enabled = false;
        anim.SetTrigger(AnimConsts.DEAD_ANIM);
        if (healthBarSlider != null)
            healthBarSlider.gameObject.SetActive(false);

        TowerManager.Ins.nameReinforcements.Add(nameReinforcement);
        if (nameReinforcement == GuiManager.Ins.nameInfoText.text.ToString())
        {
            LeanTweenManager.Ins.CloseInfoPanel();
            reinforcementInfoUI.selectionCircle.SetActive(false);
        }

        StartCoroutine(FadeIn(1f, 0f, 1f));

        Destroy(gameObject, 1f); 
    }
}