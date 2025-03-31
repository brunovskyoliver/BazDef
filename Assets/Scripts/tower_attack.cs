using System.Linq.Expressions;
using JetBrains.Annotations;
using UnityEngine;

public class TowerAttack : MonoBehaviour
{
    private TowerTargeting targeting;
    private TowerPlacement placement;
    private GameObject archer;
    private float attackCooldown;
    private float nextAttackTime = 0f;
    private float attackDamage = 1f;


    void Start()
    {
        archer = FindClosestObjectByName("Archer", transform.position);
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
                    Attack(target);
                    nextAttackTime = Time.time + attackCooldown;
                    Debug.Log(nextAttackTime);
                    Debug.Log(Time.time);
                }
            }
        }
    }

    void Attack(GameObject target)
    {
        Debug.Log(target.name);
        Walker enemy = target.GetComponent<Walker>();
        
        GameObject arrowObject = new GameObject("arrow");
        var arrow = arrowObject.AddComponent<Arrow>();
        SpriteRenderer arrowsr = arrowObject.AddComponent<SpriteRenderer>();
        // Animator archerAnim = archer.GetComponent<Animator>();

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