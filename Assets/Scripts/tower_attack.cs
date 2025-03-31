using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Assertions;

public class TowerAttack : MonoBehaviour
{
    private TowerTargeting targeting;
    private TowerPlacement placement;
    private float attackCooldown = 1f;
    private float nextAttackTime = 0f;
    private float attackDamage = 1f;

    void Start()
    {
        targeting = GetComponent<TowerTargeting>();
        placement = FindAnyObjectByType<TowerPlacement>();
        attackCooldown = level_settings.Instance.towerSettings.attackSpeed;
        attackDamage = level_settings.Instance.towerSettings.attackDamage;
    }

    void FixedUpdate()
    {
        if (Time.timeSinceLevelLoad < attackCooldown) return;
        
        if (Time.time >= nextAttackTime)
        {
            Assert.IsNotNull(targeting, "failed to init target script");
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
        Debug.Log(target.name);
        Walker enemy = target.GetComponent<Walker>();
        
        GameObject arrowObject = new GameObject("arrow");
        var arrow = arrowObject.AddComponent<Arrow>();
        SpriteRenderer arrowsr = arrowObject.AddComponent<SpriteRenderer>();
        arrowsr.sprite = placement.arrowSprite;
        arrow.towerPos = transform;
        arrow.targetedEnemy = enemy;
        arrow.arrow = arrowObject;
        arrow.attackDamage = attackDamage;
    }
}