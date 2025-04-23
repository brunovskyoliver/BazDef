using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;



public class MortarUpgrade : MonoBehaviour
{
    private MortarTowerPlacement placement;
    private MortarTowerAttack attack;
    private gameloop gameLoop;
    public List<GameObject> MortarPositions;
    private Vector3 mousePos;
    private float searchRadius = 0.8f;
    public float upgradeCostMultiplier = 1.5f;
    public float attackDamageMultiplier = 1.5f;

    public Button upgradeButtton;
    public Text upgradeText;
    public Text damageText;
    public Text plusDamageText;
    public Text attackspeedText;
    public GameObject stastPanel;

    private RectTransform buttonTransform;
    private RectTransform PanelTransform;
    private Vector2 buttonMin = new (1, 0.5f);
    private Vector2 buttonMax = new (50, 15f);
    private Vector2 PanelMin = new (1, 0.5f);
    private Vector2 PanelMax = new (115, 120);
    private float animSpeed = 12f;
    private bool isOver = false;

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
                if (!isOver)
                    ShowButton(mortar.transform.position);
                isOver = true;
                return;
            }
        }
        upgradeButtton.gameObject.SetActive(false);
        stastPanel.gameObject.SetActive(false);
        isOver = false;


    }

    void ShowButton(Vector3 MortarPos)
    {
        buttonTransform = upgradeButtton.GetComponent<RectTransform>();
        PanelTransform = stastPanel.GetComponent<RectTransform>();
        buttonTransform.sizeDelta = buttonMin;
        PanelTransform.sizeDelta = PanelMin;
        upgradeButtton.gameObject.SetActive(true);
        stastPanel.gameObject.SetActive(true);

        StartCoroutine(AnimateSize());

        upgradeButtton.transform.position = MortarPos + new Vector3 (0, -1f, 0);
        upgradeText.text = $"cost: {attack.upgradeCost}";
        damageText.text = $"damage: {attack.attackDamage}";
        plusDamageText.text = $"plus damage: {attack.attackDamage * attackDamageMultiplier -attack.attackDamage}";
        attackspeedText.text = $"attack speed: {attack.attackCooldown}";
    }

    IEnumerator AnimateSize()
    {
        while (Vector2.Distance(buttonTransform.sizeDelta, buttonMax) > 0.01f ||
            Vector2.Distance(PanelTransform.sizeDelta, PanelMax) > 0.01f)
        {
            buttonTransform.sizeDelta = Vector2.Lerp(buttonTransform.sizeDelta, buttonMax, Time.deltaTime * animSpeed);
            PanelTransform.sizeDelta = Vector2.Lerp(PanelTransform.sizeDelta, PanelMax, Time.deltaTime * animSpeed);

            yield return null;
        }

        buttonTransform.sizeDelta = buttonMax;
        PanelTransform.sizeDelta = PanelMax;

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

            upgradeText.text = $"cost: {attack.upgradeCost}";
            damageText.text = $"damage: {attack.attackDamage}";
            plusDamageText.text = $"plus damage: {attack.attackDamage * attackDamageMultiplier -attack.attackDamage}";
            attackspeedText.text = $"attack speed: {attack.attackCooldown}";

        }
    }

}