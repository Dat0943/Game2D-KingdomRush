using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using SpriteGlow;

[System.Serializable]
public class Power
{
    public Sprite powerIcon;
    public string powerIndex;
    public string description;
}

public class InfoUI : MonoBehaviour
{
    [SerializeField] private GameObject powerInfoUI;
    public Sprite icon;
    public string nameInfo;
    public Power[] power;

    public static InfoUI currentSelected;
    public virtual GameObject GetSelectionCircle() => null;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            if (hit.collider == null || hit.collider.GetComponent<InfoUI>() == null)
            {
                CloseInfoUI();
            }
        }
    }

    public virtual void OnMouseDown()
    {
        if (currentSelected != null && currentSelected != this)
        {
            var oldCircle = currentSelected.GetSelectionCircle();
            if (oldCircle != null) oldCircle.SetActive(false);
        }

        currentSelected = this;
        var circle = GetSelectionCircle();
        if (circle != null) circle.SetActive(true);

        UpdateInfoUI();
        GuiManager.Ins.ShowNameInfo(icon, nameInfo);
        LeanTweenManager.Ins.OpenInfoPanel();
    }

    public void UpdateInfoUI()
    {
        if (GuiManager.Ins.powerInfoContainer.transform.childCount > 0)
        {
            foreach (Transform child in GuiManager.Ins.powerInfoContainer.transform)
            {
                Destroy(child.gameObject);
            }
        }

        for (int i = 0; i < power.Length; i++)
        {
            GameObject powerInfoUIClone = Instantiate(powerInfoUI);
            powerInfoUIClone.GetComponent<PowerInfoUI>().UpdatePowerInfo(power[i].powerIcon, power[i].powerIndex, power[i].description);
            powerInfoUIClone.transform.SetParent(GuiManager.Ins.powerInfoContainer.transform);
            powerInfoUIClone.transform.localScale = Vector3.one;
        }
    }

    public virtual void CloseInfoUI()
    {
        LeanTweenManager.Ins.CloseInfoPanel();

        var circle = GetSelectionCircle();
        if (circle != null) circle.SetActive(false);
    }
}
