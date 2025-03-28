using UnityEngine;
using System.Collections.Generic;

public class Walker : MonoBehaviour
{
    public float spriteScale = 0.25f;
    public float speed = 2f;
    public List<Vector3> waypoints;
    private int currentWaypoint = 1;
    private Vector3 baseScale;
    private float xOffset = 0.5f;
    private bool isWalking = true;
    private Animator animator;

    void Start()
    {
        transform.position = waypoints[0];
        baseScale = new Vector3(spriteScale, spriteScale, 1);
        transform.localScale = baseScale;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (waypoints.Count == 0 || !isWalking) return;
        
        Vector3 target = waypoints[currentWaypoint];
        Vector3 direction = target - transform.position;
        if (Mathf.Abs(direction.x) > 0.01f)
        {
            baseScale.x = Mathf.Abs(spriteScale) * (direction.x > 0 ? 1 : -1);
            transform.localScale = baseScale;
        }
        
        Vector3 currentPos = transform.position;
        Vector3 targetPos = new Vector3(target.x + xOffset, target.y, -1);
        transform.position = Vector3.MoveTowards(currentPos, targetPos, speed * Time.deltaTime);
        float distance = Vector2.Distance(
            new Vector2(transform.position.x, transform.position.y),
            new Vector2(target.x + xOffset, target.y)
        );

        if (distance < 0.1f)
        {
            currentWaypoint++;
            if (currentWaypoint >= waypoints.Count)
            {
                isWalking = false;
                transform.position = targetPos;
                if (animator != null)
                {
                    animator.enabled = false;
                }
            }
        }
    }
}
