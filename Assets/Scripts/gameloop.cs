using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;


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
    private RectTransform fillRect;
    public static List<Walker> activeWalkers = new List<Walker>();
    private float numEnemiesTospawn;
    public Text moneyText;
    public GameObject mainCamera;

    public float money = 10;
    private float moneyStart;
    private ArcherTowerPlacement archerTowerPlacement;
    private MortarTowerPlacement mortarTowerPlacement;
    public List<EnemyType> baseEnemyTypes = new List<EnemyType>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
        }
    }
    public void HandleInputR()
    {
        waveStarted = false;
        level_settings.Instance.ResetToDefault();
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        List<GameObject> toDestroy = new List<GameObject>();

        foreach (GameObject obj in GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            if (obj == null) continue;

            if (obj.layer == enemyLayer || obj.CompareTag("Archer") || obj.CompareTag("Mortar") || obj.CompareTag("Arrow") || obj.CompareTag("EnemyHealth") || obj.CompareTag("Box"))
            {
                toDestroy.Add(obj);
            }
        }

        foreach (GameObject obj in toDestroy)
        {
            if (obj != null)
            {
                GameObject.Destroy(obj);
            }
        }
        money = moneyStart;
        spawnedEnemies = 0;
        startWaveButton.gameObject.SetActive(true);
        archerTowerPlacement.Clear();
        mortarTowerPlacement.Clear();
    }

    void Start()
    {
        moneyStart = money;
        numEnemiesTospawn = (float)level_settings.Instance.waveSettings.numberOfEnemies;
        archerTowerPlacement = FindFirstObjectByType<ArcherTowerPlacement>();
        mortarTowerPlacement = FindFirstObjectByType<MortarTowerPlacement>();
        SpawnPlayer();
        if (startWaveButton == null) UnityEngine.Debug.Log("Pridaj button do inspectoru");
        startWaveButton.onClick.AddListener(StartWave);
    }

    void StartWave()
    {
        waveStarted = true;
        startWaveButton.gameObject.SetActive(false);
        isSpawnable = true;
        level_settings.Instance.waveSettings.numberOfEnemies = (int)(numEnemiesTospawn * level_settings.Instance.waveSettings.numberOfEnemiesMultiplier);
        numEnemiesTospawn = level_settings.Instance.waveSettings.numberOfEnemies;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            HandleInputR();
        }
        UpdateMoney();
        if (!waveStarted) return;

        if (isSpawnable && Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
        }
        if (player == null) return;

        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemyObjects.Count() == 0)
        {
            var enemyType = level_settings.Instance.enemySettings.enemyTypes;
            foreach (var type in enemyType)
            {
                type.health *= level_settings.Instance.waveSettings.enemyHealthMultiplier;
            }
            startWaveButton.gameObject.SetActive(true);
            waveStarted = false;
            spawnedEnemies = 0;
            level_settings.Instance.waveSettings.currentWave++;
        }

        //if (level_settings.Instance.playerSettings.health >= 0) UpdateHealthBar(level_settings.Instance.playerSettings.health / level_settings.Instance.playerSettings.maxHealth);
    }

    void UpdateMoney()
    {
        money = Mathf.Round(money * 100) / 100;
        moneyText.text = $"money: {money}";

    }

    void SpawnEnemy()
    {
        GameObject newWalker = new GameObject("Mety_" + spawnedEnemies);
        newWalker.tag = "Enemy";
        newWalker.transform.position = level_settings.Instance.enemyWaypoints[0];
        newWalker.layer = LayerMask.NameToLayer("Enemy");

        CircleCollider2D collider = newWalker.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;

        SpriteRenderer sr = newWalker.AddComponent<SpriteRenderer>();
        Animator anim = newWalker.AddComponent<Animator>();

        var walker = newWalker.AddComponent<Walker>();
        walker.waypoints = new List<Vector3>(level_settings.Instance.enemyWaypoints);
        walker.barOutline = Instantiate(level_settings.Instance.enemySettings.barOutLine);
        walker.barOutline.tag = "EnemyHealth";


        var enemyTypes = level_settings.Instance.enemySettings.enemyTypes;
        if (enemyTypes.Count > 0)
        {
            int i = UnityEngine.Random.Range(0, enemyTypes.Count);
            EnemyType randType = enemyTypes[i];
            walker.Initialize(randType);
        }

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
        fillRect = healthBarFill.rectTransform;
        fillRect.sizeDelta = new Vector2(100, 10);
        fillRect.localPosition = Vector3.zero;
        fillRect.localScale = new Vector3(1f, 1f, 0);
    }

    public void UpdateHealthBar(float healthPercent)
    {
        if (healthPercent <= 0 && !level_settings.Instance.playerSettings.playerDeath)
        {
            PlayerDeath();
            level_settings.Instance.playerSettings.playerDeath = true;
        }
        if (healthBarFill != null)
        {
            UnityEngine.Debug.Log("health " + healthPercent);
            fillRect.sizeDelta = new Vector2(100 * healthPercent, 10);
            fillRect.localPosition = new Vector3(50 * (1 - healthPercent), 0, 0);
        }
    }
    public void PlayerDeath()
    {
        Animator animator = player.GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play("Player_death");
            float deathAnimLength = 1.0f;
            Destroy(player, deathAnimLength);
        }
    }

    public void CameraShake(float intensity, float duration)
    {
        StartCoroutine(DoCameraShake(intensity, duration));
    }
    private IEnumerator DoCameraShake(float intensity, float duration)
    {
        Vector3 originalPos = mainCamera.transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float offsetX = UnityEngine.Random.Range(-intensity, intensity);
            float offsetY = UnityEngine.Random.Range(-intensity, intensity);
            mainCamera.transform.position = new Vector3(originalPos.x + offsetX, originalPos.y + offsetY, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null; // wait for next frame
        }
        mainCamera.transform.position = originalPos;
    }

    public static void AddWalker(Walker walker)
    {
        activeWalkers.Add(walker);
    }
    public static void RemoveWalker(Walker walker)
    {
        activeWalkers.Remove(walker);
    }

    public static bool IsAnyWalkerAttacking()
    {
        return activeWalkers.Any(w => w != null && w.toAttack);
    }
}

