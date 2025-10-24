using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    public EnemyStats enemyStats;

    [HideInInspector] public int idEnemy;

    [SerializeField] private Slider healthBarSlider;
    [HideInInspector] public int currentHealth;
    [HideInInspector] public bool isDead;

    [HideInInspector] public bool isBlocked;
    [HideInInspector] public Melee blockingMelee = null;
    float lastAttackTime;

    Animator anim;
    EnemyInfoUI enemyInfoUI;
    Collider2D myCollider;

    void Awake()
    {
        anim = GetComponent<Animator>();
        enemyInfoUI = GetComponent<EnemyInfoUI>();
        myCollider = GetComponent<Collider2D>();
    }

    void OnEnable()
    {
        GameManager.Ins.RegisterEnemy(this);
    }

    void OnDisable()
    {
        GameManager.Ins.UnregisterEnemy(this);
    }

    void Start()
    {
        currentHealth = enemyStats.health;
        if(healthBarSlider != null )
        {
            healthBarSlider.maxValue = 1f;
            healthBarSlider.value = 1f;
        }
    }

    #region Take Damage
    public void TakeDamage(int damage, DamageType damageType)
    {
        if (isDead) return;

        int finalDamage = CalculateFinalDamage(damage, damageType);
        currentHealth -= finalDamage;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (idEnemy == GameManager.Ins.currentIdEnemy && enemyInfoUI.isActive)
        {
            enemyInfoUI.UpdateHpInfoUI();
            enemyInfoUI.UpdateInfoUI();
        }

        UpdateHealthBar();
        StartCoroutine(HitFlash());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    #region Calculate Take Damage
    int CalculateFinalDamage(int rawDamage, DamageType damageType)
    {
        switch (damageType)
        {
            case DamageType.Physical:
                return CalculatePhysicalDamage(rawDamage);

            case DamageType.Magical:
                return CalculateMagicalDamage(rawDamage);

            default:
                return Mathf.Max(rawDamage, 1);
        }
    }

    int CalculatePhysicalDamage(int rawDamage)
    {
        float damageReductionPercent = enemyStats.armor / 100f;
        int damageAfterArmor = Mathf.Max((int)(rawDamage * (1f - damageReductionPercent)), 1);
        return damageAfterArmor;
    }

    int CalculateMagicalDamage(int rawDamage)
    {
        float damageReductionPercent = enemyStats.magicResistance / 100f;
        int damageAfterMR = Mathf.Max((int)(rawDamage * (1f - damageReductionPercent)), 1);
        return damageAfterMR;
    }
    #endregion

    void UpdateHealthBar()
    {
        if (healthBarSlider == null) return;

        float healthPercent = (float) currentHealth / enemyStats.health;
        healthBarSlider.value = Mathf.Clamp01(healthPercent); 
    }

    IEnumerator HitFlash()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color originalColor = sr.color;
            sr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sr.color = originalColor;
        }
    }

    void Die()
    {
        isDead = true;

        GameManager.Ins.currentGoldInGame += enemyStats.bounty;
        GuiManager.Ins.UpdateGoldInGame(GameManager.Ins.currentGoldInGame);

        myCollider.enabled = false;

        if (blockingMelee != null)
        {
            blockingMelee.ReleaseEnemy();
        }

        if (anim.transform.localScale.x >= 0)
            SetAnimation(AnimConsts.ENEMYDIERIGHT_ANIM);
        else
            SetAnimation(AnimConsts.ENEMYDIELEFT_ANIM);

        healthBarSlider.gameObject.SetActive(false);

        if (idEnemy == GameManager.Ins.currentIdEnemy && enemyInfoUI.isActive)
        {
            LeanTweenManager.Ins.CloseInfoPanel();
            enemyInfoUI.isActive = false;
            enemyInfoUI.selectionCircle.SetActive(false);
        }

        Destroy(gameObject, 1f);
    }
    #endregion

    public void SetAnimation(string animName)
    {
        anim.Play(animName);
    }

    public void SetBlockedBy(Melee militia)
    {
        if (militia == null)
        {
            isBlocked = false;
            if (blockingMelee != null)
            {
                blockingMelee = null;
            }
        }
        else
        {
            isBlocked = true;
            blockingMelee = militia;
        }
    }

    public void EnemyAttack()
    {
        if (Time.time - lastAttackTime >= enemyStats.attackRate)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    void Attack()
    {
        DirectionEnemy();
    }

    public void DealDamage()
    {
        if (blockingMelee != null && !blockingMelee.isDead)
        {
            blockingMelee.TakeDamage(enemyStats.Damage);
        }
    }

    void DirectionEnemy()
    {
        if (blockingMelee == null || blockingMelee.isDead) return;

        Vector3 dir = (blockingMelee.transform.position - transform.position).normalized;

        bool isRight = dir.x >= 0;

        Vector3 localScale = transform.localScale;
        float absoluteX = Mathf.Abs(localScale.x);

        if (isRight)
            anim.SetTrigger(AnimConsts.ENEMYATTACKRIGHT_ANIM);
        else
            anim.SetTrigger(AnimConsts.ENEMYATTACKLEFT_ANIM);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(TagConsts.ENDPOINT_TAG))
        {
            GameManager.Ins.TakeDamage();
            GameManager.Ins.currentGoldInGame += 1;
            GuiManager.Ins.UpdateGoldInGame(GameManager.Ins.currentGoldInGame);
            Die();
        }
    }
}
