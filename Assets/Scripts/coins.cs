using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

class Coins : MonoBehaviour
{
    public GameObject coinObject;
    public RectTransform coinBag;
    public Camera mainCamera;

    private List<GameObject> allCoins;
    private float xDist;
    private float yDist;
    private float dist;
    public float speed = 0.01f;

    void Update()
    {
        allCoins = FindGameObjectsInLayer(10);
        foreach (GameObject coin in  allCoins)
        {

            Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(mainCamera, coinBag.position);
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);

            xDist = worldPos.x - coin.transform.position.x;
            yDist = worldPos.y - coin.transform.position.y;
            dist = Vector2.Distance(worldPos, coin.transform.position);

            if (dist < 0.3f) 
            {
                Destroy(coin);
                return;
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

}