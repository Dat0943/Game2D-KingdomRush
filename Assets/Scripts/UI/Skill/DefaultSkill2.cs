using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultSkill2 : DefaultSkill
{
    [Header("Skill 2")]
    [SerializeField] private DefaultSkill2Stats stats;

    [SerializeField] private GameObject reinforcementPrefabA;
    [SerializeField] private GameObject reinforcementPrefabB;

    protected override void Start()
    {
        base.Start();

        skillCooldown = stats.coolDown;
    }

    protected override void UseSkill(Vector3 position)
    {
        GameObject prefabA = Random.value > 0.5f ? reinforcementPrefabA : reinforcementPrefabB;
        GameObject prefabB = Random.value > 0.5f ? reinforcementPrefabA : reinforcementPrefabB;

        Vector3 offsetA = Random.value > 0.5f ? new Vector3(-0.15f, 0.15f, 0f) : new Vector3(0.15f, 0.15f, 0f);
        Vector3 offsetB = Random.value > 0.5f ? new Vector3(0.15f, -0.15f, 0f) : new Vector3(-0.15f, -0.15f, 0f);

        GameObject prefabAClone = Instantiate(prefabA, position + offsetA, Quaternion.identity);
        prefabAClone.GetComponent<Reinforcement>().originalPos = position + offsetA;
        int nameReinforcementAIndex = Random.Range(0, TowerManager.Ins.nameReinforcements.Count);
        prefabAClone.GetComponent<Reinforcement>().nameReinforcement = TowerManager.Ins.nameReinforcements[nameReinforcementAIndex];
        TowerManager.Ins.nameReinforcements.RemoveAt(nameReinforcementAIndex);
        prefabAClone.GetComponent<Reinforcement>().reinforcementStats = stats;

        GameObject prefabBClone = Instantiate(prefabB, position + offsetB, Quaternion.identity);
        prefabBClone.GetComponent<Reinforcement>().originalPos = position + offsetB;
        int nameReinforcementBIndex = Random.Range(0, TowerManager.Ins.nameReinforcements.Count);
        prefabBClone.GetComponent<Reinforcement>().nameReinforcement = TowerManager.Ins.nameReinforcements[nameReinforcementBIndex];
        TowerManager.Ins.nameReinforcements.RemoveAt(nameReinforcementBIndex);
        prefabBClone.GetComponent<Reinforcement>().reinforcementStats = stats;
    }
}
