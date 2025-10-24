using HPC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDataManager : Singleton<LevelDataManager>
{
    public List<LevelDataStats> levels;
    [HideInInspector] public int currrentLevelDataIndex;
    [HideInInspector] public int totalStars;

    protected override void Awake()
    {
        MakeSingleton(false);
    }

    void Start()
    {
        currrentLevelDataIndex = Prefs.CurrentLevelData;
        totalStars = Prefs.TotalStarsData;
        BaseManager.Ins.UpdateTotalStatsText(totalStars);

        foreach (var level in levels)
            level.LoadProgress();
    }
}
