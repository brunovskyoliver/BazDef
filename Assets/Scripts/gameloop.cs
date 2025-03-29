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
        SpriteRenderer sr = newWalker.AddComponent<SpriteRenderer>();
        sr.sprite = level_settings.Instance.enemySprite;
        
        Animator anim = newWalker.AddComponent<Animator>();
        anim.runtimeAnimatorController = level_settings.Instance.enemyAnimator;
        
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
}
