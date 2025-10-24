using HPC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject titlePanel, startPanel, creditsPanel;
    [SerializeField] private GameObject creditsDialog;

    void Start()
    {
        StartCoroutine(OnStart());
    }

    IEnumerator OnStart()
    {
        LeanTweenManager.Ins.OpenDoor();
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(ShowUI());
    }

    IEnumerator ShowUI()
    {
        OpenPanel(titlePanel, 116f);
        yield return new WaitForSeconds(0.5f);
        OpenPanel(startPanel, -179f);
        yield return new WaitForSeconds(0.5f);
        OpenPanel(creditsPanel, 0f);
    }

    IEnumerator CloseUI()
    {
        ClosePanel(creditsPanel, 70f);
        yield return new WaitForSeconds(0.3f);
        ClosePanel(startPanel, 13f);
        yield return new WaitForSeconds(0.3f);
        ClosePanel(titlePanel, 454f);
    }

    public void OpenPanel(GameObject panel, float yOpen)
    {
        LeanTween.moveLocal(panel, new Vector3(0, yOpen, 0), 0.5f).setEase(LeanTweenType.easeOutBack);
    }

    public void ClosePanel(GameObject panel, float yClose)
    {
        LeanTween.moveLocal(panel, new Vector3(0, yClose, 0), 0.3f).setEase(LeanTweenType.easeInBack);
    }

    public void OnClickCreditsButton()
    {
        StartCoroutine(HandleCreditsButton());
    }

    IEnumerator HandleCreditsButton()
    {
        StartCoroutine(CloseUI());
        yield return new WaitForSeconds(0.5f);
        LeanTweenManager.Ins.CloseDoor();
        yield return new WaitForSeconds(1f);
        creditsDialog.SetActive(true);
        LeanTweenManager.Ins.OpenDoor();
    }

    public void OnClickBackButton()
    {
        StartCoroutine(HandleBackButton());
    }

    IEnumerator HandleBackButton()
    {
        LeanTweenManager.Ins.CloseDoor();
        yield return new WaitForSeconds(1f);
        creditsDialog.SetActive(false);
        LeanTweenManager.Ins.OpenDoor();
        StartCoroutine(ShowUI());
    }

    public void OnClickStartButton()
    {
        StartCoroutine(HandleStartButton());
    }

    IEnumerator HandleStartButton()
    {
        StartCoroutine(CloseUI());
        yield return new WaitForSeconds(0.5f);
        LeanTweenManager.Ins.CloseDoor();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("BaseScene");
    }
}
