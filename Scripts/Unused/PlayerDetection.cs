using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    public KeyCode interactKey;
    bool inRange = false;

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            if (inRange)
            {
                GameObject.Find("Player").GetComponent<ItemList>().enabled = true;
            }           
        }
    }

   private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            inRange = true;
        }
    }

   private void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player")
        {
            inRange = false;
        }
    }
}
