using UnityEngine;
using System.Collections.Generic;

public class Walker : MonoBehaviour
{
    public float spriteScale = 0.25f;
    public float speed = 2f;
    public List<Vector3> waypoints;
    private int currentWaypoint = 0;
    private Vector3 baseScale;
    private float xOffset = 0.5f;

    void Start()
    {
        transform.position = new Vector3(-1 + xOffset, 9, -1);
        baseScale = new Vector3(spriteScale, spriteScale, 1);
        transform.localScale = baseScale;
    }

    void Update()
    {
        if (waypoints.Count == 0) return;
        Vector3 target = waypoints[currentWaypoint];
        Vector3 direction = target - transform.position;
        if (Mathf.Abs(direction.x) > 0.10f)
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
                speed = 0;
            }
        }
    }
}
