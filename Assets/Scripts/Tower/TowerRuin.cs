using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SpriteGlow;

public class TowerRuin : MonoBehaviour
{
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private Slider progressBar; 
    [SerializeField] private float buildTime = 3f;
    [SerializeField] private float offset;

    SpriteGlowEffect glow;

    float timer = 0f;
    [HideInInspector] public GameObject currentSpot;

    void Awake()
    {
        glow = GetComponent<SpriteGlowEffect>();
    }

    void Start()
    {
        progressBar.value = 0f;
        if (glow != null)
            glow.OutlineWidth = 0;
    }

    void Update()
    {
        timer += Time.deltaTime;
        progressBar.value = timer / buildTime;

        if (timer >= buildTime)
        {
            BuildComplete();
        }
    }

    void OnMouseEnter()
    {
        if (glow != null)
            glow.OutlineWidth = 1;
    }

    void OnMouseExit()
    {
        if (glow != null)
            glow.OutlineWidth = 0;
    }

    void BuildComplete()
    {
        GameObject towerClone = Instantiate(towerPrefab, new Vector3(transform.position.x, transform.position.y - offset, transform.position.z), transform.rotation);
        towerClone.GetComponent<Tower>().currentSpot = currentSpot;
        Instantiate(GameManager.Ins.smokePrefab, new Vector3(transform.position.x, transform.position.y - offset - 0.15f, transform.position.z), Quaternion.identity);
        Destroy(gameObject); 
    }
}
