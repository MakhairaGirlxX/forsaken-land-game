using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*
 * Handles all the phone behavior
 * Right now it only works with the inventory but is set up for flexible use
 * Items collected around the level are displayed and handled here
 * Some test code for future use
 */
public class PhoneBehavior : MonoBehaviour {

	public GameObject homeScreen, inventoryScreen, inventoryBtn, flashlightBtn, weaponBtn, keyBtn, mapBtn;
	[SerializeField] private KeyCode inventoryKey, backKey, useKey, deleteKey;
	public Text keyText;
	//pulls from ItemList.cs
	ItemList item = new ItemList();

	public GameObject hand, phone;
	Animator inventoryAnim;

	public Material lockScreenMat, homeScreenMat, offScreenMat;

	private List<GameObject> btns = new List<GameObject>();

	void Start()
	{
		//get animator 
		inventoryAnim = hand.GetComponent<Animator>();

		//create button based on the button set in the inspector (inventoryBtn)
		Button iBtn = inventoryBtn.GetComponent<Button> ();
		//listener to determine what action to take when the button is pressed
		iBtn.onClick.AddListener(OpenInventory);

		//inactive, for testing purposes only with multiple buttons
		Button fBtn = flashlightBtn.GetComponent<Button>();
		fBtn.onClick.AddListener(OpenFlashlight);
		//same as above (inactive)
		Button mBtn = mapBtn.GetComponent<Button>();
		//add inspector defined buttons to list (items in the inventory)
		btns.Add(keyBtn);

		//adds a listener to inspector defined buttons (items in the inventory)
		foreach (GameObject i in btns)
		{
			int btnNum = 0;

			//btnNum = item.findIndex();
			Button inBtn = i.GetComponent<Button>();
			inBtn.onClick.AddListener(delegate { DisplayItem(EventSystem.current.currentSelectedGameObject.name); });
		}

	}

	void Update()
	{
		//if the inventory key is pressed or the back button and the homescreen is active
		if ((Input.GetKeyDown(inventoryKey) || Input.GetKeyDown(backKey)) && homeScreen.gameObject.activeSelf == true)
		{
			//close inventory and homescreen and reset hand position
			homeScreen.gameObject.SetActive(false);
			inventoryScreen.gameObject.SetActive(false);
			inventoryAnim.SetTrigger("InventoryClosed");
			hand.SetActive(true);
			//resumes time
			Time.timeScale = 1;
		}
		//if the inventory key is pressed and the homescreen isn't active
		else if ((Input.GetKeyDown(inventoryKey) && homeScreen.gameObject.activeSelf == false))
		{
			//open the phone (run phone animation)
			StartCoroutine(HomeAnimation());

		}
		//if the back key is pressed and the inventory is active
        else if(Input.GetKeyDown(backKey) && inventoryScreen.activeSelf == true)
        {
			//close inventory
			hand.SetActive(false);
			CloseInventory();
        }
	}
	//opens the inventory
	void OpenInventory()
	{
		homeScreen.gameObject.SetActive (false);
		inventoryScreen.gameObject.SetActive(true);

		EventSystem.current.SetSelectedGameObject(null);
		//sets the first button to active (highlighted)
		EventSystem.current.SetSelectedGameObject(keyBtn);
	}
	//closes inventory
	void CloseInventory()
    {
		phone.GetComponent<Renderer>().material = offScreenMat;
		homeScreen.gameObject.SetActive(true);
		inventoryScreen.gameObject.SetActive(false);
		//paused
		Time.timeScale = 0;

		EventSystem.current.SetSelectedGameObject(null);
		//highlights first button on phone screen
		EventSystem.current.SetSelectedGameObject(inventoryBtn);
	}
	//sets the animation for opening the phone initially
	IEnumerator HomeAnimation()
    {
		
		Debug.Log("Animation");
		inventoryAnim.SetTrigger("InventoryOpen");

		yield return new WaitForSeconds(1.5f);
	    phone.GetComponent<Renderer>().material = lockScreenMat;

		yield return new WaitForSeconds(0.65f);
		phone.GetComponent<Renderer>().material = homeScreenMat;

		yield return new WaitForSeconds(0.5f);
		CloseInventory();
	}
	//future use
	void OpenFlashlight()
    {


    }
	//displays inventory item description depending on which object is selected
	public void DisplayItem(string name)
	{
		if(name != null)
        {
			if(name == "Key")
            {
				Debug.Log("Key Name Triggered");
				keyText.enabled = true;
				//disable all other inventory item text
				//weaponText.enabled = false;
			}
			/*
			else if(name == "Weapon")
            {
				Debug.Log("Weapon Name Triggered");
				weaponText.enabled = true;
				keyText.enabled = false;
			}
			*/
			
			CheckForUse(name);
		}
		
		Debug.Log("No items");
		
	}
	//for future use to delete items
	public void CheckForUse(string name)
	{
		if (Input.GetKeyDown(deleteKey))
		{
			Debug.Log("Remove");
			item.RemoveItems(name);

			if(name == "Key")
            {
				keyText.enabled = false;
            }
			/*
			else if(name == "Weapon")
            {
				weaponText.enabled = false;
            }
			*/
		}
	}
}


