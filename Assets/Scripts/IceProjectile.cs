using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class IceProjectile : MonoBehaviour
{
    public Transform towerPos;
    public Walker targetedEnemy;
    private Stats stats;
    public GameObject projectile;
    private Vector3 move;
    private float arrowSpeed = 0.2f;
    private float xOffset;
    private float yOffset;
    public float attackDamage;
    private bool destroyed = false;
    private Vector3 arrowScale = new Vector3(2f, 2f, 0);
    private GameObject arrowTrail;
    public float speedmultiplier = 0.5f;
    
    void Start()
    {
        projectile.transform.position = towerPos.position + new Vector3(0f, 0.5f, 0); // to spawn fro marcher not the middle of tower
        projectile.transform.localScale = arrowScale;
        projectile.tag = "Arrow";

        stats = FindFirstObjectByType<Stats>();
    }

    void FixedUpdate()
    {
        if (targetedEnemy.toBeDestroyed == true)
        {
            Destroy(projectile);
            return;
        }
        xOffset = projectile.transform.position.x - targetedEnemy.transform.position.x;
        yOffset = projectile.transform.position.y - targetedEnemy.transform.position.y;
        
        if (Mathf.Abs(xOffset) < 0.1f && Mathf.Abs(yOffset) < 0.1f && !destroyed) 
        {
            if (targetedEnemy != null)
            {
                targetedEnemy.health -= attackDamage;
                targetedEnemy.speed *= speedmultiplier;
                StartCoroutine(WaitForSeconds(2.0f,targetedEnemy));
                destroyed = true;
                projectile.transform.localScale = new Vector3(0,0,0);
                
            }
        }
        double angleToEnemyRadians = Math.Atan(yOffset/xOffset);
        projectile.transform.rotation = Quaternion.Euler(0,0,(float)angleToEnemyRadians * Mathf.Rad2Deg + 90);
        if (xOffset > 0) // inak to islo napok v prvej polke tak preto to minusko
        {
            move = -new Vector3((float)(1*Math.Cos(angleToEnemyRadians)), (float)(1*Math.Sin(angleToEnemyRadians)), 0);  
        }
        else
        {
            move = new Vector3((float)(1*Math.Cos(angleToEnemyRadians)), (float)(1*Math.Sin(angleToEnemyRadians)), 0);  
        }

        projectile.transform.position += move * arrowSpeed;
        

        
    }

    IEnumerator WaitForSeconds(float time, Walker targetedEnemy)
    {
        yield return new WaitForSeconds(time);
        
        targetedEnemy.speed *= 1/speedmultiplier;
        Debug.Log(targetedEnemy.speed);
        Destroy(projectile);
    }
}