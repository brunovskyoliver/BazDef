using System;
using UnityEngine;


public class MortarTowerAttack : MonoBehaviour
{
    private MortarTowerTargeting targeting;
    private MortarTowerPlacement placement;
    private GameObject archer;
    public float attackCooldown = 2f;
    private float nextAttackTime = 0f;
    public float attackDamage = 10f;
    private const float animLenght = 0.5f;
    private int enemyDir; // 0 = down, 1 = up, 2 = left, 3 = right
    private Walker enemy;
    public float upgradeCost = 10f;


    void Start()
    {
        
        targeting = GetComponent<MortarTowerTargeting>();
        placement = FindAnyObjectByType<MortarTowerPlacement>();
        attackCooldown = level_settings.Instance.mortarTowerSettings.attackSpeed;
        attackDamage = level_settings.Instance.mortarTowerSettings.attackDamage;


    }

    void FixedUpdate()
    {
        if (Time.timeSinceLevelLoad < attackCooldown) return;
        
        if (Time.time >= nextAttackTime)
        {
            
            if (targeting != null && targeting.HasTargetsInRange())
            {
                GameObject target = targeting.GetClosestEnemy();
                if (target != null)
                {
                    Attack(target);
                    
                    
                    nextAttackTime = Time.time + attackCooldown;
                }
            }
        }

    }

    void Attack(GameObject target)
    {
        archer = FindClosestObjectByName("Mortar", transform.position);
        Debug.Log(target.name);
        enemy = target.GetComponent<Walker>();

        if (!enemy.toBeDestroyed)
        { 
            enemyDir = FindEnemeyDir();
            CreateArrow();
        }

    }

    int FindEnemeyDir()
    {
        int dir;
        float xOffset = archer.transform.position.x - enemy.transform.position.x;
        float yOffset = archer.transform.position.y - enemy.transform.position.y;
        if (Math.Abs(yOffset) > Math.Abs(xOffset))
        {
            if (yOffset > 0)
            {
                dir = 1;
            }
            else
            {
                dir = 0;
            }
        }
        else
        {
            if (xOffset > 0)
            {
                dir = 2;
            }
            else
            {
                dir = 3;
            }
        }

        return dir;
    }

    void CreateArrow()
    {

        GameObject ballObject = new GameObject("mortarBall");
        var ball = ballObject.AddComponent<MortarBall>();
        SpriteRenderer ballsr = ballObject.AddComponent<SpriteRenderer>();

        ballsr.sprite = placement.arrowSprite;
        ball.towerPos = transform;
        ball.targetedEnemy = enemy;
        ball.mortarBall = ballObject;
        ball.attackDamage = attackDamage;


    }


    GameObject FindClosestObjectByName(string targetName, Vector3 position)
    {
        GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None); // Get all active objects
        GameObject closestObject = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name == targetName) // Check if name matches
            {
                float distance = Vector3.Distance(position, obj.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestObject = obj;
                }
            }
        }

        return closestObject;
    }
}