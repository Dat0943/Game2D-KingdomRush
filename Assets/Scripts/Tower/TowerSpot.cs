using SpriteGlow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSpot : MonoBehaviour
{
    public SpriteGlowEffect glow;

    [HideInInspector] public bool isShowMenu;

    void Start()
    {
        if (glow != null)
            glow.OutlineWidth = 0;
    }

    void OnMouseDown()
    {
        if (!GameManager.Ins.isStartGame) return;
        if (TowerManager.Ins.towerIsOpenMenu) return;
        if (TowerManager.Ins.barrackTowerIsActive) return;

        TowerManager.Ins.ShowMenu(this, transform.position);
    }

    void OnMouseEnter()
    {
        if (!GameManager.Ins.isStartGame) return;
        if (TowerManager.Ins.towerIsOpenMenu) return;
        if (TowerManager.Ins.barrackTowerIsActive) return;

        if (glow != null)
            glow.OutlineWidth = 1;

    }

    void OnMouseExit()
    {
        if (!GameManager.Ins.isStartGame) return;
        if (TowerManager.Ins.barrackTowerIsActive) return;

        if (isShowMenu) return;

        if (glow != null)
            glow.OutlineWidth = 0;
    }

    public void BuildTower(GameObject ruinPrefab, float offset)
    {
        StartCoroutine(TowerManager.Ins.HideMenu());
        gameObject.SetActive(false);
        GameObject ruinClone = Instantiate(ruinPrefab, new Vector3(transform.position.x, transform.position.y - offset, 0f), Quaternion.identity);
        ruinClone.GetComponent<TowerRuin>().currentSpot = gameObject;
    }
}
