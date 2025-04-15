using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ArcherUpgrade : MonoBehaviour
{
    private ArcherTowerPlacement placement;
    private ArcherTowerAttack attack;
    private gameloop gameLoop;
    private List<GameObject> ArcherPositions;
    private Vector3 mousePos;
    private float searchRadius = 0.7f;
    public float upgradeCostMultiplier = 1.5f;
    public float attackDamageMultiplier = 1.5f;

    public Button upgradeButtton;
    public Text upgradeText;
    public Text damageText;
    public Text plusDamageText;
    public Text attackspeedText;
    public GameObject stastPanel;

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
            float dist = Vector2.Distance(mousePos, archer.transform.position  + new Vector3 (0, -1f, 0));
            if (dist <= searchRadius)
            {
                attack = archer.GetComponent<ArcherTowerAttack>();
                ShowButton(archer.transform.position);
                return;
            }
        }
        upgradeButtton.gameObject.SetActive(false);
        stastPanel.gameObject.SetActive(false);

    }

    void ShowButton(Vector3 archerPos)
    {

        upgradeButtton.gameObject.SetActive(true);
        stastPanel.gameObject.SetActive(true);
        upgradeButtton.transform.position = archerPos + new Vector3 (0, -1f, 0);
        attack = FindClosestObject(upgradeButtton.transform);
        upgradeText.text = $"cost: {attack.upgradeCost}";
        damageText.text = $"damage: {attack.attackDamage}";
        plusDamageText.text = $"plus damage: {attack.attackDamage * attackDamageMultiplier -attack.attackDamage}";
        attackspeedText.text = $"attack speed: {attack.attackCooldown}";
    }

    void OnUpgrade()
    {
        if (attack == null) return;
        if(attack.upgradeCost <= gameLoop.money)
        {
            gameLoop.money -= attack.upgradeCost;
            attack.attackDamage *= attackDamageMultiplier;
            attack.upgradeCost *= upgradeCostMultiplier;
            attack.attackDamage = Mathf.Round(attack.attackDamage * 100) / 100;
            attack.upgradeCost = Mathf.Round(attack.upgradeCost * 100) / 100;
        }
    }

    ArcherTowerAttack FindClosestObject(Transform reference)
    {
        ArcherTowerAttack[] allObjects = FindObjectsByType<ArcherTowerAttack>(FindObjectsSortMode.None);
        ArcherTowerAttack closest = null;
        float minDistance = Mathf.Infinity;

        foreach (ArcherTowerAttack obj in allObjects)
        {
            float distance = Vector3.Distance(reference.position, obj.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = obj;
            }
        }

        return closest;
    }

}