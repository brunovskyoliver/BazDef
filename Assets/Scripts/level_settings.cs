using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class WaveSettings
{
    public int numberOfEnemies = 5;
    public float spawnDelay = 1.5f;
    public float speed = 2f;

    public float numberOfEnemiesMultiplier = 1.2f;
    public float enemyAttackDamageMultiplier = 1.1f;
    public float enemyHealthMultiplier = 1.1f;
    public float currentWave;
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
    public List<EnemyType> enemyTypes = new List<EnemyType>();
    public float baseAttackDamage = 1f;
    public float baseAttackSpeed = 0.8f;
    public GameObject barFill;
    public GameObject barOutLine;
}

[System.Serializable]

public class ArcherTowerSettings
{
    public float towerRange = 2f;
    public float attackSpeed = 2f;
    public float attackDamage = 1f;
    public float cost = 2;
    public float costMultiplier = 1.8f;
}

[System.Serializable]
public class MortarTowerSettings
{
    public float towerRange = 2f;
    public float attackSpeed = 2f;
    public float attackDamage = 1f;
    public float cost = 5;
    public float costMultiplier = 2f;
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

        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
        }
    }

}
