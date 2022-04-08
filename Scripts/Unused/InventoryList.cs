using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryList : MonoBehaviour
{
    public GameObject keyobj, weaponobj;
    Image image;
    Sprite keyImage, weaponImage;
    string description;
    private Dictionary<int, string> item;
    InventoryUI inventory;

    void Start()
    {
        keyImage = Resources.Load<Sprite>("Assets/Images/Key.jpg");
        weaponImage = Resources.Load<Sprite>("Assets/Images/Weapon.jpg");
        item = new Dictionary<int, string>();
    }

    public void StoreObject()
    {
        item.Add(GameObject.Find("Player").GetComponent<ObjectType>().k, GameObject.Find("Player").GetComponent<ObjectType>().v);

        if (item.ContainsKey(1))
        {
            keyobj.gameObject.SetActive(false);
            image.sprite = keyImage;
            description = item[1];
   //         inventory.Display(image, description);
        }

        else if (item.ContainsKey(2))
        {
            weaponobj.gameObject.SetActive(false);
            image.sprite = weaponImage;
            description = item[2];
        //    inventory.Display(image, description);
        }

        foreach (KeyValuePair<int, string> pair in item)
        {
            Debug.Log(pair.Key + ", " + pair.Value);
        }

        Debug.Log(item.Count);
    }

    public void RemoveObject()
    {

    }
}
