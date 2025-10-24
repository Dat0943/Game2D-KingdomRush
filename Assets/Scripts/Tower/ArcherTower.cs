using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpriteGlow;
using UnityEngine.EventSystems;
using System.Linq;

public class ArcherTower : RangeTower
{
    ArcherTowerStats archerTowerCurrentStats;

    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform shootPoint1;
    [SerializeField] private Transform shootPoint2;
    [SerializeField] private GameObject animArcher1;
    [SerializeField] private GameObject animArcher2;

    public override void Start()
    {
        base.Start();

        archerTowerCurrentStats = (ArcherTowerStats) rangeTowerCurrentStats;

        TowerManager.Ins.spawnCountArcher++;
        idTower = "ArcherTower" + TowerManager.Ins.spawnCountArcher.ToString();

        StartCoroutine(Attack());
    }

    protected override IEnumerator ExecuteAttack()
    {
        if (currentTarget == null || currentTarget.isDead)
            yield break;

        ArcherDirection(animArcher1.GetComponent<Animator>());
        yield return DelayShootArrow(shootPoint1, currentTarget);

        if (currentTarget == null || currentTarget.isDead)
        {
            currentTarget = GetNearestEnemy();
        }

        if (currentTarget == null || currentTarget.isDead)
            yield break;

        yield return new WaitForSeconds(rangeTowerCurrentStats.attackSpeed);

        if (currentTarget == null || currentTarget.isDead)
        {
            currentTarget = GetNearestEnemy();
        }

        if (currentTarget != null && !currentTarget.isDead && IsTargetInRange(currentTarget))
        {
            ArcherDirection(animArcher2.GetComponent<Animator>());
            yield return DelayShootArrow(shootPoint2, currentTarget);
        }

        yield return new WaitForSeconds(rangeTowerCurrentStats.attackSpeed);
    }

    void ArcherDirection(Animator anim)
    {
        Vector3 dir = currentTarget.transform.position - transform.position;

        bool isRight = dir.x >= 0;
        bool isUp = dir.y > 0;

        Vector3 localScale = anim.transform.localScale;
        localScale.x = Mathf.Abs(localScale.x); 
        anim.transform.localScale = localScale;

        if (isUp) // trên
        {
            if (isRight)
            {
                anim.SetTrigger(AnimConsts.SHOOT1_ANIM);
            }
            else // trái
            {
                anim.SetTrigger(AnimConsts.SHOOT1_ANIM);
                anim.transform.localScale = new Vector3(-localScale.x, localScale.y, localScale.z);
            }
        }
        else // dưới hoặc ngang
        {
            if (isRight)
            {
                anim.SetTrigger(AnimConsts.SHOOT_ANIM);
            }
            else // trái
            {
                anim.SetTrigger(AnimConsts.SHOOT_ANIM);
                anim.transform.localScale = new Vector3(-localScale.x, localScale.y, localScale.z);
            }
        }
    }

    IEnumerator DelayShootArrow(Transform shootPoint, Enemy target)
    {
        yield return new WaitForSeconds(delayShoot);
        if (target != null)
            ShootArrow(shootPoint, target);
    }

    void ShootArrow(Transform shootPoint, Enemy target)
    {
        GameObject arrowObj = Instantiate(arrowPrefab, shootPoint.position, Quaternion.identity);
        Arrow arrow = arrowObj.GetComponent<Arrow>();
        arrow.SetTarget(target, rangeTowerCurrentStats.PhysicalDamage);
    }

    protected override void UpdateSprite()
    {
        archerTowerCurrentStats = (ArcherTowerStats)rangeTowerCurrentStats;
        GetComponent<SpriteRenderer>().sprite = archerTowerCurrentStats.archerTowerUpgradeSprite;
        animArcher1.GetComponent<Animator>().runtimeAnimatorController = archerTowerCurrentStats.archerUpgradeAnim;
        animArcher2.GetComponent<Animator>().runtimeAnimatorController = archerTowerCurrentStats.archerUpgradeAnim;
    }
}
