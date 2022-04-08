using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterRoomCollider : MonoBehaviour
{
    public bool inWideRange = false;
    bool inRange = false;

    void Update()
    {
        if (inRange == true)
        {
            inWideRange = true;
        }
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
