using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpriteGlow;
using UnityEngine.EventSystems;

public class MageTower : RangeTower
{
    MageTowerStats mageTowerCurrentStats;

    [SerializeField] private GameObject magePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private GameObject mage;

    public override void Start()
    {
        base.Start();

        mageTowerCurrentStats = (MageTowerStats)rangeTowerCurrentStats;

        TowerManager.Ins.spawnCountMage++;
        idTower = "MageTower" + TowerManager.Ins.spawnCountMage.ToString();

        StartCoroutine(Attack());
    }

    protected override IEnumerator ExecuteAttack()
    {
        mage.GetComponent<Animator>().SetTrigger(AnimConsts.SHOOT_ANIM);
        GetComponent<Animator>().SetTrigger(AnimConsts.SHOOT_ANIM);
        yield return new WaitForSeconds(delayShoot);
        if (currentTarget != null)
            ShootArrow(shootPoint, currentTarget);

        yield return new WaitForSeconds(rangeTowerCurrentStats.attackSpeed - delayShoot);
    }

    void ShootArrow(Transform shootPoint, Enemy target)
    {
        GameObject mageObj = Instantiate(magePrefab, shootPoint.position, Quaternion.identity);
        Mage mage = mageObj.GetComponent<Mage>();
        mage.SetTarget(target, rangeTowerCurrentStats.MagicalDamage);
    }

    protected override void UpdateSprite()
    {
        mageTowerCurrentStats = (MageTowerStats)rangeTowerCurrentStats;
        GetComponent<SpriteRenderer>().sprite = mageTowerCurrentStats.mageTowerUpgradeSprite;
        GetComponent<Animator>().runtimeAnimatorController = mageTowerCurrentStats.mageTowerUpgradeAnim;
    }
}
