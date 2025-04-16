using UnityEngine;
using System.Collections.Generic;


public class TowerPlacement : MonoBehaviour
{
    public GameObject ArcherTowerButton;
    public GameObject MortarTowerButton;
    public LayerMask targetLayer;

    private ArcherTowerPlacement archerTowerPlacement;
    private MortarTowerPlacement mortarTowerPlacement;

    private HashSet<Vector2Int> towerPositions = new HashSet<Vector2Int>();


    void Start()
    {
        archerTowerPlacement = FindAnyObjectByType<ArcherTowerPlacement>();
        archerTowerPlacement.towerPositions = towerPositions;
        mortarTowerPlacement = FindAnyObjectByType<MortarTowerPlacement>();
        mortarTowerPlacement.towerPositions = towerPositions;
        
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Detect left mouse click
        {


            if (CheckObjectClicked("Archer_tower_button"))
            {
                archerTowerPlacement.isMouseOver = true;
                //Debug.Log("clicked");
            }
            else if (CheckObjectClicked("Mortar_tower_button"))
            {
                mortarTowerPlacement.isMouseOver = true;
                //Debug.Log("clicked");
            }
        }
        
    }

    bool CheckObjectClicked(string targetObjectName)
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, targetLayer);

        if (hit.collider != null)
        {
            if (hit.collider.gameObject.name == targetObjectName)
            {
                return true;
            }
        }

        return false;
    }

    
}
