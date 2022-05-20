using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
 * This script is set up as the main driver of the inventory system. It stores inventory items, assigns them id's and names, and updates the list when new items are added
 * It also disables and enables the EnemyJanitorRoom.cs script when the "key" object is collected
 * Remove is not currently being used but is set up for future use
 * Any commented out code was from testing the flexibility of the code with multiple inventory items (left in for future reference)
 */

public class ItemList : MonoBehaviour
{
    private List<Items> inventory;

    public Text hidePrompt;
    public Image keyImage, weaponImage;
    public GameObject keyobj, enemy, player, door;

    public KeyCode interactKey;

    bool keyBool = false;
    public bool stop = false;
    // bool weaponBool = false;

    Animator doorAnim;
    //Items are from the script Items.cs
    Items i1;
    Items i2;

    int index = 0;
   
    void Start()
    {
        //generate an empty list of the Items object
        inventory = new List<Items>();
        //set the name and the value
        i1 = new Items("Key", 0);
       // i2 = new Items("Weapon", 1);
       
        hidePrompt.enabled = false;
    }

    void Update()
    {
        //if the player is in the janitor room and gets respawned, set the enemy to inactive
        //had to call this from an external script to ensure the enemy is reset properly
        if (enemy.GetComponent<EnemyJanitorRoom>().restarted)
        {
            enemy.SetActive(false);
            enemy.GetComponent<EnemyJanitorRoom>().enabled = false;
            StartCoroutine(WaitToStart());
        }
        //if the player interacts with an inventory item
        if (Input.GetKeyDown(interactKey))
        {
            //if the inventory isn't full
            if (inventory.Count <= 3)
            {
                //if the object is a key
                if (keyBool == true)
                {
                    //add it to the inventory list
                    inventory.Add(i1);
                    //make the key object disappear (as if it's going into the players inventory)
                    keyobj.SetActive(false);
                    //enable the key image in the inventory
                    keyImage.enabled = true;
                    //testing
                    Debug.Log("Key: " + inventory.FindIndex(item => item.id == i1.id));
                    //finds the index of the key item (the keys id)
                    index = inventory.FindIndex(item => item.id == i1.id);
                    //sets the position of the key image to that index
                    SetPosition(keyImage, index);

                    //tells the player to hide once the key is collected
                    hidePrompt.enabled = true;
                    StartCoroutine(HidePrompt());
                }
                /*
                else if (weaponBool == true)
                {
                    inventory.Add(i2);
                    weaponobj.SetActive(false);
                    weaponImage.enabled = true;
                    Debug.Log("Weapon: " + inventory.FindIndex(item => item.id == i2.id));
                    index = inventory.FindIndex(item => item.id == i2.id);
                    SetPosition(weaponImage, index);
                }
                */
            }
            else
            {
                Debug.Log("Inventory Full");
            }
        }
    }
    //waits before setting the enemy to active in EnemyJanitorRoom.cs 
    IEnumerator WaitToStart()
    {
        yield return new WaitForSeconds(10f);
        enemy.SetActive(true);
        enemy.GetComponent<EnemyJanitorRoom>().enabled = true;
    }
    //
    IEnumerator HidePrompt()
    {
        yield return new WaitForSeconds(10f);
        //tells other scripts that the keys have been collected
        stop = true;
        //the switch from one enemy script to another as the enemy enters the janitor room
        enemy.GetComponent<NewEnemyAI>().enabled = false;
        enemy.GetComponent<EnemyJanitorRoom>().enabled = true;
        //can still access the key object even if it cannot be seen so it has to be destroyed
        Destroy(keyobj);
        hidePrompt.enabled = false;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "JanitorKeys")
        {
            Debug.Log("picked up janitor keys");
            keyBool = true;
        }
        /*
        else if (col.tag == "Weapon")
        {          
            weaponBool = true;
        }
        */
    }

    //return item with this id
    public Items CheckForItem(string name)
    {
        return inventory.Find(item => item.name == name);
    }
    //allows for objects to be set to certain inventory positions depending on which one was collected first
    public void SetPosition(Image image, int index)
    {
        //if the object collected has the index of 0, set it to a specific position in the inventory otherwise set to the second position
        if (index == 0)
        {
            Debug.Log("Added to position 1");      
            image.transform.position = new Vector2(440, 600);
        } else if(index == 1)
        {
            Debug.Log("Added to position 2");
            image.transform.position = new Vector2(406, 392);
        }
    }
    //Not being used but set up for future use
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
