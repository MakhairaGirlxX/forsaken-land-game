using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectType : MonoBehaviour
{
   public bool JanitorKeys = false;
   public bool Weapon = false;

    public int k = 0;
    public string v = "";

    void Update()
    {
        if(JanitorKeys)
        {
            k = 1;
            v = "Keys: Key Description";
        }

        else if(Weapon)
        {
            k = 2;
            v = "Weapon: Weapon Description";
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "JanitorKeys")
        {
            JanitorKeys = true;
        }

        else if(col.tag == "Weapon")
        {
            Weapon = true;
        }
    }
}
