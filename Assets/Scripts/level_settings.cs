using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class WaveSettings
{
    public int numberOfEnemies = 5;
    public float spawnDelay = 1.5f;
    public float speed = 2f;
}

[System.Serializable]
public class PlayerSettings
{
    public Sprite sprite;
    public float health = 5f;
    public float maxHealth = 5f;
    public bool playerDeath = false;
    public GameObject playerTowerObject;
    public RuntimeAnimatorController animator;
    public Vector3 scale;
    public Vector3 offset;
}

[System.Serializable]
public class EnemySettings
{
    public GameObject parentWaypoint;
    public Sprite enemySprite;
    public RuntimeAnimatorController enemyAnimator;
    public float attackDamage = 1f;
    public float attackSpeed = 0.8f;
}

[System.Serializable]

public class ArcherTowerSettings
{
    public float towerRange = 2f;
    public float attackSpeed = 2f;
    public float attackDamage = 1f;
}

[System.Serializable]
public class MortarTowerSettings
{
    public float towerRange = 2f;
    public float attackSpeed = 2f;
    public float attackDamage = 1f;
}

public class level_settings : MonoBehaviour
{
    public PlayerSettings playerSettings;  
    public EnemySettings enemySettings;
    [Tooltip("Prvy waypoint je startovacia pozicia")]
    public List<Vector3> enemyWaypoints;
    public GameObject parentWaypoint;
    public WaveSettings waveSettings;
    public ArcherTowerSettings archerTowerSettings;
    public MortarTowerSettings mortarTowerSettings;

    public static level_settings Instance { get; private set; }

    void Awake()
    {
       // enemyWaypoints = FindWayPoints();
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
        }
    }

    public List<Vector3> FindWayPoints()
    {
        List<Vector3> waypointsPos = new List<Vector3>();
        foreach (Transform child in parentWaypoint.transform)
        {
            waypointsPos.Add(child.position);  
        }
        return waypointsPos;
    }
}
