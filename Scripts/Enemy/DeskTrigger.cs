using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeskTrigger : MonoBehaviour
{
    /*
     * Tells the enemy if the player is under the desk or not. If they are, they will eventually be found
     * Set up the same as Hidden.cs
     */
    public bool underDesk = false;
    bool inRange = false;

    void Update()
    {
        if (inRange)
        {
            underDesk = true;
        }
        else
        {
            underDesk = false;
        }
    }
    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            inRange = true;
        }
    }
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            inRange = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        inRange = false;
    }
}
