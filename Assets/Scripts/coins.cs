using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


class Coins : MonoBehaviour
{
    public Sprite coinSprite;
    public RectTransform coinText;
    public GameObject coinChest;
    public Sprite openChest;
    public Sprite closedChest;

    public Camera mainCamera;
    private gameloop gameloopInstance;

    private List<GameObject> allCoins;
    private float xDist;
    private float yDist;
    private float dist;
    public float speed = 0.01f;
    private SpriteRenderer chestSr;

    void Start()
    {
        gameloopInstance = FindFirstObjectByType<gameloop>();
        chestSr = coinChest.AddComponent<SpriteRenderer>();
        chestSr.sprite = closedChest;
    }
    void Update()
    {
        allCoins = FindGameObjectsInLayer(10);
        foreach (GameObject coin in  allCoins)
        {

            Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(mainCamera, coinText.position);
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);

            xDist = worldPos.x - coin.transform.position.x;
            yDist = worldPos.y - coin.transform.position.y;
            dist = Vector2.Distance(worldPos, coin.transform.position);

            if (dist < 0.7f) 
            {
                Destroy(coin);
                gameloopInstance.money +=1;
                if (allCoins.Count == 1)
                    chestSr.sprite = closedChest;
                return;
            }
            else
            {
                chestSr.sprite = openChest;
            }
            

            dist *= dist;
            coin.transform.position = coin.transform.position + new Vector3(xDist/dist * speed, yDist/dist * speed, 0);

            
        }
    }

    public List<GameObject> FindGameObjectsInLayer(int layer)
    {
        GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        List<GameObject> result = new List<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == layer)
            {
                result.Add(obj);
            }
        }

        return result;
    }

    public void SpawnCoins(int n, Vector3 position)
    {
        StartCoroutine(Spawn(n, position));

    }

    private IEnumerator Spawn(int n, Vector3 position)
    {
        for (int i = 0; i < n; i++)
        {
            GameObject newCoin = new GameObject("Coin");
            newCoin.transform.position = position;
            newCoin.layer = 10;
            

            SpriteRenderer sr = newCoin.AddComponent<SpriteRenderer>();
            sr.sprite = coinSprite;
            sr.sortingOrder = 500;

            yield return new WaitForSeconds(0.15f);
        }
    }

  


}