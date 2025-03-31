using UnityEngine;
using UnityEngine.Assertions;

public class TowerAttack : MonoBehaviour
{
    private TowerTargeting targeting;
    private float attackCooldown = 0.5f;
    private float nextAttackTime = 0f;
    private float attackDamage = 1f;

    void Start()
    {
        targeting = GetComponent<TowerTargeting>();
        attackCooldown = level_settings.Instance.towerSettings.attackSpeed;
        attackDamage = level_settings.Instance.towerSettings.attackDamage;
    }

    void Update()
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
        if (enemy != null)
        {
            enemy.health -= attackDamage;
        }
        GameObject line = new GameObject("Line");
        LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, target.transform.position);
        Destroy(line, attackCooldown);
    }
}