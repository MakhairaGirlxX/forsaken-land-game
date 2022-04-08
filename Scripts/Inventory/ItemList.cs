using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ItemList : MonoBehaviour
{
    private List<Items> inventory;

    public Image keyImage, weaponImage;
    public GameObject keyobj, weaponobj;

    public KeyCode interactKey;

    InventoryUI system;

    bool keyBool = false;
    bool weaponBool = false;

    Items i1;
    Items i2;

    int index = 0;
   
    void Start()
    {
        inventory = new List<Items>();

        i1 = new Items("Key", 0);
        i2 = new Items("Weapon", 1);
    }

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            if (inventory.Count <= 3)
            {
                if (keyBool == true)
                {
                    inventory.Add(i1);
                    keyobj.SetActive(false);
                    keyImage.enabled = true;
                    Debug.Log("Key: " + inventory.FindIndex(item => item.id == i1.id));
                    index = inventory.FindIndex(item => item.id == i1.id);
                    SetPosition(keyImage, index);
                }

                else if (weaponBool == true)
                {
                    inventory.Add(i2);
                    weaponobj.SetActive(false);
                    weaponImage.enabled = true;
                    Debug.Log("Weapon: " + inventory.FindIndex(item => item.id == i2.id));
                    index = inventory.FindIndex(item => item.id == i2.id);
                    SetPosition(weaponImage, index);
                }
            }
            else
            {
                Debug.Log("Inventory Full");
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "JanitorKeys")
        {
            keyBool = true;
        }

        else if (col.tag == "Weapon")
        {          
            weaponBool = true;
        }
    }

    //return item with this id
    public Items CheckForItem(string name)
    {
        return inventory.Find(item => item.name == name);
    }

    public void SetPosition(Image image, int index)
    {
        if (index == 0)
        {
            Debug.Log("Added to position 1");      
            image.transform.position = new Vector2(263, 392);
        } else if(index == 1)
        {
            Debug.Log("Added to position 2");
            image.transform.position = new Vector2(406, 392);
        }
    }

    public void RemoveItems(string name)
    {
        Items item = CheckForItem(name);

        if(item != null)
        {
            inventory.Remove(item);
            
            Debug.Log("Item Removed: ");
        }

        Debug.Log("Item not Found");
    }
    
}
