using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    List<Transform> waypoints = new List<Transform>();
    GameObject waypointsParent;
    Enemy enemy;

    int waypointIndex = 0;
    Vector2 lastDirection;

    void Awake()
    {
        enemy = GetComponent<Enemy>();

        string[] waypointParents = { "WayPointsParent1", "WayPointsParent2", "WayPointsParent3" };
        string chosenParent = waypointParents[Random.Range(0, waypointParents.Length)];
        waypointsParent = GameObject.Find(chosenParent);

        foreach (Transform child in waypointsParent.transform)
        {
            if (child.CompareTag(TagConsts.WAYPOINT_TAG))
                waypoints.Add(child);
        }
    }

    void Start()
    {
        transform.position = waypoints[waypointIndex].position;
        enemy.SetAnimation(AnimConsts.ENEMYRUN_ANIM);
    }

    void Update()
    {
        if (enemy.isDead) return;

        if (enemy.blockingMelee != null && !enemy.blockingMelee.isDead)
        {
            if (Vector2.Distance(transform.position, enemy.blockingMelee.transform.position) <= 0.35f)
            {
                enemy.EnemyAttack();
            }
            else
            {
                Move();
            }
        }
        else
        {
            enemy.blockingMelee = null;
            enemy.isBlocked = false;
            Move();
        }
    }

    void Move()
    {
        if (waypointIndex >= waypoints.Count) return;

        transform.position = Vector2.MoveTowards(transform.position, waypoints[waypointIndex].position, enemy.enemyStats.speed * Time.deltaTime);

        Vector2 direction = (waypoints[waypointIndex].position - transform.position).normalized;
        if (direction != lastDirection)
        {
            UpdateAnimation(direction);
            lastDirection = direction;
        }

        if (Vector2.Distance(transform.position, waypoints[waypointIndex].position) < 0.05f)
        {
            waypointIndex++;
        }
    }

    void UpdateAnimation(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            if(dir.x > 0)
                enemy.SetAnimation(AnimConsts.ENEMYRUNRIGHT_ANIM);
            else
                enemy.SetAnimation(AnimConsts.ENEMYRUNLEFT_ANIM);
        }
        else
        {
            if (dir.y > 0)
                enemy.SetAnimation(AnimConsts.ENEMYRUNBACK_ANIM);
            else
                enemy.SetAnimation(AnimConsts.ENEMYRUN_ANIM);
        }
    }
}
