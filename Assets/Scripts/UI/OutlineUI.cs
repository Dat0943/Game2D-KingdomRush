using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Coffee.UIEffects;
using UnityEngine.EventSystems;

public class OutlineUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    UIEffect effect;

    void Awake()
    {
        effect = GetComponent<UIEffect>();
    }

    void OnEnable()
    {
        SetGlowFade(0f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetGlowFade(1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetGlowFade(0f);
    }

    void SetGlowFade(float value)
    {
        if (effect != null)
            effect.shadowFade = Mathf.Clamp01(value);
    }
}
