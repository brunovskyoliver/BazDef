using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;


public class gameloop : MonoBehaviour
{
    public Sprite walkerSkin;

    int spawnedmetys = 0;
    const int numOfMetods = 5;
    const int timeDelay = 1500; // v milisekundach
    bool spawnMety = true;

    Stopwatch timer = new Stopwatch();
    
    void Start()
    {
        UnityEngine.Debug.Log("Game Started");

        timer.Start();
    }

    void Update()
    {
        if (spawnMety)
        {
            if (timer.ElapsedMilliseconds > timeDelay)
            {
                timer.Restart();
                GameObject newWalker = new GameObject("Mety");
                SpriteRenderer renderer = newWalker.AddComponent<SpriteRenderer>();
                renderer.sprite = walkerSkin;
                newWalker.AddComponent<Walker>();

                if (spawnedmetys >= numOfMetods) spawnMety = false;
                spawnedmetys ++;

            }
        }
    }



}
