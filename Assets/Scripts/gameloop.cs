using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class gameloop : MonoBehaviour
{
    private int spawnedEnemies = 0;
    private bool isSpawnable = true;
    private float nextSpawnTime = 0f;


    void Start()

    {
        UnityEngine.Debug.Log("Game Started");
        SpawnPlayer();
    }

    void Update()
    {
        if (isSpawnable && Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        GameObject newWalker = new GameObject("Mety_"+spawnedEnemies); // rozdelime ich podla poradia spawnutia
        newWalker.layer = LayerMask.NameToLayer("Enemy");
        CircleCollider2D collider = newWalker.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;

        SpriteRenderer sr = newWalker.AddComponent<SpriteRenderer>();
        sr.sprite = level_settings.Instance.enemySettings.enemySprite;
        
        Animator anim = newWalker.AddComponent<Animator>();
        anim.runtimeAnimatorController = level_settings.Instance.enemySettings.enemyAnimator;
        
        var walker = newWalker.AddComponent<Walker>();
        walker.waypoints = new List<Vector3>(level_settings.Instance.enemyWaypoints);
        walker.speed = level_settings.Instance.waveSettings.speed;

        spawnedEnemies++;
        if (spawnedEnemies >= level_settings.Instance.waveSettings.numberOfEnemies)
        {
            isSpawnable = false;
        }
        
        nextSpawnTime = Time.time + level_settings.Instance.waveSettings.spawnDelay;
    }
    void SpawnPlayer()
    {
        GameObject player = new GameObject("Player");
        SpriteRenderer sr = player.AddComponent<SpriteRenderer>();
        sr.sprite = level_settings.Instance.playerSettings.sprite;
        
        Animator anim = player.AddComponent<Animator>();
        anim.runtimeAnimatorController = level_settings.Instance.playerSettings.animator;
        GameObject playerTowerObj = level_settings.Instance.playerSettings.playerTowerObject;
        player.transform.SetParent(playerTowerObj.transform);
        player.transform.localPosition = level_settings.Instance.playerSettings.offset; 
        player.transform.localScale = level_settings.Instance.playerSettings.scale;
    }

}

