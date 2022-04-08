using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hidden : MonoBehaviour
{
    public bool hidden = false;
    bool inRange = false;

    void Update()
    {
        if (inRange)
        {
            hidden = true;
        }
        else
        {
            hidden = false;
          //  Debug.Log("Not hidden");
        }
    }
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            inRange = true;
            Debug.Log("hidden");
        }
    }

    void OnTriggerExit(Collider col)
    {
        inRange = false;
    }
}
