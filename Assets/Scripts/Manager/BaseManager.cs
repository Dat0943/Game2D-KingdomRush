using HPC;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BaseManager : Singleton<BaseManager>
{
    [SerializeField] private GameObject selectPanel, starPanel;
    [SerializeField] private Image backMenuImg, shopButtonImg, upgradeButtonImg, encyclopediaButtonImg, achivementButtonImg;
    [SerializeField] private Image descriptionPanel, background, titleImage, descriptionImage, backDescriptionButton, toStartButton;
    [SerializeField] private TMP_Text titleText, contentText, completedText;
    [SerializeField] private TMP_Text totalStarsText;
    [SerializeField] private Image[] descriptionStarImage;

    [HideInInspector] public string nextGameScene;

    [SerializeField] private RectTransform mapPanel;
    [SerializeField] private GameObject levelNodePrefab;

    protected override void Awake()
    {
        MakeSingleton(false);
    }

    void Start()
    {
        StartCoroutine(OnStartUI());
        StartCoroutine(OnStartNode());
    }

    IEnumerator OnStartUI()
    {
        LeanTweenManager.Ins.OpenDoor();
        yield return new WaitForSeconds(1f);
        OpenSelectPanel();

        StartCoroutine(FadeIn(backMenuImg));
        StartCoroutine(FadeIn(shopButtonImg));
        StartCoroutine(FadeIn(upgradeButtonImg));
        StartCoroutine(FadeIn(encyclopediaButtonImg));
        StartCoroutine(FadeIn(achivementButtonImg));

        OpenStarPanel();
    }

    void OpenSelectPanel()
    {
        LeanTween.moveLocal(selectPanel, new Vector3(0, -250f, 0), 0.5f).setEase(LeanTweenType.easeInOutCubic);
    }

    void OpenStarPanel()
    {
        LeanTween.moveLocal(starPanel, new Vector3(227f, 289f, 0), 0.5f).setEase(LeanTweenType.easeInOutCubic);
    }

    IEnumerator FadeIn(Image targetImage)
    {
        Color color = targetImage.color;

        color.a = 0f;
        targetImage.color = color;

        float elapsed = 0f;
        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsed / 0.5f); 
            targetImage.color = color;
            yield return null;
        }

        color.a = 1f;
        targetImage.color = color;
    }

    IEnumerator FadeIn(TMP_Text target)
    {
        Color color = target.color;

        color.a = 0f;
        target.color = color;

        float elapsed = 0f;
        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsed / 0.5f);
            target.color = color;
            yield return null;
        }

        color.a = 1f;
        target.color = color;
    }

    IEnumerator FadeOut(Image targetImage)
    {
        Color color = targetImage.color;

        color.a = 1f;
        targetImage.color = color;

        float elapsed = 0f;
        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(1f - (elapsed / 0.5f));
            targetImage.color = color;
            yield return null;
        }

        color.a = 0f;
        targetImage.color = color;
    }

    IEnumerator FadeOut(TMP_Text targetImage)
    {
        Color color = targetImage.color;

        color.a = 1f;
        targetImage.color = color;

        float elapsed = 0f;
        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(1f - (elapsed / 0.5f));
            targetImage.color = color;
            yield return null;
        }

        color.a = 0f;
        targetImage.color = color;
    }

    public void UpdateDescriptionUI(Sprite titleSprite, Sprite descriptionMapSprite, string content, bool isCompleted)
    {
        titleImage.sprite = titleSprite;
        descriptionImage.sprite = descriptionMapSprite;
        contentText.text = content;

        if(isCompleted)
            completedText.gameObject.SetActive(true);
        else
            completedText.gameObject.SetActive(false);
    }

    #region OnClick
    public void OnClickBackButton()
    {
        StartCoroutine(HandleBackButton());
    }

    IEnumerator HandleBackButton()
    {
        LeanTweenManager.Ins.CloseDoor();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("MenuScene");
    }

    public void OpenDescriptionPanel()
    {
        descriptionPanel.gameObject.SetActive(true);
        LeanTween.moveLocal(descriptionPanel.gameObject, Vector3.zero, 0.5f).setEase(LeanTweenType.easeInOutCubic);

        StartCoroutine(FadeIn(descriptionPanel));
        StartCoroutine(FadeIn(background));
        StartCoroutine(FadeIn(titleImage));
        StartCoroutine(FadeIn(descriptionImage));
        StartCoroutine(FadeIn(backDescriptionButton));
        StartCoroutine(FadeIn(toStartButton));
        StartCoroutine(FadeIn(titleText));
        StartCoroutine(FadeIn(contentText));

        if (completedText.gameObject.activeSelf)
            StartCoroutine(FadeIn(completedText));
    }

    public void OnClickBackDescriptionButton()
    {
        StartCoroutine(CloseDescriptionPanel());
    }

    IEnumerator CloseDescriptionPanel()
    {
        LeanTween.moveLocal(descriptionPanel.gameObject, new Vector3(0f, 180f, 0f), 0.5f).setEase(LeanTweenType.easeInOutCubic);

        StartCoroutine(FadeOut(descriptionPanel));
        StartCoroutine(FadeOut(background));
        StartCoroutine(FadeOut(titleImage));
        StartCoroutine(FadeOut(descriptionImage));
        StartCoroutine(FadeOut(backDescriptionButton));
        StartCoroutine(FadeOut(toStartButton));
        StartCoroutine(FadeOut(titleText));
        StartCoroutine(FadeOut(contentText));

        if (completedText.gameObject.activeSelf)
            StartCoroutine(FadeOut(completedText));

        yield return new WaitForSeconds(0.5f);
        descriptionPanel.gameObject.SetActive(false);
    }

    public void OnClickToBattleButton()
    {
        StartCoroutine(HandleToBattleButton());
    }

    IEnumerator HandleToBattleButton()
    {
        LeanTweenManager.Ins.CloseDoor();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(nextGameScene);
    }
    #endregion

    #region Node
    IEnumerator OnStartNode()
    {
        yield return new WaitForSeconds(1f);

        foreach (var level in LevelDataManager.Ins.levels)
        {
            if (level.isUnlocked)
                SpawnLevelNode(level);
        }
    }

    void SpawnLevelNode(LevelDataStats data)
    {
        GameObject node = Instantiate(levelNodePrefab, mapPanel);
        node.GetComponent<RectTransform>().anchoredPosition = data.mapPosition;

        LevelNode nodeScript = node.GetComponent<LevelNode>();
        nodeScript.Setup(data);
    }
    #endregion

    public void UpdateTotalStatsText(int totalStars)
    {
        totalStarsText.text = totalStars.ToString() + "/62";
    }

    public void UpdateDescriptionStarImage(int stars)
    {
        for (int i = 0; i < descriptionStarImage.Length; i++)
        {
            Image img = descriptionStarImage[i];

            if (i < stars)
                img.color = Color.white;
            else
                img.color = Color.black;
        }
    }
}
