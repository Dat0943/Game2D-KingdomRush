using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class DefaultSkill : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject skillButton;
    [SerializeField] private GameObject descriptionSkill;
    [SerializeField] private Animator bgAnim;
    [SerializeField] private Animator doorAnim;
    bool isOpening;

    [SerializeField] private GameObject cursor;
    [SerializeField] private Sprite defaultSkillSprite;
    [SerializeField] private Sprite selectedSkillSprite;
    [SerializeField] private Sprite cursorError;
    [SerializeField] private Sprite cursorDefault;
    [SerializeField] private Image image;
    GameObject cursorClone;
    bool isSelecting;

    static DefaultSkill currentSelected;

    [SerializeField] private float defaultRotateSpeed;
    [SerializeField] private Vector3 defaultPos, defaultScale;
    [SerializeField] private Quaternion defaultRot;
    [SerializeField] private LayerMask battlefieldLayer;

    [Header("Skill Settings")]
    [SerializeField] private Image cooldownImage;
    protected float skillCooldown;
    float nextSkillTime;

    protected virtual void Start()
    {
        StartCoroutine(ShowSkillButton());
        
        cooldownImage.enabled = false;
    }

    void Update()
    {
        if (!isOpening) return;

        if(isSelecting && Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;

            Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos, battlefieldLayer);

            if (hit != null)
            {
                UseSkill(mouseWorldPos);

                nextSkillTime = Time.time + skillCooldown;
                StartCoroutine(CooldownRoutine());

                Deselect();
                if (cursorClone != null)
                {
                    Destroy(cursorClone);
                    cursorClone = null;
                }
            }
            else
            {
                StartCoroutine(Error());
            }
        }  
    }

    protected abstract void UseSkill(Vector3 position);

    IEnumerator Error()
    {
        SpriteRenderer cursorSpr = cursorClone.GetComponentInChildren<SpriteRenderer>();

        cursorSpr.sprite = cursorError;
        cursorSpr.transform.localPosition = Vector3.zero;
        cursorSpr.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        cursorSpr.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        cursorClone.GetComponent<ControlCircle>().rotateSpeed = 0f;
        cursorClone.GetComponent<ControlCircle>().transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        yield return new WaitForSeconds(0.3f);
        if (cursorSpr == null) yield break;

        cursorSpr.sprite = cursorDefault;
        cursorSpr.transform.localPosition = defaultPos;
        cursorSpr.transform.localRotation = defaultRot;
        cursorSpr.transform.localScale = defaultScale;
        cursorClone.GetComponent<ControlCircle>().rotateSpeed = defaultRotateSpeed;
    }

    public IEnumerator ShowSkillButton()
    {
        bgAnim.Play("BG");
        doorAnim.Play("Open");
        skillButton.SetActive(true);

        yield return new WaitForSeconds(0.6f);

        isOpening = true;
        bgAnim.gameObject.SetActive(false);
        doorAnim.gameObject.SetActive(false);
    }

    public void OnClickButton()
    {
        if (!isOpening) return;

        if(Time.time >= nextSkillTime)
        {
            if (!isSelecting)
            {
                if (currentSelected != null && currentSelected != this)
                {
                    if (currentSelected.cursorClone != null)
                    {
                        Destroy(currentSelected.cursorClone);
                        currentSelected.cursorClone = null;
                    }
                    currentSelected.Deselect();
                }
                else
                {
                    image.sprite = selectedSkillSprite;
                    isSelecting = true;
                    currentSelected = this;

                    cursorClone = Instantiate(cursor);
                }
            }
            else
            {
                Deselect();
                if (cursorClone != null)
                {
                    Destroy(cursorClone);
                    cursorClone = null;
                }
            }   
        }
    }

    IEnumerator CooldownRoutine()
    {
        cooldownImage.enabled = true;
        cooldownImage.fillAmount = 1f; 
        float elapsed = 0f;

        while (elapsed < skillCooldown)
        {
            elapsed += Time.deltaTime;
            cooldownImage.fillAmount = 1f - (elapsed / skillCooldown);
            yield return null;
        }

        cooldownImage.fillAmount = 0f;
        cooldownImage.enabled = false;
    }

    void Deselect()
    {
        image.sprite = defaultSkillSprite;
        isSelecting = false;
        currentSelected = null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(isOpening)
            descriptionSkill.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isOpening)
            descriptionSkill.SetActive(false);
    }
}
