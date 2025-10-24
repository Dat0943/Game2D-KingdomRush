using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class DefaultSkill1 : DefaultSkill
{
    [Header("Skill 1")]
    [SerializeField] private DefaultSkill1Stats stats;

    [Header("Meteor Prefabs")]
    [SerializeField] private GameObject meteorPrefab;      
    [SerializeField] private GameObject explosionPrefab; 
    [SerializeField] private GameObject scorchPrefab;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnHeight = 10f;      
    [SerializeField] private float impactDelay = 0.5f;
    [SerializeField] private float damageRadius;

    protected override void Start()
    {
        base.Start();
        skillCooldown = stats.coolDown;
    }

    protected override void UseSkill(Vector3 position)
    {
        StartCoroutine(SpawnMeteors(position));
    }

    IEnumerator SpawnMeteors(Vector3 targetPos)
    {
        int meteorCount = stats.numberOfMeteors;

        for (int i = 0; i < meteorCount; i++)
        {
            Vector3 spawnOffset = new Vector3(Random.Range(-0.35f, 0.35f), Random.Range(-0.35f, 0.35f), 0f);
            Vector3 spawnPos = targetPos + spawnOffset + Vector3.up * spawnHeight;

            GameObject meteor = Instantiate(meteorPrefab, spawnPos, Quaternion.Euler(0f, 0f, -90f));

            StartCoroutine(MeteorFall(meteor, targetPos + spawnOffset));

            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator MeteorFall(GameObject meteor, Vector3 targetPos)
    {
        Vector3 startPos = meteor.transform.position;
        float elapsed = 0f;

        while (elapsed < impactDelay)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / impactDelay;

            meteor.transform.position = Vector3.Lerp(startPos, targetPos, t);

            yield return null;
        }

        Impact(meteor, targetPos);
    }

    void Impact(GameObject meteor, Vector3 hitPos)
    {
        Destroy(meteor);

        //DrawDebugCircle(hitPos, damageRadius, Color.red, 1.5f);

        Collider2D[] hits = Physics2D.OverlapCircleAll(hitPos, damageRadius, enemyLayer);
        foreach (var hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(stats.MeteorDamage, DamageType.Physical);
            }
        }

        if (explosionPrefab != null)
        {
            GameObject expl = Instantiate(explosionPrefab, hitPos, Quaternion.identity);
            Destroy(expl, 1.5f);
        }

        if (scorchPrefab != null && stats.isScorchedEarth)
        {
            GameObject scorch = Instantiate(scorchPrefab, hitPos, Quaternion.identity);
            scorch.GetComponent<ScorchDamage>().damage = stats.ScorchedEarthDamage;
            Destroy(scorch, stats.scorchedEarthDuration); 
        }
    }

    //void DrawDebugCircle(Vector3 center, float radius, Color color, float duration = 0f, int segments = 32)
    //{
    //    float angleStep = 360f / segments;
    //    Vector3 prevPoint = center + new Vector3(Mathf.Cos(0), Mathf.Sin(0)) * radius;

    //    for (int i = 1; i <= segments; i++)
    //    {
    //        float rad = Mathf.Deg2Rad * (i * angleStep);
    //        Vector3 nextPoint = center + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;
    //        Debug.DrawLine(prevPoint, nextPoint, color, duration);
    //        prevPoint = nextPoint;
    //    }
    //}
}
