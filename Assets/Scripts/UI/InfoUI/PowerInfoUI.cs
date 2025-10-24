using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PowerInfoUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image powerIconImage;
    [SerializeField] private TMP_Text powerIndexText;
    [SerializeField] private GameObject descriptionObj;
    [SerializeField] private TMP_Text descriptionText;

    public void UpdatePowerInfo(Sprite powerIcon, string powerIndex, string description)
    {
        powerIconImage.sprite = powerIcon;
        powerIndexText.text = powerIndex;
        descriptionText.text = description;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        descriptionObj.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        descriptionObj.SetActive(false);
    }
}
