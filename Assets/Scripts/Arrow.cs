using System;
using Unity.Mathematics;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Transform towerPos;
    public Walker targetedEnemy;
    public GameObject arrow;
    private Vector3 move;
    private float arrowSpeed = 0.1f;
    private float xOffset;
    private float yOffset;
    public float attackDamage;
    
    void Start()
    {
        arrow.transform.position = towerPos.position;
    }

    void FixedUpdate()
    {
        if (targetedEnemy == null)
        {
            Destroy(arrow);
            return;
        }
        xOffset = arrow.transform.position.x - targetedEnemy.transform.position.x;
        yOffset = arrow.transform.position.y - targetedEnemy.transform.position.y;

        if (Mathf.Abs(xOffset) < 0.1f && Mathf.Abs(yOffset) < 0.1f) 
        {
            if (targetedEnemy != null)
            {
                targetedEnemy.health -= attackDamage;
                Destroy(arrow);
            }
        }
        double angleToEnemyRadians = Math.Atan(yOffset/xOffset);

        if (xOffset > 0)
        {
            move = -new Vector3((float)(1*Math.Cos(angleToEnemyRadians)), (float)(1*Math.Sin(angleToEnemyRadians)), 0);  
        }
        else
        {
            move = new Vector3((float)(1*Math.Cos(angleToEnemyRadians)), (float)(1*Math.Sin(angleToEnemyRadians)), 0);  
        }
        Debug.Log(move);

        arrow.transform.position += move * arrowSpeed;
        

        
    }
}