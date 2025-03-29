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
        if (level_settings.Instance == null)
        {
            UnityEngine.Debug.LogError("Chýba Level Settings objekt v scéne!");
            return;
        }

        if (level_settings.Instance.enemyWaypoints == null || level_settings.Instance.enemyWaypoints.Count == 0)
        {
            UnityEngine.Debug.LogError("Waypoints nie sú nastavené v Level Settings!");
            return;
        }

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
        if (level_settings.Instance == null || level_settings.Instance.enemyWaypoints == null)
        {
            UnityEngine.Debug.LogError("Level Settings alebo waypoints nie sú nastavené!");
            return;
        }

        GameObject newWalker = new GameObject("Mety_"+spawnedEnemies);
        SpriteRenderer sr = newWalker.AddComponent<SpriteRenderer>();
        sr.sprite = level_settings.Instance.enemySettings.enemySprite;
        
        Animator anim = newWalker.AddComponent<Animator>();
        anim.runtimeAnimatorController = level_settings.Instance.enemySettings.enemyAnimator;
        
        var walker = newWalker.AddComponent<Walker>();
        
        // Vytvoríme novú kópiu waypointov
        walker.waypoints = new List<Vector3>(level_settings.Instance.enemyWaypoints);
        UnityEngine.Debug.Log($"Nastavujem {walker.waypoints.Count} waypointov pre {newWalker.name}");
        
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
        
        player.transform.position = level_settings.Instance.playerSettings.position;
        player.transform.localScale = level_settings.Instance.playerSettings.scale;
    }
}

