using HPC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Wave
{
    public List<SpawnGroup> spawnGroups = new List<SpawnGroup>();
}

[System.Serializable]
public class SpawnGroup
{
    public string groupEnemyName;
    public GameObject enemyPrefab;
    public int count;
    public float spawnInterval; 
    public float delayAfterGroup;
}

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private float minStartDelayBetweenWave;
    [SerializeField] private float maxStartDelayBetweenWave;
    [SerializeField] private Transform spawnPoint;

    public List<Wave> waves = new List<Wave>();
    public List<Enemy> activeEnemies = new List<Enemy>();

    [HideInInspector] public int currentWaveIndex;
    int numOfEnemy;
    [HideInInspector] public int currentIdEnemy;
    [HideInInspector] public string enemyStatistic;

    Coroutine waveRoutine;
    Coroutine cooldownRoutine;
    bool isWaitingCooldown = false;

    [Header("GUI In Game")]
    [SerializeField] private int goldStartInGame;
    [HideInInspector] public int currentGoldInGame;
    [SerializeField] private int healthInGame = 20;
    [HideInInspector] public int currentHealthInGame;
    [HideInInspector] public bool isCalledEnemy;
    [HideInInspector] public bool isStartGame;
    [HideInInspector] public bool isEndGame;

    [Header("Star")]
    [SerializeField] private GameObject starPrefab;
    [SerializeField] private RectTransform starPanel;

    public GameObject smokePrefab;

    protected override void Awake()
    {
        MakeSingleton(false);
    }

    void Start()
    {
        currentGoldInGame = goldStartInGame;
        currentHealthInGame = healthInGame;
        enemyStatistic = MonsterCountStatistics(waves[currentWaveIndex]);
        GuiManager.Ins.UpdateWaveText(currentWaveIndex, waves.Count);
        GuiManager.Ins.UpdateHealthText(currentHealthInGame);
        GuiManager.Ins.UpdateGoldInGame(currentGoldInGame);
    }

    #region Wave
    IEnumerator RunWaves()
    {
        while (currentWaveIndex < waves.Count)
        {
            currentWaveIndex++;
            GuiManager.Ins.UpdateWaveText(currentWaveIndex, waves.Count);
            yield return StartCoroutine(SpawnWave(waves[currentWaveIndex - 1]));

            if (currentWaveIndex < waves.Count) 
            {
                enemyStatistic = MonsterCountStatistics(waves[currentWaveIndex]);

                GuiManager.Ins.ShowRallyFlag();
                isWaitingCooldown = true;

                cooldownRoutine = StartCoroutine(CooldownAndNextWave());
                yield return cooldownRoutine; 
            }
        }

        yield return new WaitUntil(() => activeEnemies.Count == 0);
        yield return new WaitForSeconds(3f);

        WinGame();
    }

    IEnumerator CooldownAndNextWave()
    {
        float startDelayBetweenWave = Random.Range(minStartDelayBetweenWave, maxStartDelayBetweenWave);
        StartCoroutine(GuiManager.Ins.CooldownRoutine(startDelayBetweenWave));
        yield return new WaitForSeconds(startDelayBetweenWave);

        StartCoroutine(GuiManager.Ins.HideRallyFlag());
        isWaitingCooldown = false;
    }

    IEnumerator SpawnWave(Wave wave)
    {
        foreach (var group in wave.spawnGroups)
        {
            for (int i = 0; i < group.count; i++)
            {
                GameObject enemy = Instantiate(group.enemyPrefab, spawnPoint.position, Quaternion.identity);
                enemy.GetComponent<Enemy>().idEnemy = numOfEnemy++;
                yield return new WaitForSeconds(group.spawnInterval);
            }
            yield return new WaitForSeconds(group.delayAfterGroup);
        }
    }

    string MonsterCountStatistics(Wave wave)
    {
        Dictionary<string, int> enemyCounts = new Dictionary<string, int>();

        foreach (var group in wave.spawnGroups)
        {
            if (enemyCounts.ContainsKey(group.groupEnemyName))
            {
                enemyCounts[group.groupEnemyName] += group.count;
            }
            else
            {
                enemyCounts[group.groupEnemyName] = group.count;
            }
        }

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (var kvp in enemyCounts)
        {
            sb.AppendLine($"{kvp.Key} x {kvp.Value}");
        }

        string result = sb.ToString();
        return result;
    }
    #endregion

    #region Enemy
    public void RegisterEnemy(Enemy enemy)
    {
        if (!activeEnemies.Contains(enemy))
            activeEnemies.Add(enemy);
    }

    public void UnregisterEnemy(Enemy enemy)
    {
        activeEnemies.Remove(enemy);
    }
    #endregion

    public void CallMonstersToAppear()
    {
        isCalledEnemy = true;

        if (isWaitingCooldown)
        {
            StopCoroutine(cooldownRoutine);

            isWaitingCooldown = false;
            StartCoroutine(GuiManager.Ins.HideRallyFlag());

            waveRoutine = StartCoroutine(RunWaves());
        }
        else
        {
            if (waveRoutine == null)
            {
                StartCoroutine(GuiManager.Ins.HideRallyFlag());

                waveRoutine = StartCoroutine(RunWaves());
            }
        }
    }

    public void TakeDamage()
    {
        if (isEndGame) return;

        currentHealthInGame--;
        currentHealthInGame = Mathf.Max(currentHealthInGame, 0);

        GuiManager.Ins.UpdateHealthText(currentHealthInGame);

        if (currentHealthInGame <= 0)
        {
            EndGame();
        }
    }

    void EndGame()
    {
        isEndGame = true;
        StartCoroutine(GuiManager.Ins.ShowDefeatPanel());
    }

    void WinGame()
    {
        var levels = LevelDataManager.Ins.levels;
        int index = LevelDataManager.Ins.currrentLevelDataIndex;

        // Hoàn thành level hiện tại
        var currentLevel = levels[index];
        int newStars = CalculateStars();

        if (!currentLevel.isCompleted)
            currentLevel.isCompleted = true;

        if (currentLevel.stars < newStars)
        {
            LevelDataManager.Ins.totalStars = LevelDataManager.Ins.totalStars - currentLevel.stars + newStars;
            currentLevel.stars = newStars;
            Prefs.TotalStarsData = LevelDataManager.Ins.totalStars;
        }    

        currentLevel.SaveProgress();

        // Mở khóa level kế tiếp
        if (index + 1 < levels.Count)
        {
            var nextLevel = levels[index + 1];
            if (!nextLevel.isUnlocked)
            {
                nextLevel.isUnlocked = true;
                nextLevel.SaveProgress();
            }
        }

        StartCoroutine(GuiManager.Ins.ShowVictoryPanel());
        SpawnStars();
    }

    void SpawnStars()
    {
        int countStars = CalculateStars();
        for (int i = 0; i < countStars; i++)
        {
            GameObject star = Instantiate(starPrefab, starPanel);
        }
    }

    int CalculateStars()
    {
        if (currentHealthInGame >= 18)
            return 3;
        else if (currentHealthInGame >= 6)
            return 2;
        else if (currentHealthInGame >= 1)
            return 1;
        else
            return 0; 
    }

    #region OnClick
    public void PauseGame()
    {
        LeanTweenManager.Ins.CloseSupportPanel();
        LeanTweenManager.Ins.CloseHUDPanel();
        LeanTweenManager.Ins.ClosePauseButton();
        StartCoroutine(GuiManager.Ins.HideRallyFlag());
        if (!isCalledEnemy)
            StartCoroutine(GuiManager.Ins.HideStartBattleImage());
        StartCoroutine(GuiManager.Ins.ShowPausePanel());
    }

    public void ResumeGame()
    {
        LeanTweenManager.Ins.OpenSupportPanel();
        LeanTweenManager.Ins.OpenHUDPanel();
        LeanTweenManager.Ins.OpenPauseButton();
        if(isWaitingCooldown || (!isWaitingCooldown && !isCalledEnemy))
            GuiManager.Ins.ShowRallyFlag();
        if (!isCalledEnemy)
            StartCoroutine(GuiManager.Ins.ShowStartBattleImage());
        StartCoroutine(GuiManager.Ins.HidePausePanel());
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        StartCoroutine(HandleRestartGame());
    }

    IEnumerator HandleRestartGame()
    {
        LeanTweenManager.Ins.CloseDoor();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Time.timeScale = 1;
        StartCoroutine(HandleQuitGame());
    }

    IEnumerator HandleQuitGame()
    {
        LeanTweenManager.Ins.CloseDoor();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("BaseScene");
    }

    public void ContinueGame()
    {
        Time.timeScale = 1;
        StartCoroutine(HandleContinueGame());
    }

    IEnumerator HandleContinueGame()
    {
        LeanTweenManager.Ins.CloseDoor();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    #endregion
}
