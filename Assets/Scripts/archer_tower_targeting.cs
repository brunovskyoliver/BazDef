using UnityEngine;
using System.Collections.Generic;

public class AcrherTowerTargeting : MonoBehaviour
{
    private float targetRange;
    private LayerMask enemyLayer;
    private List<GameObject> enemiesInRange = new List<GameObject>();
    private bool isInitialized = false;

    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (isInitialized) return;

        ArcherTowerPlacement towerPlacement = Object.FindFirstObjectByType<ArcherTowerPlacement>();
        if (towerPlacement != null)
        {
            targetRange = towerPlacement.towerRange * 2f; 
        }
        enemyLayer = LayerMask.GetMask("Enemy");
        isInitialized = true;
    }

    void Update()
    {
        DetectEnemiesInRange();
    }

    void DetectEnemiesInRange()
    {
        if (!isInitialized || Time.timeSinceLevelLoad < 0.5f) return;

        enemiesInRange.Clear();
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(
            transform.position, 
            targetRange, 
            enemyLayer
        );

        foreach (Collider2D col in hitColliders)
        {
            if (col != null && col.gameObject != null)
            {
                float distance = Vector2.Distance(transform.position, col.transform.position);
                if (distance <= targetRange)
                {
                    enemiesInRange.Add(col.gameObject);
                }
            }
        }
    }

    public bool HasTargetsInRange()
    {
        return enemiesInRange.Count > 0;
    }

    public GameObject GetClosestEnemy()
    {
        if (enemiesInRange.Count == 0) return null;
        GameObject closest = null;
        //float closestDistance = 1000f;
        float priority = 0f;
        foreach (GameObject enemy in enemiesInRange)
        {
            // float distance = Vector2.Distance(transform.position, enemy.transform.position);
            // if (distance < closestDistance)
            // {
            //     closest = enemy;
            //     closestDistance = distance;
            // }
            // get the furthest enemy
            Walker walker = enemy.GetComponent<Walker>();
            if (walker != null && walker.priority > priority && !walker.toBeDestroyed)
            {
                closest = enemy;
                priority = walker.priority;
            }
        }
        
        return closest;
    }
}