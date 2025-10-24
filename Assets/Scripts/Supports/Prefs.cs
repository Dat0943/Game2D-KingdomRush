using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prefs : MonoBehaviour
{
    public static int CurrentLevelData
    {
        set => PlayerPrefs.SetInt("CurrentLevelIndex", value);
        get => PlayerPrefs.GetInt("CurrentLevelIndex", 0);
    }

    public static int TotalStarsData
    {
        set => PlayerPrefs.SetInt("TotalStars", value);
        get => PlayerPrefs.GetInt("TotalStars", 0);
    }
}
