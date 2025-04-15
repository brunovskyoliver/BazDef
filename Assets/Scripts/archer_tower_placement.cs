using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.Mathematics;
using System;


public class ArcherTowerPlacement : MonoBehaviour
{
    public Tilemap groundTilemap;
    public TileBase grassTile;
    private GameObject playerTower;
    private bool isDragging = false;
    private GameObject towerPreview;
    public Sprite towerSprite;
    public Sprite towerRangeSprite; 
    public Material arrowTrailMaterial;
    public Sprite towerRangePlacedSprite;
    public Sprite towerArcherSprite;
    public RuntimeAnimatorController archerAnimator;
    public Sprite arrowSprite;
    public Vector3 archerSize = new Vector3(3,3,0);
    public Text costText;
    private Camera mainCamera;
    private SpriteRenderer spriteRenderer;
    private Color validColor = new Color(1, 1, 1, 0.7f);
    private Color invalidColor = new Color(1, 0, 0, 0.7f); // red
    public bool isMouseOver = false;
    private float xOffset = 0.5f;
    private float yOffset = 0.75f;
    public HashSet<Vector2Int> towerPositions;
    public float towerRange = 2f; 
    public Color rangeColor = new Color(1f, 1f, 1f, 0.2f);
    public float ArcherTowerCost;
    private gameloop gameloopInstance; 
    public List<GameObject> ArcherPositions = new List<GameObject>();
    private float startArcherTowerCost;
   

    public void Clear()
    {
        towerPositions.Clear();
        ArcherPositions.Clear();
        ArcherTowerCost = startArcherTowerCost;
        UpdateCostText();
        level_settings.Instance.ResetToDefault();
    }
    void Start()
    {
        startArcherTowerCost = level_settings.Instance.archerTowerSettings.cost;
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerTower = GameObject.Find("Player_tower");
        towerRange = level_settings.Instance.archerTowerSettings.towerRange;
        ArcherTowerCost = level_settings.Instance.archerTowerSettings.cost;
        gameloopInstance = FindFirstObjectByType<gameloop>();
        UpdateCostText();

    }


    void Update()
    {
        if (gameloopInstance.money < ArcherTowerCost) 
        {
            isMouseOver = false;
            return;
        }

        if (isMouseOver && Input.GetMouseButtonUp(0) && !isDragging) // toto zabezpeci ze sa prvotny klik nepocita ako kliknutie na place
        {
            isDragging = true;
            CreateTowerPreview();
            return;
        }
        if (isDragging && towerPreview != null)
        {
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = -1;

            mousePosition.x = Mathf.Floor(mousePosition.x) + xOffset;
            mousePosition.y = Mathf.Floor(mousePosition.y) + yOffset;
            
            towerPreview.transform.position = mousePosition;

            bool isValidPosition = CheckValidPosition(mousePosition);
            towerPreview.GetComponent<SpriteRenderer>().color = 
                isValidPosition ? validColor : invalidColor;
            Transform archerPreview = towerPreview.transform.Find("Archer");
            if (archerPreview != null)
            {
                Color archerColorCurrent = isValidPosition ? 
                    validColor : new Color(invalidColor.r, invalidColor.g, invalidColor.b, validColor.a);
                archerPreview.GetComponent<SpriteRenderer>().color = archerColorCurrent;
            }
            Transform rangeCircle = towerPreview.transform.Find("TowerPreviewRange");
            if (rangeCircle != null)
            {
                Color rangeColorCurrent = isValidPosition ? 
                    rangeColor : new Color(invalidColor.r, invalidColor.g, invalidColor.b, rangeColor.a);
                rangeCircle.GetComponent<SpriteRenderer>().color = rangeColorCurrent;
            }
            
            if (Input.GetMouseButtonDown(0) && isValidPosition)
            {
                PlaceTower(mousePosition);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                CancelPlacement();
            }
        }
    }

    void CreateTowerPreview()
    {
        towerPreview = new GameObject("TowerPreview");
        SpriteRenderer sr = towerPreview.AddComponent<SpriteRenderer>();
        sr.sprite = towerSprite;
        sr.color = validColor;
        CreateArcher(towerPreview, sr);
        CreateTowerPreviewRange(towerPreview);
    }

