using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Tells the enemy when the player is in the same room as the enemy
 * Used as a fail safe in case the player is in range of the enemies box collider but not in the room so the enemy won't follow the player
 */
public class EnterRoomCollider : MonoBehaviour
{
    public bool inWideRange = false;
    bool inRange = false;

    void Update()
    {
        //subscribes to the onDeath delegate in PlayerMove.cs
        PlayerMove.onDeath += Reset;

        if (inRange == true)
        {
            inWideRange = true;
        }
    }
    //if the player dies, the range is set to false so it seems as if the player never entered the room when they respawn
    void Reset()
    {
        inWideRange = false;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            inRange = true;
            Debug.Log("seen by wide collider");
        }
    }

    void OnTriggerExit(Collider col)
    {
        inRange = false;
    }
}
