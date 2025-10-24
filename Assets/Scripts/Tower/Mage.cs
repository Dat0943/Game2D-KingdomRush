using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage : MonoBehaviour
{
    [SerializeField] private float speed;  
    [SerializeField] private float lifeTime = 3f; 
    [SerializeField] private float facingOffsetDeg = 0f;

    Enemy target;
    int damage;

    public void SetTarget(Enemy target, int damage)
    {
        this.target = target;
        this.damage = damage;

        if (target != null)
        {
            Vector3 dir = target.transform.position - transform.position;
            float angleZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + facingOffsetDeg;
            transform.rotation = Quaternion.Euler(0f, 0f, angleZ);
        }

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (target == null)
        {
            transform.position += transform.right * speed * Time.deltaTime;
            return;
        }

        CalculatePosition();

        CheckHitTarget();
    }

    void CalculatePosition()
    {
        Vector3 dir = (target.transform.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
    }

    void CheckHitTarget()
    {
        if (Vector2.Distance(transform.position, target.transform.position) < 0.2f)
        {
            target.TakeDamage(damage, DamageType.Magical);
            Destroy(gameObject);
        }
    }
}
