using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Tells the NewEnemyAI script if the player is hidden behind something or not
 * This script is placed on any hiding spots which allows for flexibility when generating multiple hiding spots
 * OnTrigger calls the objects box collider object (mesh render unchecked in the inspector so it isn't seen)
 */
public class Hidden : MonoBehaviour
{
    public bool hidden = false;
    bool inRange = false;

    //keeps the updated value of inRange on a frame by frame basis
    void Update()
    {
        if (inRange)
        {
            hidden = true;
        }
        else
        {
            hidden = false;
        }
    }
    //OnTriggerStay keeps inRange as true for however long the player is 'hiding'
    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            inRange = true;
        }
    }

    //OnTriggerEnter is when the player first enters the collider
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            inRange = true;
        }
    }
    //When the player exits the collider
    void OnTriggerExit(Collider col)
    {
        inRange = false;
    }
}
