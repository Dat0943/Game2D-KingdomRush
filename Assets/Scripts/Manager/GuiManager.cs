using HPC;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuiManager : Singleton<GuiManager>
{
    public GameObject powerInfoContainer;
    public TMP_Text nameInfoText;
    [SerializeField] private Image iconInfoImage;

    [Header("Rally Flag")]
    [SerializeField] private GameObject rallyFlag;
    [SerializeField] private GameObject bgRallyFlag;
    [SerializeField] private Image cooldownRallyImage;

    [Header("Start Battle")]
    [SerializeField] private GameObject startBattle;
    [SerializeField] private GameObject bgStartBattle;
    bool isStartBattle;

    [Header("Gui")]
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text healthText;

    [SerializeField] private GameObject pausePanel;

    [Header("Instruction")]
    [SerializeField] private GameObject instruction;
    [SerializeField] private GameObject bgInstruction;

    [Header("Victory")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject bgVictoryPanel;
    [SerializeField] private GameObject continuePanel;
    [SerializeField] private GameObject restartPanel;

    [Header("Defeat")]
    [SerializeField] private GameObject defeatPanel;

    [SerializeField] private TMP_Text goldText;

    protected override void Awake()
    {
        MakeSingleton(false);
    }

    void Start()
    {
        StartCoroutine(OnStart());
    }

    IEnumerator OnStart()
    {
        LeanTweenManager.Ins.OpenDoor();
        yield return new WaitForSeconds(1.5f);
        ShowInstruction();
    }

    public void ShowNameInfo(Sprite icon, string nameInfo)
    {
        iconInfoImage.sprite = icon;
        nameInfoText.text = nameInfo;
    }

    #region Instruction
    void ShowInstruction()
    {
        instruction.SetActive(true);
        LeanTweenManager.Ins.OpenInstructionDialog(bgInstruction);
    }

    public void HideInstruction()
    {
        StartCoroutine(HideInstructionIE());
    }

    IEnumerator HideInstructionIE()
    {
        LeanTweenManager.Ins.CloseInstructionDialog(bgInstruction);
        yield return new WaitForSeconds(0.5f);
        
        GameManager.Ins.isStartGame = true;

        instruction.SetActive(false);
        ShowRallyFlag();
        StartCoroutine(ShowStartBattleImage());
        LeanTweenManager.Ins.OpenSupportPanel();
        LeanTweenManager.Ins.OpenHUDPanel();
        LeanTweenManager.Ins.OpenPauseButton();
    }
    #endregion

    #region Rally
    public void ShowRallyFlag()
    {
        rallyFlag.SetActive(true);
        LeanTweenManager.Ins.OpenDialog(bgRallyFlag);
    }

    public IEnumerator HideRallyFlag()
    {
        LeanTweenManager.Ins.CloseDialog(bgRallyFlag);
        yield return new WaitForSeconds(0.2f);
        rallyFlag.SetActive(false);
    }

    public IEnumerator CooldownRoutine(float cooldown)
    {
        cooldownRallyImage.fillAmount = 1f;
        float elapsed = 0f;

        while (elapsed < cooldown)
        {
            elapsed += Time.deltaTime;
            cooldownRallyImage.fillAmount = 1f - (elapsed / cooldown);
            yield return null;
        }

        cooldownRallyImage.fillAmount = 0f;
    }
    #endregion

    #region Start Battle
    public IEnumerator ShowStartBattleImage()
    {
        if (startBattle)
        {
            startBattle.SetActive(true);
            LeanTweenManager.Ins.OpenDialog(bgStartBattle);
            yield return new WaitForSeconds(0.5f);
            bgStartBattle.GetComponent<Animator>().enabled = true;
        }
    }

    public IEnumerator HideStartBattleImage()
    {
        if(startBattle)
        {
            bgStartBattle.GetComponent<Animator>().enabled = false;
            LeanTweenManager.Ins.CloseDialog(bgStartBattle);
            yield return new WaitForSeconds(0.2f);
            startBattle.SetActive(false);
        }
    }
    #endregion

    #region HUD
    public void UpdateWaveText(int currentWave, int numOfWave)
    {
        if (waveText != null)
            waveText.text = "WAVE " + currentWave + "/" + numOfWave;
    }

    public void UpdateHealthText(int health)
    {
        if(healthText != null)
            healthText.text = health.ToString();
    }

    public void UpdateGoldInGame(int gold)
    {
        if (goldText != null)
            goldText.text = gold.ToString();
    }
    #endregion

    #region Pause Panel
    public IEnumerator ShowPausePanel()
    {
        pausePanel.SetActive(true);
        LeanTweenManager.Ins.OpenPausePanel(pausePanel);
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 0;
    }

    public IEnumerator HidePausePanel()
    {
        Time.timeScale = 1;
        LeanTweenManager.Ins.ClosePausePanel(pausePanel);
        yield return new WaitForSeconds(0.2f);
        pausePanel.SetActive(false);
    }
    #endregion

    #region Victory Panel
    public IEnumerator ShowVictoryPanel()
    {
        victoryPanel.SetActive(true);
        LeanTweenManager.Ins.OpenDialog(bgVictoryPanel);
        yield return new WaitForSeconds(0.5f);
        LeanTweenManager.Ins.OpenContinuePanel(continuePanel);
        yield return new WaitForSeconds(0.5f);
        LeanTweenManager.Ins.OpenRestartPanel(restartPanel);
    }

    public IEnumerator ShowDefeatPanel()
    {
        defeatPanel.SetActive(true);
        LeanTweenManager.Ins.OpenDefeatPanel(defeatPanel);
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 0;
    }
    #endregion
}
