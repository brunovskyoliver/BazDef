using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TowerPlacement : MonoBehaviour
{
    public Tilemap groundTilemap;
    public TileBase grassTile;
    private GameObject playerTower;
    private bool isDragging = false;
    private GameObject towerPreview;
    public Sprite towerSprite;
    private Camera mainCamera;
    private SpriteRenderer spriteRenderer;
    private Color validColor = new Color(1, 1, 1, 0.7f);
    private Color invalidColor = new Color(1, 0, 0, 0.7f); // red
    private bool isMouseOver = false;
    private float xOffset = 0.5f;
    private float yOffset = 0.75f;
    private HashSet<Vector2Int> towerPositions = new HashSet<Vector2Int>();

    void Start()
    {
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerTower = GameObject.Find("Player_tower");
    }

    void OnMouseEnter()
    {
        isMouseOver = true;
    }

    void OnMouseExit()
    {
        isMouseOver = false;
    }

    void Update()
    {
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
        Vector3Int cellPosition = groundTilemap.WorldToCell(position);
        Vector2Int gridPosition = new Vector2Int(cellPosition.x, cellPosition.y);
        
        if (towerPositions.Contains(gridPosition))
        {
            return;
        }
        
        GameObject newTower = new GameObject("Tower");
        SpriteRenderer sr = newTower.AddComponent<SpriteRenderer>();
        sr.sprite = towerSprite;
        newTower.transform.position = position;
        
        towerPositions.Add(gridPosition);
        CancelPlacement();
    }

    void CancelPlacement()
    {
        if (towerPreview != null)
        {
            Destroy(towerPreview);
        }
        isDragging = false;
    }
}
