using UnityEngine;
using System.Collections.Generic;

public class Walker : MonoBehaviour
{
    public float spriteScale = 0.25f;
    [Tooltip("Prvy waypoint je startovacia pozicia")]
    public List<Vector3> waypoints;
    private int currentWaypoint = 1; // prvy vykreslime na Start()
    public float speed;
    private Vector3 baseScale;
    private float xOffset = 0.5f;
    private bool isWalking = true;
    private Animator animator;
    private float lastDirection = 1f;

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

        // pri poslednom checkpointe sa neotaca
        if (isWalking && Mathf.Abs(direction.x) > 0.01f && currentWaypoint < waypoints.Count -1 )
        {
            lastDirection = Mathf.Sign(direction.x); 
            baseScale.x = Mathf.Abs(spriteScale) * lastDirection;
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
                // toto zabezpeci, ze sa zachova posledny smer
                baseScale.x = Mathf.Abs(spriteScale) * lastDirection;
                transform.localScale = baseScale;
            }
        }
    }
}
