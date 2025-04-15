using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MortarUpgrade : MonoBehaviour
{
    private MortarTowerPlacement placement;
    private MortarTowerAttack attack;
    private gameloop gameLoop;
    public List<GameObject> MortarPositions;
    private Vector3 mousePos;
    private float searchRadius = 1f;
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
        placement = FindFirstObjectByType<MortarTowerPlacement>();
        gameLoop = FindFirstObjectByType<gameloop>();
        MortarPositions = placement.MortarPositions;
        upgradeButtton.onClick.AddListener(OnUpgrade);
    }

    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        foreach (GameObject mortar in MortarPositions)
        {
            float dist = Vector2.Distance(mousePos, mortar.transform.position + new Vector3 (0, -1f, 0));
            if (dist <= searchRadius)
            {
                attack = mortar.GetComponent<MortarTowerAttack>();
                ShowButton(mortar.transform.position);
                return;
            }
        }
        upgradeButtton.gameObject.SetActive(false);
        stastPanel.gameObject.SetActive(false);

    }

    void ShowButton(Vector3 MortarPos)
    {
        upgradeButtton.gameObject.SetActive(true);
        stastPanel.gameObject.SetActive(true);
        upgradeButtton.transform.position = MortarPos + new Vector3 (0, -1f, 0);
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

}