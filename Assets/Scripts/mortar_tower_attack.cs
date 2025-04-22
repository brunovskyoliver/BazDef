using System;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;



public class MortarTowerAttack : MonoBehaviour
{
    private MortarTowerTargeting targeting;
    private MortarTowerPlacement placement;
    private GameObject archer;
    public float attackCooldown = 2f;
    private float nextAttackTime = 0f;
    public float attackDamage = 10f;
    private const float animLenght = 0.39f;
    private Walker enemy;
    public float upgradeCost = 10f;
    private Animator mortarAnim;


    void Start()
    {
        
        targeting = GetComponent<MortarTowerTargeting>();
        placement = FindAnyObjectByType<MortarTowerPlacement>();
        attackCooldown = level_settings.Instance.mortarTowerSettings.attackSpeed;
        attackDamage = level_settings.Instance.mortarTowerSettings.attackDamage;
        mortarAnim = this.AddComponent<Animator>();
        mortarAnim.runtimeAnimatorController = placement.mortarAnimator;


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
        //Debug.Log(target.name);
        enemy = target.GetComponent<Walker>();
        mortarAnim.Play("MortarShoot");

        if (!enemy.toBeDestroyed)
        {
            StartCoroutine(CreateArrow());
        }

    }

    IEnumerator CreateArrow()
    {
        yield return new WaitForSeconds(animLenght);
        GameObject ballObject = new GameObject("mortarBall");
        var ball = ballObject.AddComponent<MortarBall>();
        SpriteRenderer ballsr = ballObject.AddComponent<SpriteRenderer>();

        ballsr.sprite = placement.arrowSprite;
        ball.towerPos = transform;
        ball.targetedEnemy = enemy;
        ball.mortarBall = ballObject;
        ball.attackDamage = attackDamage;
        mortarAnim.Play("mortar idle");



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