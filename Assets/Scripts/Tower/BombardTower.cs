using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpriteGlow;
using UnityEngine.EventSystems;

public class BombardTower : RangeTower
{
    BombardTowerStats bombardTowerCurrentStats;

    [SerializeField] private GameObject gunMount;
    [SerializeField] private GameObject bomb3;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private GameObject smokeBarrelPrefab;
    [SerializeField] private Transform smokeBarrelPoint;
    [SerializeField] private Animator gunBarrelAnim;
    [SerializeField] private Animator gunner1Anim;
    [SerializeField] private Animator gunner2Anim;

    public override void Start()
    {
        base.Start();

        bombardTowerCurrentStats = (BombardTowerStats)rangeTowerCurrentStats;

        TowerManager.Ins.spawnCountBombard++;
        idTower = "BombardTower" + TowerManager.Ins.spawnCountBombard.ToString();

        StartCoroutine(Attack());
    }

    protected override IEnumerator ExecuteAttack()
    {
        gunBarrelAnim.SetTrigger(AnimConsts.SHOOT_ANIM);
        gunner2Anim.SetTrigger(AnimConsts.SHOOT_ANIM);
        yield return new WaitForSeconds(delayShoot);
        if (currentTarget != null)
            ShootArrow(shootPoint, currentTarget);
        gunner1Anim.SetTrigger(AnimConsts.SHOOT_ANIM);

        yield return new WaitForSeconds(rangeTowerCurrentStats.attackSpeed - delayShoot);
    }

    void ShootArrow(Transform shootPoint, Enemy target)
    {
        GameObject bombObj = Instantiate(bombardTowerCurrentStats.bombPrefab, shootPoint.position, Quaternion.identity);
        Bomb bomb = bombObj.GetComponent<Bomb>();
        bomb.SetTarget(shootPoint.transform.position, target, rangeTowerCurrentStats.PhysicalDamage);

        GameObject smokeObj = Instantiate(smokeBarrelPrefab, Vector3.zero, Quaternion.identity);
        smokeObj.transform.SetParent(smokeBarrelPoint, false);
        Destroy(smokeObj, 2f);
    }

    protected override void UpdateSprite()
    {
        bombardTowerCurrentStats = (BombardTowerStats)rangeTowerCurrentStats;
        gunMount.GetComponent<SpriteRenderer>().sprite = bombardTowerCurrentStats.gunMountUpgradeSprite;
        bomb3.GetComponent<SpriteRenderer>().sprite = bombardTowerCurrentStats.bomb3UpgradeSprite;
        gunner1Anim.runtimeAnimatorController = bombardTowerCurrentStats.gunner1UpgradeAnim;
    }
}
