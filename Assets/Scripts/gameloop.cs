using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class gameloop : MonoBehaviour
{
    private int spawnedEnemies = 0;
    private bool isSpawnable = true;
    private float nextSpawnTime = 0f;
    public bool waveStarted = false;
    public Button startWaveButton;
    public static gameloop Instance { get; private set; }
    private GameObject player;
    private Image healthBarFill;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        UnityEngine.Debug.Log("Game Started");
        SpawnPlayer();
        if (startWaveButton == null) UnityEngine.Debug.Log("Pridaj button do inspectoru");
        startWaveButton.onClick.AddListener(StartWave);
    }

    void StartWave()
    {
        waveStarted = true;
        startWaveButton.gameObject.SetActive(false); 
    }

    void Update()
    {
        if (!waveStarted) return; 

        if (isSpawnable && Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        GameObject newWalker = new GameObject("Mety_" + spawnedEnemies); // rozdelime ich podla poradia spawnutia
        newWalker.transform.position = level_settings.Instance.enemyWaypoints[0]; // toto je kvoli attackovaniu, pretoze unity ked spawne objekt tak ho hodi na 0,0,0 a tym padom moze triggerovat attack veze
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
        player = new GameObject("Player");
        SpriteRenderer sr = player.AddComponent<SpriteRenderer>();
        sr.sprite = level_settings.Instance.playerSettings.sprite;
        sr.sortingOrder = 1;

        Animator anim = player.AddComponent<Animator>();
        anim.runtimeAnimatorController = level_settings.Instance.playerSettings.animator;
        GameObject playerTowerObj = level_settings.Instance.playerSettings.playerTowerObject;
        player.transform.SetParent(playerTowerObj.transform);
        player.transform.localPosition = level_settings.Instance.playerSettings.offset;
        player.transform.localScale = level_settings.Instance.playerSettings.scale;

        GameObject healthBarContainer = new GameObject("HealthBarContainer");
        Canvas canvas = healthBarContainer.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 2;
        
        healthBarContainer.transform.SetParent(player.transform);
        healthBarContainer.transform.localPosition = new Vector3(-0.3f, 0.5f, 0);
        healthBarContainer.transform.localScale = new Vector3(0.01f, 0.01f, 1f); 

        GameObject healthBarBg = new GameObject("HealthBarBackground");
        healthBarBg.transform.SetParent(healthBarContainer.transform);
        Image bgImage = healthBarBg.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f); 
        RectTransform bgRect = bgImage.rectTransform;
        bgRect.sizeDelta = new Vector2(100, 10); 
        bgRect.localPosition = Vector3.zero;
        bgRect.localScale = new Vector3(1f, 1f, 0);

        GameObject healthBarFillObj = new GameObject("HealthBarFill");
        healthBarFillObj.transform.SetParent(healthBarBg.transform);
        healthBarFill = healthBarFillObj.AddComponent<Image>(); 
        healthBarFill.color = Color.green;
        RectTransform fillRect = healthBarFill.rectTransform;
        fillRect.sizeDelta = new Vector2(100, 10);
        fillRect.localPosition = Vector3.zero;
        fillRect.localScale = new Vector3(1f, 1f, 0);
    }

    public void UpdateHealthBar(float healthPercent)
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = healthPercent;
        }
    }
}

