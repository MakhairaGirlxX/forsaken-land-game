using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Item object generated and defined in ItemList.cs
 */
public class Items : MonoBehaviour
{
    string name;
    public int id;
    //sets name and id
    public Items(string name, int id)
    {
        this.name = name;
        this.id = id;
    }
    //gets object id
    public int getId()
    {
        return id;
    }
    //gets object name
    public string toString()
    {
        return name;
    }
}
