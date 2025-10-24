using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "KR/LevelData")]
public class LevelDataStats : ScriptableObject
{
    public int index;
    public string levelID;
    public string levelName;
    public string gameScene;

    public Sprite currentSprite;
    public Sprite highlightSprite;
    public Sprite completedCurrentSprite;
    public Sprite completedHighlightSprite;

    public Sprite titleSprite;
    public Sprite descriptionMapSprite;
    [TextArea(3, 10)]
    public string content;

    public Vector2 mapPosition; 

    public bool isUnlocked;
    public bool isCompleted;
    public int stars;

    string GetKey(string key) => $"{levelID}_{key}";

    public void SaveProgress()
    {
        PlayerPrefs.SetInt(GetKey("Unlocked"), isUnlocked ? 1 : 0);
        PlayerPrefs.SetInt(GetKey("Completed"), isCompleted ? 1 : 0);
        PlayerPrefs.SetInt(GetKey("Stars"), stars);
        PlayerPrefs.Save();
    }

    public void LoadProgress()
    {
        isUnlocked = PlayerPrefs.GetInt(GetKey("Unlocked"), isUnlocked ? 1 : 0) == 1;
        isCompleted = PlayerPrefs.GetInt(GetKey("Completed"), 0) == 1;
        stars = PlayerPrefs.GetInt(GetKey("Stars"), stars);
    }
}
