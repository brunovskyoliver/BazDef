using System;
using Unity.Mathematics;
using UnityEditor.PackageManager;
using UnityEngine;

public class MortarBall : MonoBehaviour
{
    public Transform towerPos;
    public Walker targetedEnemy;
    public GameObject mortarBall;
    private Vector3 move;
    private float arrowSpeed = 0.2f;
    private float xOffset;
    private float yOffset;
    public float attackDamage;
    private Vector3 arrowScale = new Vector3(1.5f, 1.5f, 0);
    
    void Start()
    {
        mortarBall.transform.position = towerPos.position + new Vector3(0f, 0.5f, 0); // to spawn fro marcher not the middle of tower
        mortarBall.transform.localScale = arrowScale;
    }

    void FixedUpdate()
    {
        if (targetedEnemy.toBeDestroyed == true)
        {
            Destroy(mortarBall);
            return;
        }
        xOffset = mortarBall.transform.position.x - targetedEnemy.transform.position.x;
        yOffset = mortarBall.transform.position.y - targetedEnemy.transform.position.y;
        
        if (Mathf.Abs(xOffset) < 0.1f && Mathf.Abs(yOffset) < 0.1f) 
        {
            if (targetedEnemy != null)
            {
                targetedEnemy.health -= attackDamage;
                Destroy(mortarBall);
            }
        }
        double angleToEnemyRadians = Math.Atan(yOffset/xOffset);
        mortarBall.transform.rotation = Quaternion.Euler(0,0,(float)angleToEnemyRadians * Mathf.Rad2Deg + 90);
        if (xOffset > 0) // inak to islo napok v prvej polke tak preto to minusko
        {
            move = -new Vector3((float)(1*Math.Cos(angleToEnemyRadians)), (float)(1*Math.Sin(angleToEnemyRadians)), 0);  
        }
        else
        {
            move = new Vector3((float)(1*Math.Cos(angleToEnemyRadians)), (float)(1*Math.Sin(angleToEnemyRadians)), 0);  
        }

        mortarBall.transform.position += move * arrowSpeed;
        

        
    }
}