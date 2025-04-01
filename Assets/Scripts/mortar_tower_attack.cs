using System;
using System.Collections;
using System.Runtime.ExceptionServices;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Rendering;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.Assertions;

public class MortarTowerAttack : MonoBehaviour
{
    private MortarTowerTargeting targeting;
    private MortarTowerPlacement placement;
    private GameObject archer;
    private float attackCooldown;
    private float nextAttackTime = 0f;
    private float attackDamage = 1f;
    private const float animLenght = 0.5f;
    private int enemyDir; // 0 = down, 1 = up, 2 = left, 3 = right
    private Walker enemy;


    void Start()
    {
        
        targeting = GetComponent<MortarTowerTargeting>();
        placement = FindAnyObjectByType<MortarTowerPlacement>();
        attackCooldown = level_settings.Instance.towerSettings.attackSpeed;
        attackDamage = level_settings.Instance.towerSettings.attackDamage;
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

        GameObject arrowObject = new GameObject("arrow");
        var arrow = arrowObject.AddComponent<Arrow>();
        SpriteRenderer arrowsr = arrowObject.AddComponent<SpriteRenderer>();

        arrowsr.sprite = placement.arrowSprite;
        arrow.towerPos = transform;
        arrow.targetedEnemy = enemy;
        arrow.arrow = arrowObject;
        arrow.attackDamage = attackDamage;

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