    void CreateTowerPreviewRange(GameObject tower)
    {
        GameObject rangeCircle = new GameObject("TowerPreviewRange");
        rangeCircle.layer = 7;
        rangeCircle.transform.SetParent(tower.transform);
        rangeCircle.transform.localPosition = Vector3.zero;

        SpriteRenderer rangeRenderer = rangeCircle.AddComponent<SpriteRenderer>();
        rangeRenderer.sprite = towerRangeSprite;
        rangeRenderer.color = rangeColor;
        rangeRenderer.sortingOrder = -2;
        float scaleFactor = towerRange;
        rangeCircle.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1);
    }
    void CreateTowerRange(GameObject tower)
    {
        GameObject rangeCircle = new GameObject("TowerRange");
        rangeCircle.layer = 7;
        rangeCircle.transform.SetParent(tower.transform);
        rangeCircle.transform.localPosition = Vector3.zero;
        SpriteRenderer rangeRenderer = rangeCircle.AddComponent<SpriteRenderer>();
        rangeRenderer.sprite = towerRangePlacedSprite;
        rangeRenderer.color = rangeColor;
        rangeRenderer.sortingOrder = -2;
        float scaleFactor = towerRange;
        rangeCircle.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1);
    }


    void CreateArcher(GameObject tower, SpriteRenderer towerRenderer)
    {
        GameObject archer = new GameObject("Archer");
        archer.transform.SetParent(tower.transform);
        archer.transform.localPosition = new Vector3(0, 0.5f, 0);
        archer.transform.localScale = archerSize;
        SpriteRenderer archerRenderer = archer.AddComponent<SpriteRenderer>();
        Animator archerAnim = archer.AddComponent<Animator>();
        archerAnim.runtimeAnimatorController = archerAnimator;
        archerRenderer.sprite = towerArcherSprite;
        archerRenderer.sortingOrder = 1;
        archerRenderer.color = towerRenderer.color;

    }

    bool CheckValidPosition(Vector3 position)
    {
        if (playerTower != null && Vector3.Distance(playerTower.transform.position, position) < 0.5f)
        {
            return false;
        }
        
        Vector3Int cellPosition = groundTilemap.WorldToCell(position);
        Vector2Int gridPosition = new Vector2Int(cellPosition.x, cellPosition.y);
        
        if (towerPositions.Contains(gridPosition))
        {
            return false;
        }
        
        TileBase tile = groundTilemap.GetTile(cellPosition);
        return tile == grassTile;
    }

    void PlaceTower(Vector3 position)
    {
        if (gameloop.Instance.waveStarted) 
        {
            Debug.Log("game started - cant place nomore");
            return;
        }
        Vector3Int cellPosition = groundTilemap.WorldToCell(position);
        Vector2Int gridPosition = new Vector2Int(cellPosition.x, cellPosition.y);
        
        if (towerPositions.Contains(gridPosition))
        {
            return;
        }
        GameObject newTower = new GameObject("Tower");
        newTower.tag = "Archer";
        SpriteRenderer sr = newTower.AddComponent<SpriteRenderer>();
        newTower.layer = 8;
        sr.sprite = towerSprite;
        newTower.transform.position = position;

        ArcherPositions.Add(newTower);

        PurchaseTower();
        
        CreateTowerRange(newTower);
        CreateArcher(newTower, sr);
        newTower.AddComponent<AcrherTowerTargeting>();
        newTower.AddComponent<ArcherTowerAttack>();
        towerPositions.Add(gridPosition);
        CancelPlacement();
    }

    void PurchaseTower()
    {
        gameloopInstance.money -= ArcherTowerCost;
        ArcherTowerCost *= level_settings.Instance.archerTowerSettings.costMultiplier;
        ArcherTowerCost = (float)Math.Round(ArcherTowerCost, 2);
        UpdateCostText();
    }

    void CancelPlacement()
    {
        if (towerPreview != null)
        {
            Destroy(towerPreview);
        }
        isDragging = false;
        isMouseOver = false;
    }

    void UpdateCostText()
    {
        costText.text = $"cost: {ArcherTowerCost}";
    }
}
