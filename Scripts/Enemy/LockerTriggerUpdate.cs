using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Set up the same as Hidden.cs
 * Tells the enemy if the player is in the locker or not
 * They are safe if they are in the locker
 */
public class LockerTriggerUpdate : MonoBehaviour
{
    public bool inLocker = false;
    bool inRange = false;

    void Update()
    {
        if (inRange)
        {
            inLocker = true;
        }
        else if(!inRange)
        {
            inLocker = false;
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