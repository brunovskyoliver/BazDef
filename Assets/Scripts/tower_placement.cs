using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class TowerPlacement : MonoBehaviour
{
    public GameObject ArcherTowerButton;
    public LayerMask targetLayer;

    private ArcherTowerPlacement archerTowerPlacement;

    void Start()
    {
        archerTowerPlacement = FindAnyObjectByType<ArcherTowerPlacement>();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Detect left mouse click
        {


            if (CheckObjectClicked("Archer_tower_button"))
            {
                archerTowerPlacement.isMouseOver = true;
                Debug.Log("clicked");
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
