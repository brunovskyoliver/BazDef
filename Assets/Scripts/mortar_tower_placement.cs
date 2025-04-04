using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using Unity.VisualScripting;


public class MortarTowerPlacement : MonoBehaviour
{
    public Tilemap groundTilemap;
    public TileBase grassTile;
    private GameObject playerTower;
    private bool isDragging = false;
    private GameObject towerPreview;
    public Sprite towerSprite;
    public Sprite towerRangeSprite; 
    public Sprite towerRangePlacedSprite;
    public Sprite arrowSprite;
    private Camera mainCamera;
    private SpriteRenderer spriteRenderer;
    private Color validColor = new Color(1, 1, 1, 0.7f);
    private Color invalidColor = new Color(1, 0, 0, 0.7f); // red
    public bool isMouseOver = false;
    private float xOffset = 0.5f;
    private float yOffset = 0.75f;
    public HashSet<Vector2Int> towerPositions;
    public float towerRange; 
    public Color rangeColor = new Color(1f, 1f, 1f, 0.2f); 
    private float MortarTowerCost;
    private gameloop gameloopInstance; 

    void Start()
    {
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerTower = GameObject.Find("Player_tower");
        towerRange = level_settings.Instance.mortarTowerSettings.towerRange;
        MortarTowerCost = level_settings.Instance.mortarTowerSettings.cost;
        gameloopInstance = FindFirstObjectByType<gameloop>();
    }


    void Update()
    {
        if (gameloopInstance.money < MortarTowerCost) 
        {
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
            Transform archerPreview = towerPreview.transform.Find("Mortar");
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
        GameObject newTower = new GameObject("Mortar");
        SpriteRenderer sr = newTower.AddComponent<SpriteRenderer>();
        sr.sprite = towerSprite;
        newTower.transform.position = position;

        PurchaseTower();
        
        CreateTowerRange(newTower);
        newTower.AddComponent<MortarTowerTargeting>();
        newTower.AddComponent<MortarTowerAttack>();

        towerPositions.Add(gridPosition);
        CancelPlacement();
    }

    void PurchaseTower()
    {
        gameloopInstance.money -= MortarTowerCost;
        MortarTowerCost *= level_settings.Instance.mortarTowerSettings.costMultiplier;
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
}
