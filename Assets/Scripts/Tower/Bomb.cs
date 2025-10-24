using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float explosionRadius;
    [SerializeField] private float arcHeight;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private GameObject explodeEffect;

    int damage;
    float progress;
    Enemy enemy;
    Vector3 spawnPosition;
    Vector3 oldPoint2;

    public void SetTarget(Vector3 spawnPosition, Enemy enemy, int damage)
    {
        this.spawnPosition = spawnPosition;
        this.enemy = enemy;
        this.damage = damage;
    }

    void Update()
    {
        Vector3 point2; 
        if (enemy != null)
        {
            point2 = enemy.transform.position; 
        }
        else
        {
            point2 = oldPoint2; 
        }

        Vector3 point0 = spawnPosition; 
        Vector3 point1 = point0 + (point2 - point0) / 2 + Vector3.up * arcHeight; 
        oldPoint2 = point2;

        if (progress < 1.0f) 
        {
            progress += speed * Time.deltaTime; 
            Vector3 m1 = Vector3.Lerp(point0, point1, progress);
            Vector3 m2 = Vector3.Lerp(point1, point2, progress);
            transform.position = Vector3.Lerp(m1, m2, progress);

            transform.Rotate(-Vector3.forward * rotationSpeed * Time.deltaTime);
        }

        if (progress > 1.0f) 
        {
            HitTarget();
        }
    }

    void HitTarget()
    {
        GameObject effectIns = Instantiate(explodeEffect, transform.position + new Vector3(0f, 0.2f, 0f), Quaternion.identity);
        Destroy(effectIns, 3f);

        Explode();
    }

    void Explode()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, DamageType.Physical);
            }
        }

        Destroy(gameObject);
    }
}
