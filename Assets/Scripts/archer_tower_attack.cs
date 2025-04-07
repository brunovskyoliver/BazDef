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

public class ArcherTowerAttack : MonoBehaviour
{
    private AcrherTowerTargeting targeting;
    private ArcherTowerPlacement placement;
    private GameObject archer;
    private float attackCooldown;
    private float nextAttackTime = 0f;
    private float attackDamage = 1f;
    private float waitToChangeArcherAnim = 0.2f;
    private const float animLenght = 0.5f;
    private bool toCreateArrow;
    private int enemyDir; // 0 = down, 1 = up, 2 = left, 3 = right
    private Walker enemy;
    private TrailRenderer arrowTrail;


    void Start()
    {

        targeting = GetComponent<AcrherTowerTargeting>();
        placement = FindAnyObjectByType<ArcherTowerPlacement>();
        attackCooldown = level_settings.Instance.archerTowerSettings.attackSpeed;
        attackDamage = level_settings.Instance.archerTowerSettings.attackDamage;
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
        archer = FindClosestObjectByName("Archer", transform.position);
        Debug.Log(target.name);
        enemy = target.GetComponent<Walker>();

        Animator archerAnim = archer.GetComponent<Animator>();
        if (!enemy.toBeDestroyed)
        {
            enemyDir = FindEnemeyDir();
            archer.transform.localScale = placement.archerSize; // reset the orientation
            if (enemyDir == 0)
                archerAnim.Play("archer_Shoot_Back");
            else if (enemyDir == 1)
                archerAnim.Play("archer_Shoot_Front");
            else if (enemyDir == 2)
                archerAnim.Play("archer_Shoot_Left");
            else
            {
                archerAnim.Play("archer_Shoot_Left");
                archer.transform.localScale = new Vector3(-3, 3, 1); // if I want to shoot to the right i have to miiror the left anim
            }

            StartCoroutine(CreateArrow());
        }

    }

    int FindEnemeyDir()
    {
        int dir;
        float xOffset = archer.transform.position.x - enemy.transform.position.x;
        float yOffset = archer.transform.position.y - enemy.transform.position.y;
        dir = Math.Abs(yOffset) > Math.Abs(xOffset) ? yOffset > 0 ? 1 : 0 : xOffset > 0 ? 2 : 3;
        return dir;
    }

    IEnumerator CreateArrow()
    {
        yield return new WaitForSeconds(animLenght);
        Animator archerAnim = archer.GetComponent<Animator>();
        if (enemyDir == 0)
            archerAnim.Play("archer_Idle_Back");
        else if (enemyDir == 1)
            archerAnim.Play("archer_Idle_Front");
        else
            archerAnim.Play("archer_Idle_Left");




        GameObject arrowObject = new GameObject("arrow");
        arrowTrail = arrowObject.AddComponent<TrailRenderer>();
        arrowTrail.time = 0.2f;
        arrowTrail.startWidth = 0f;
        arrowTrail.endWidth = 0f;
        arrowTrail.material = placement.arrowTrailMaterial;
        arrowTrail.transform.localPosition = new Vector3(0, 0, -0.5f);
        AnimationCurve widthCurve = new AnimationCurve();
        widthCurve.AddKey(0.08f, 0f);
        widthCurve.AddKey(0.126f, 0.10f);
        widthCurve.AddKey(0.89f, 0f);
        arrowTrail.widthCurve = widthCurve;
        var arrow = arrowObject.AddComponent<Arrow>();
        SpriteRenderer arrowsr = arrowObject.AddComponent<SpriteRenderer>();

        arrowsr.sprite = placement.arrowSprite;
        arrow.towerPos = transform;
        arrow.targetedEnemy = enemy;
        arrow.arrow = arrowObject;
        arrow.attackDamage = attackDamage;
        yield break;
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
