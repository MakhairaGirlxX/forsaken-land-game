using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherSide : MonoBehaviour
{
    public bool otherSide = false;
    bool inRange = false;

    void Update()
    {
        if (inRange)
        {
            otherSide = true;
        }
        else
        {
            otherSide = false;
            //  Debug.Log("Not hidden");
        }
    }
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            inRange = true;
            Debug.Log("other side");
        }
    }

    void OnTriggerExit(Collider col)
    {
        inRange = false;
    }
}
