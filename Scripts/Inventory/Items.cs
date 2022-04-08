using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items : MonoBehaviour
{
   // public Text keyText, weaponText;
    string name;
   // GameObject go;
   // Text description;
   // Image icon;
    public int id;

    public Items(string name, int id)
    {
        this.name = name;
       // this.go = go;
        //this.icon = icon;
        this.id = id;
    }

    public int getId()
    {
        return id;
    }

    public string toString()
    {
        return name;
    }
}
