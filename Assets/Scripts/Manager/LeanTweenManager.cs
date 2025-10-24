using HPC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeanTweenManager : Singleton<LeanTweenManager>
{
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private GameObject supportPanel;
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject leftDoor, rightDoor;

    protected override void Awake()
    {
        MakeSingleton(false);
    }

    public void OpenDialog(GameObject dialog)
    {
        LeanTween.scale(dialog, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutElastic);
    }

    public void CloseDialog(GameObject background)
    {
        LeanTween.scale(background, Vector3.zero, 0.2f).setEase(LeanTweenType.easeInQuad);
    }

    public void OpenInstructionDialog(GameObject dialog)
    {
        LeanTween.scale(dialog, Vector3.one, 1f).setEase(LeanTweenType.easeOutElastic);
    }

    public void CloseInstructionDialog(GameObject background)
    {
        LeanTween.scale(background, Vector3.zero, 0.5f).setEase(LeanTweenType.easeInQuad);
    }

    public void OpenInfoPanel()
    {
        LeanTween.moveLocal(infoPanel, new Vector3(47f, -279f, 0), 0.2f).setEase(LeanTweenType.easeInOutCubic);
    }

    public void CloseInfoPanel()
    {
        LeanTween.moveLocal(infoPanel, new Vector3(47f, -331f, 0), 0.5f).setEase(LeanTweenType.easeInOutCubic);
    }

    public void OpenSupportPanel()
    {
        LeanTween.moveLocal(supportPanel, new Vector3(0, -285f, 0), 0.2f).setEase(LeanTweenType.easeInOutCubic);
    }

    public void CloseSupportPanel()
    {
        LeanTween.moveLocal(supportPanel, new Vector3(0, -342f, 0), 0.5f).setEase(LeanTweenType.easeInOutCubic);
    }

    public void OpenHUDPanel()
    {
        LeanTween.moveLocal(hudPanel, new Vector3(-272f, 255f, 0), 0.2f).setEase(LeanTweenType.easeInOutCubic);
    }

    public void CloseHUDPanel()
    {
        LeanTween.moveLocal(hudPanel, new Vector3(-272f, 327f, 0), 0.5f).setEase(LeanTweenType.easeInOutCubic);
    }

    public void OpenPauseButton()
    {
        LeanTween.moveLocal(pauseButton, new Vector3(325f, 276f, 0), 0.2f).setEase(LeanTweenType.easeInOutCubic);
    }

    public void ClosePauseButton()
    {
        LeanTween.moveLocal(pauseButton, new Vector3(325f, 333f, 0), 0.5f).setEase(LeanTweenType.easeInOutCubic);
    }

    public void OpenPausePanel(GameObject pausePanel)
    {
        LeanTween.moveLocal(pausePanel, new Vector3(0, 116f, 0), 0.2f).setEase(LeanTweenType.easeInOutCubic);
    }

    public void ClosePausePanel(GameObject pausePanel)
    {
        LeanTween.moveLocal(pausePanel, new Vector3(0, 457f, 0), 0.5f).setEase(LeanTweenType.easeInOutCubic);
    }

    public void OpenDoor()
    {
        LeanTween.moveLocal(leftDoor, new Vector3(-600f, 0f, 0f), 0.5f).setEase(LeanTweenType.easeInOutCubic);
        LeanTween.moveLocal(rightDoor, new Vector3(600f, 0f, 0f), 0.5f).setEase(LeanTweenType.easeInOutCubic);
    }

    public void CloseDoor()
    {
        LeanTween.moveLocal(leftDoor, new Vector3(-175f, 0f, 0f), 0.5f).setEase(LeanTweenType.easeOutBounce);
        LeanTween.moveLocal(rightDoor, new Vector3(175f, 0f, 0f), 0.5f).setEase(LeanTweenType.easeOutBounce);
    }

    public void OpenContinuePanel(GameObject continuePanel)
    {
        LeanTween.moveLocal(continuePanel, new Vector3(0, -116f, 0), 0.5f).setEase(LeanTweenType.easeInOutCubic);
    }

    public void OpenRestartPanel(GameObject restartPanel)
    {
        LeanTween.moveLocal(restartPanel, new Vector3(0, 18f, 0), 0.5f).setEase(LeanTweenType.easeInOutCubic);
    }

    public void OpenDefeatPanel(GameObject defeatPanel)
    {
        LeanTween.moveLocal(defeatPanel, new Vector3(0, 60f, 0), 0.5f).setEase(LeanTweenType.easeInOutCubic);
    }
}
