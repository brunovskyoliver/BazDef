using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

class Stats : MonoBehaviour
{

    public GameObject statsPanel;
    public RectTransform panelTransform;
    public Text archerStatsText;
    public Text mortarStatsText;
    public Button statsButton;
    public Button exitButton;

    private Vector2 targetSizeMax = new (130, 200);
    private Vector2 targetSizeMin = new (30, 30);
    public float animSpeed = 1f;

    private bool updateSize;

    public float allArcherDamage = 0;
    public float allMortarDamage = 0;

    void Start()
    {
        statsButton.gameObject.SetActive(true);

        statsButton.onClick.AddListener(OnButtonClick);
        exitButton.onClick.AddListener(OnExitButtonClick);

        statsPanel.gameObject.SetActive(false);

    }

    void Update()
    {
        if (updateSize)
        {
            panelTransform.sizeDelta = Vector2.Lerp(panelTransform.sizeDelta, targetSizeMax, Time.deltaTime * animSpeed);

            archerStatsText.text = $"Archer Total damage = {allArcherDamage}";
            mortarStatsText.text = $"Mortar Total damage = {allMortarDamage}";
        }
        else
        {
            panelTransform.sizeDelta = Vector2.Lerp(panelTransform.sizeDelta, targetSizeMin, Time.deltaTime * animSpeed);
        }
        
    }

    void OnButtonClick()
    {
        statsPanel.gameObject.SetActive(true);
        statsButton.gameObject.SetActive(false);

        panelTransform.sizeDelta = new Vector2(30,30);
        updateSize = true;

    }

    void OnExitButtonClick()
    {
        updateSize = false;
        StartCoroutine(WaitthenDestroy(0.35f));
    }

    IEnumerator WaitthenDestroy(float time)
    {
        yield return new WaitForSeconds(time);

        statsPanel.gameObject.SetActive(false);
        statsButton.gameObject.SetActive(true);
    }


}