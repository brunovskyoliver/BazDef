using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

class OrderInLayer : MonoBehaviour
{
    private float highestPoint = 10f;

    void Update()
    {
        GameObject[] gameObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach(GameObject obj in gameObjects)
        {
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();

            if (sr != null && obj.layer != 7 && obj.layer != 8)
            {
                if (obj.transform.parent == null)
                {
                    sr.sortingOrder = (int)(highestPoint - obj.transform.position.y);
                }
                else 
                {
                    SpriteRenderer parentSr = obj.transform.parent.GetComponent<SpriteRenderer>();
                    sr.sortingOrder = parentSr.sortingOrder + 1;
                }
            }


        }
    }


}