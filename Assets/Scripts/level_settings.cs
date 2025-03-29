using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class WaveSettings
{
    public int numberOfEnemies = 5;
    public float spawnDelay = 1.5f;
    public float speed = 2f;
}

public class level_settings : MonoBehaviour
{
    public GameObject parentWaypoint;
    public Sprite enemySprite;
    public RuntimeAnimatorController enemyAnimator;
     [Tooltip("Prvy waypoint je startovacia pozicia")]
    public List<Vector3> enemyWaypoints;
    public WaveSettings waveSettings;

    public static level_settings Instance { get; private set; }

    void Awake()
    {
        enemyWaypoints = FindWayPoints();
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
