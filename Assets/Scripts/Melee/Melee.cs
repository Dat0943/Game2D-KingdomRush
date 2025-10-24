using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Melee : MonoBehaviour
{
    [Header("IdleFlip")]
    [SerializeField] private float maxFlipInterval;
    [SerializeField] private float minFlipInterval;
    Coroutine flipCo;
    [HideInInspector] public bool isIdle;

    [Header("Move")]
    [SerializeField] private float speed;
    [HideInInspector] public Coroutine moveCo;
    [HideInInspector] public bool isRunning;

    [Header("Attack")]
    [SerializeField] private float combatRange;
    [SerializeField] private float attackRange;
    [HideInInspector] public float lastAttackTime;
    [HideInInspector] public bool isAttack;

    [Header("Health")]
    public Slider healthBarSlider;
    [HideInInspector] public int currentHealth;
    [HideInInspector] public bool isDead;

    [HideInInspector] public Vector2 originalPos;
    [HideInInspector] public Enemy blockedEnemy;
    [HideInInspector] public Collider2D myCollider;
    [HideInInspector] public Animator anim;

    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        myCollider = GetComponent<Collider2D>();
    }

    protected virtual void Start()
    {
        if (healthBarSlider != null)
        {
            healthBarSlider.maxValue = 1f;
            healthBarSlider.value = 1f;
        }
    }

    #region Idle
    void SetIdle(bool value)
    {
        isIdle = value;

        if (isIdle && flipCo == null)
        {
            flipCo = StartCoroutine(IdleFlip());
        }
        else if (!isIdle && flipCo != null)
        {
            StopCoroutine(flipCo);
            flipCo = null;
        }
    }

    IEnumerator IdleFlip()
    {
        while (true)
        {
            float flipInterval = Random.Range(minFlipInterval, maxFlipInterval);

            yield return new WaitForSeconds(flipInterval);

            if (isIdle)
            {
                bool flipRight = Random.value > 0.5f;
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x) * (flipRight ? 1 : -1);
                transform.localScale = scale;

                Vector3 healthBarSliderScale = healthBarSlider.transform.localScale;
                healthBarSliderScale.x = Mathf.Abs(healthBarSliderScale.x) * (flipRight ? 1 : -1);
                healthBarSlider.transform.localScale = healthBarSliderScale;
            }
        }
    }
    #endregion

    #region Run
    public void RunTo(Vector3 targetWorldPos)
    {
        if (moveCo != null) StopCoroutine(moveCo);
        moveCo = StartCoroutine(MoveTo(targetWorldPos));
    }

    protected IEnumerator MoveTo(Vector3 target)
    {
        HandleRunningAnim(true);

        while ((transform.position - target).sqrMagnitude > 0.03f * 0.03f)
        {
            if (blockedEnemy != null && !blockedEnemy.isDead)
            {
                target = CalculateCombatPosition(blockedEnemy.transform.position);
            }

            Vector3 dir = (target - transform.position).normalized;
            transform.position += dir * speed * Time.deltaTime;

            if (blockedEnemy == null || blockedEnemy.isDead)
                MeleeDirection(target, transform.position);
            else
            {
                if ((target.x < transform.position.x && transform.position.x < blockedEnemy.transform.position.x) || (target.x > transform.position.x && transform.position.x > blockedEnemy.transform.position.x))
                    MeleeDirection(blockedEnemy.transform.position, transform.position);
                else
                    MeleeDirection(target, transform.position);
            }

            yield return null;
        }

        HandleRunningAnim(false);

        moveCo = null;
    }
    #endregion

    protected virtual void Update()
    {
        HandleCombat();
    }

    #region Attack
    void HandleCombat()
    {
        if (blockedEnemy == null || blockedEnemy.isDead)
        {
            FindNewEnemy();
        }
        else
        {
            if (moveCo != null && !IsEnemyInRangeAttack(blockedEnemy))
            {
                StopCoroutine(moveCo);
                moveCo = StartCoroutine(MoveTo(blockedEnemy.transform.position));
            }

            if (IsEnemyInRangeAttack(blockedEnemy))
            {
                MeleeDirection(blockedEnemy.transform.position, transform.position);

                MilitiaAttack();

                if (moveCo != null)
                {
                    moveCo = null;
                    HandleRunningAnim(false);
                }
            }
        }
    }

    protected abstract void MilitiaAttack();
    protected abstract void ReturnToOriginalPos();

    protected void Attack()
    {
        isAttack = true;
        isIdle = false;
        isRunning = false;

        anim.SetTrigger(AnimConsts.ATTACK_ANIM);

    }

    Vector3 CalculateCombatPosition(Vector3 enemyPosition)
    {
        Vector3 directionToEnemy = (enemyPosition - transform.position).normalized;

        Vector3 combatPos = enemyPosition;
        combatPos.x -= Mathf.Sign(directionToEnemy.x) * 0.3f;

        return combatPos;
    }

    void FindNewEnemy()
    {
        Enemy closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Enemy enemy in GameManager.Ins.activeEnemies)
        {
            if (enemy.isDead || enemy.isBlocked) continue;

            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < combatRange && distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        if (closestEnemy != null)
        {
            blockedEnemy = closestEnemy;
            closestEnemy.SetBlockedBy(this);

            RunTo(closestEnemy.transform.position);
        }
        else
        {
            if (isAttack || moveCo == null)
            {
                isAttack = false;
                ReturnToOriginalPos();
            }
        }
    }

    bool IsEnemyInRangeAttack(Enemy enemy)
    {
        return Vector2.Distance(transform.position, enemy.transform.position) <= attackRange;
    }

    public void ReleaseEnemy()
    {
        if (blockedEnemy != null)
        {
            blockedEnemy.SetBlockedBy(null);
            blockedEnemy = null;
        }
    }
    #endregion

    #region Health
    public abstract void TakeDamage(int damage);
    #endregion

    public void HandleRunningAnim(bool value)
    {
        if (anim)
            anim.SetBool(AnimConsts.RUNNING_ANIM, value);
        isRunning = value;
        isAttack = false;
        SetIdle(!value);
    }

    public void MeleeDirection(Vector3 targetTo, Vector3 targetFrom)
    {
        Vector3 dir = targetTo - targetFrom;

        bool isRight = dir.x >= 0;

        Vector3 localScale = transform.localScale;
        localScale.x = Mathf.Abs(localScale.x);
        transform.localScale = localScale;

        Vector3 healthBarLocalScale = healthBarSlider.transform.localScale;
        healthBarLocalScale.x = Mathf.Abs(healthBarLocalScale.x);
        healthBarSlider.transform.localScale = healthBarLocalScale;

        if (isRight)
        {
            transform.localScale = new Vector3(localScale.x, localScale.y, localScale.z);
            healthBarSlider.transform.localScale = new Vector3(healthBarLocalScale.x, healthBarLocalScale.y, healthBarLocalScale.z);
        }
        else
        {
            transform.localScale = new Vector3(-localScale.x, localScale.y, localScale.z);
            healthBarSlider.transform.localScale = new Vector3(-healthBarLocalScale.x, healthBarLocalScale.y, healthBarLocalScale.z);
        }
    }
}
