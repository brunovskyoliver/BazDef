using System.Collections.Generic;
using UnityEngine;

public class MortarBall : MonoBehaviour
{
    public Transform towerPos;
    public Walker targetedEnemy;
    public GameObject mortarBall;
    public float attackDamage = 10f;
    public float flightDuration = 0.8f;
    public float arcHeight = 2.5f;


    private Vector3 startPoint;
    private Vector3 endPoint;
    private float flightTime = 0f;
    private bool flying = true;

    void Start()
    {
        startPoint = towerPos.position + new Vector3(0f, 0.5f, 0); 
        endPoint = targetedEnemy.transform.position;

        transform.position = startPoint;
        transform.localScale = new Vector3(1.5f, 1.5f, 0);
    }

    void Update()
    {
        if (!flying) return;

        flightTime += Time.deltaTime;
        float t = Mathf.Clamp01(flightTime / flightDuration);


        Vector3 currentPos = Vector3.Lerp(startPoint, endPoint, t);


        float height = arcHeight * 4 * (t - t * t);
        currentPos.y += height;

        transform.position = currentPos;


        if (t < 1f)
        {
            Vector3 direction = (currentPos - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + 90f);
        }


        if (t >= 1f)
        {
            float radius = 1.5f;
            LayerMask enemyMask = LayerMask.GetMask("Enemy");
            List<GameObject> enemiesNearby = GetObjectsInRadius(transform.position, radius, enemyMask);
            foreach (GameObject enemy in enemiesNearby)
            {
                Walker enemyScript = enemy.GetComponent<Walker>();
                enemyScript.health -= attackDamage;
            }

            flying = false;
            Destroy(gameObject);
        }
    }

    public List<GameObject> GetObjectsInRadius(Vector3 center, float radius, LayerMask layerMask)
    {
        List<GameObject> objectsInRange = new List<GameObject>();
        

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(center, radius, layerMask);

        foreach (Collider2D col in hitColliders)
        {
            objectsInRange.Add(col.gameObject);

        }

        return objectsInRange;
    }
}