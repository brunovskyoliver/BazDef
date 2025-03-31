using System.Linq.Expressions;
using JetBrains.Annotations;
using UnityEditor.Rendering;
using UnityEditor.Toolbars;
using UnityEngine;

public class TowerAttack : MonoBehaviour
{
    private TowerTargeting targeting;
    private TowerPlacement placement;
    private GameObject archer;
    private float attackCooldown;
    private float nextAttackTime = 0f;
    private float attackDamage = 1f;
    private float waitToChangeArcherAnim = 0.2f;
    private float startWaitTime;
    private const float animLenght = 0.5f;
    private bool toCreateArrow;
    private Walker enemy;


    void Start()
    {
        
        targeting = GetComponent<TowerTargeting>();
        placement = FindAnyObjectByType<TowerPlacement>();
        attackCooldown = level_settings.Instance.towerSettings.attackSpeed;
        attackDamage = level_settings.Instance.towerSettings.attackDamage;
    }

    void Update()
    {
        if (Time.timeSinceLevelLoad < attackCooldown) return;
        
        if (Time.time >= nextAttackTime)
        {
            
            if (targeting != null && targeting.HasTargetsInRange())
            {
                GameObject target = targeting.GetClosestEnemy();
                if (target != null)
                {
                    startWaitTime = Time.time + animLenght;
                    toCreateArrow = true; 
                    Attack(target);
                    
                    nextAttackTime = Time.time + attackCooldown;
                }
            }
        }

        if (toCreateArrow && Time.time > startWaitTime)
        {
            CreateArrow();
            toCreateArrow = false;
        }
    }

    void Attack(GameObject target)
    {
        archer = FindClosestObjectByName("Archer", transform.position);
        Debug.Log(target.name);
        enemy = target.GetComponent<Walker>();

        Animator archerAnim = archer.GetComponent<Animator>();
        archerAnim.SetTrigger("archer_Shoot");

    }

    void CreateArrow()
    {
        Animator archerAnim = archer.GetComponent<Animator>();
        archerAnim.SetTrigger("archer_Idle");
        
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