using System;
using System.Collections;
using UnityEngine;


public class IceTowerAttack : MonoBehaviour
{
    private IceTowerTargeting targeting;
    private IceTowerPlacement placement;
    private GameObject ice;
    public float attackCooldown;
    private float nextAttackTime = 0f;
    public float attackDamage = 1f;

    private Walker enemy;
    public float upgradeCost = 2f;


    void Start()
    {

        targeting = GetComponent<IceTowerTargeting>();
        placement = FindAnyObjectByType<IceTowerPlacement>();
        attackCooldown = level_settings.Instance.iceTowerSettings.attackSpeed;
        attackDamage = level_settings.Instance.iceTowerSettings.attackDamage;
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
        ice = FindClosestObjectByName("Ice", transform.position);
        Debug.Log(target.name);
        enemy = target.GetComponent<Walker>();

        if (!enemy.toBeDestroyed)
        {
            StartCoroutine(CreateArrow());
        }


    }

    IEnumerator CreateArrow()
    {
        GameObject projectileObject = new GameObject("IceProjectile");
        var ball = projectileObject.AddComponent<IceProjectile>();
        SpriteRenderer ballsr = projectileObject.AddComponent<SpriteRenderer>();

        ballsr.sprite = placement.iceSprite;
        ball.towerPos = transform;
        ball.targetedEnemy = enemy;
        ball.projectile = projectileObject;
        ball.attackDamage = attackDamage;

        yield return null;


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
