using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private List<Transform> waypoints = new List<Transform>();

    Animator anim;
    int waypointIndex = 0;
    Vector2 lastDirection;
    bool isWaiting = false; // trạng thái đang nghỉ

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        transform.position = waypoints[waypointIndex].position;
        anim.SetBool("isRunning", true);
    }

    void Update()
    {
        if (!isWaiting) // chỉ di chuyển khi không nghỉ
        {
            Move();
        }
    }

    void Move()
    {
        if (waypointIndex >= waypoints.Count) return;

        transform.position = Vector2.MoveTowards(transform.position, waypoints[waypointIndex].position, moveSpeed * Time.deltaTime);

        Vector2 direction = (waypoints[waypointIndex].position - transform.position).normalized;
        if (direction != lastDirection)
        {
            lastDirection = direction;
        }

        if (Vector2.Distance(transform.position, waypoints[waypointIndex].position) < 0.05f)
        {
            StartCoroutine(WaitAtWaypoint());
        }
    }

    IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;
        anim.SetBool("isRunning", false); 

        yield return new WaitForSeconds(2f); 

        waypointIndex++;
        if (waypointIndex >= waypoints.Count)
        {
            waypointIndex = 0; 
        }

        anim.SetBool("isRunning", true); 
        isWaiting = false;
    }
}