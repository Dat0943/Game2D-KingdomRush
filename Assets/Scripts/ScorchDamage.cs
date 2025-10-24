using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorchDamage : MonoBehaviour
{
    [SerializeField] private float tickInterval = 1f;
    [SerializeField] private LayerMask enemyLayer;
    [HideInInspector] public int damage;

    List<Enemy> enemiesInZone = new List<Enemy>();

    void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && !enemiesInZone.Contains(enemy))
        {
            enemiesInZone.Add(enemy);
            StartCoroutine(DealDamage(enemy));
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && enemiesInZone.Contains(enemy))
        {
            enemiesInZone.Remove(enemy);
        }
    }

    IEnumerator DealDamage(Enemy enemy)
    {
        while (enemiesInZone.Contains(enemy))
        {
            enemy.TakeDamage(damage, DamageType.Physical); 
            yield return new WaitForSeconds(tickInterval);
        }
    }
}
