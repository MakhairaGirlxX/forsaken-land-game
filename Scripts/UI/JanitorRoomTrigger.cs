using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JanitorRoomTrigger : MonoBehaviour
{
    /*
     * Tells the enemy if the player is in the Janitor's office
     * */
    public bool inJanitorRoom = false;
    bool inRange = false;

    void Update()
    {
        if (inRange)
        {
            inJanitorRoom = true;
        }
        else
        {
            inJanitorRoom = false;
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
