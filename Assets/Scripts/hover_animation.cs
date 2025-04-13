using UnityEngine;

public class HoverAnimation : MonoBehaviour
{
    public RectTransform image;
    public RectTransform costText;
    private Vector2 startPosImg;
    private Vector2 startPosCost;
    public Vector2 hoverOffsetUI = new Vector2(0, 10f);
    public float animSpeed = 5f;  // mensie cislo -> pomalsie
    private Vector2 targetPosImg;
    private Vector2 targetPosCost;

    void Start()
    {
        startPosImg = image.anchoredPosition;
        startPosCost = costText.anchoredPosition;
        targetPosImg = startPosImg;
        targetPosCost = startPosCost;
    }

    void Update()
    {
        image.anchoredPosition = Vector2.Lerp(
            image.anchoredPosition, 
            targetPosImg, 
            Time.deltaTime * animSpeed
        );
        
        costText.anchoredPosition = Vector2.Lerp(
            costText.anchoredPosition, 
            targetPosCost, 
            Time.deltaTime * animSpeed
        );
    }

    void OnMouseEnter()
    {
        targetPosImg = startPosImg + hoverOffsetUI;
        targetPosCost = startPosCost + hoverOffsetUI;
    }

    void OnMouseExit()
    {
        targetPosImg = startPosImg;
        targetPosCost = startPosCost;
    }
}
