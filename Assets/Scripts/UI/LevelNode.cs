using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelNode : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private GameObject[] starImages;
    [SerializeField] private GameObject starPanel;
    int index;

    LevelDataStats data;
    SpriteState spriteState;

    void Start()
    {
        spriteState = button.spriteState;
        if (data.isCompleted)
        {
            button.GetComponent<Image>().sprite = data.completedCurrentSprite;
            spriteState.highlightedSprite = data.completedHighlightSprite;
            spriteState.pressedSprite = data.completedHighlightSprite;
            spriteState.selectedSprite = data.completedCurrentSprite;
            spriteState.disabledSprite = data.completedCurrentSprite;
            UpdateStarImage();
        }
        else
        {
            button.GetComponent<Image>().sprite = data.currentSprite;
            spriteState.highlightedSprite = data.highlightSprite;
            spriteState.pressedSprite = data.highlightSprite;
            spriteState.selectedSprite = data.currentSprite;
            spriteState.disabledSprite = data.currentSprite;
        }
        button.spriteState = spriteState;
    }

    public void Setup(LevelDataStats levelData)
    {
        data = levelData;
        index = levelData.index;

        button.onClick.AddListener(OnNodeClicked);
    }

    void OnNodeClicked()
    {
        BaseManager.Ins.nextGameScene = data.gameScene;
        BaseManager.Ins.UpdateDescriptionUI(data.titleSprite, data.descriptionMapSprite, data.content, data.isCompleted);

        BaseManager.Ins.OpenDescriptionPanel();

        BaseManager.Ins.UpdateDescriptionStarImage(data.stars);

        LevelDataManager.Ins.currrentLevelDataIndex = index;
        Prefs.CurrentLevelData = LevelDataManager.Ins.currrentLevelDataIndex;
    }

    void UpdateStarImage()
    {
        starPanel.SetActive(true);

        for (int i = 0; i < starImages.Length; i++)
        {
            Image img = starImages[i].GetComponent<Image>();

            if (i < data.stars)
                img.color = Color.white; 
            else
                img.color = Color.red;
        }
    }
}
