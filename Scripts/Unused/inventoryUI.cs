using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public int slotNum = 0;

    public void Display(Image image, Text d)
    {
        if(slotNum <= 3)
        {
            Debug.Log("Called");
            image.enabled = true;
          //  d.SetActive(true);
            slotNum++;
        }
        else
        {
            Debug.Log("Inventory full");
        }
    }

    public void Remove(Image image, Text d)
    {
        if(slotNum > 0)
        {
            image.enabled = false;
           // d.SetActive(false);
            slotNum--;
        }
        else
        {
            Debug.Log("Inventory empty");
        }
    }
}
