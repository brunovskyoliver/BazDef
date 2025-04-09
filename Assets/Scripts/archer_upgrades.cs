using System;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class ArcherUpgrade : MonoBehaviour
{
    private ArcherTowerPlacement placement;
    private ArcherTowerAttack attack;
    private gameloop gameLoop;
    private List<GameObject> ArcherPositions;
    private Vector3 mousePos;
    private float searchRadius = 1.5f;
    public float upgradeCostMultiplier = 1.5f;
    public float attackDamageMultiplier = 1.5f;

    public Button upgradeButtton;
    public Text upgradeText;

    void Start()
    {
        placement = FindFirstObjectByType<ArcherTowerPlacement>();
        gameLoop = FindFirstObjectByType<gameloop>();
        ArcherPositions = placement.ArcherPositions;
        upgradeButtton.onClick.AddListener(OnUpgrade);
    }

    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);


        foreach (GameObject archer in ArcherPositions)
        {
            Vector3 archerPos = archer.transform.position;
            if (archerPos.x < mousePos.x+searchRadius && archerPos.x > mousePos.x-searchRadius)
            {
                if (archerPos.y < mousePos.y+searchRadius && archerPos.y > mousePos.y-searchRadius)
                {
                    ShowButton(archerPos);
                    return;
                }
            }
        }
        upgradeButtton.transform.position = new Vector3 (100, -1f, 0);

    }

    void ShowButton(Vector3 archerPos)
    {
        attack = FindFirstObjectByType<ArcherTowerAttack>();
        upgradeButtton.transform.position = archerPos + new Vector3 (0, -1f, 0);
        upgradeText.text = $"cost: {attack.upgradeCost}";

    }

    void OnUpgrade()
    {
        if(attack.upgradeCost <= gameLoop.money)
        {
            gameLoop.money -= attack.upgradeCost;
            attack.attackDamage *= attackDamageMultiplier;
            attack.upgradeCost *= upgradeCostMultiplier;
        }
    }

}