using UnityEngine;

public class TowerPlacement : MonoBehaviour
{
    private bool isDragging = false;
    private GameObject towerPreview;
    public Sprite towerSprite;
    private Camera mainCamera;
    private SpriteRenderer spriteRenderer;
    private Color validColor = new Color(1, 1, 1, 0.7f);
    private Color invalidColor = new Color(1, 0, 0, 0.7f);
    private bool isMouseOver = false;
    private float yOffset = 2.50f;

    void Start()
    {
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
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

            // Zarovnanie na grid
            mousePosition.x = Mathf.Floor(mousePosition.x) + 0.5f;
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
        return true;
    }

    void PlaceTower(Vector3 position)
    {
        GameObject newTower = new GameObject("Tower");
        SpriteRenderer sr = newTower.AddComponent<SpriteRenderer>();
        sr.sprite = towerSprite;
        newTower.transform.position = position;
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
