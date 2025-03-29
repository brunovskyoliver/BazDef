using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;


public class gameloop : MonoBehaviour
{
    public Sprite walkerSkin;
    private Animator animator;
    public  RuntimeAnimatorController Mety;

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
                newWalker.AddComponent<SpriteRenderer>();
                newWalker.AddComponent<Walker>();
                newWalker.AddComponent<Animator>();
                animator = newWalker.GetComponent<Animator>();
                animator.runtimeAnimatorController = Mety; 

                if (spawnedmetys >= numOfMetods) spawnMety = false;
                spawnedmetys ++;

            }
        }
    }



}
