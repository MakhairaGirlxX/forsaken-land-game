using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PhoneBehavior : MonoBehaviour {

	public GameObject homeScreen, inventoryScreen, inventoryBtn, flashlightBtn, weaponBtn, keyBtn;
	[SerializeField] private KeyCode inventoryKey, backKey, useKey, deleteKey;
	public Text keyText, weaponText;

	ItemList item = new ItemList();

	public GameObject hand, phone;
	Animator inventoryAnim;

	public Material lockScreenMat, homeScreenMat, offScreenMat;

	private List<GameObject> btns = new List<GameObject>();

	void Start()
	{
		inventoryAnim = hand.GetComponent<Animator>();

		Button iBtn = inventoryBtn.GetComponent<Button> ();
		iBtn.onClick.AddListener(OpenInventory);

		Button fBtn = flashlightBtn.GetComponent<Button>();
		fBtn.onClick.AddListener(OpenFlashlight);

		btns.Add(keyBtn);
		btns.Add(weaponBtn);

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
		if ((Input.GetKeyDown(inventoryKey) || Input.GetKeyDown(backKey)) && homeScreen.gameObject.activeSelf == true)
		{
			homeScreen.gameObject.SetActive(false);
			inventoryScreen.gameObject.SetActive(false);
			inventoryAnim.SetTrigger("InventoryClosed");
			hand.SetActive(true);

			Time.timeScale = 1;
			Debug.Log("Not Active");
		}

		else if ((Input.GetKeyDown(inventoryKey) && homeScreen.gameObject.activeSelf == false))
		{
			StartCoroutine(HomeAnimation());

		}
        else if(Input.GetKeyDown(backKey) && inventoryScreen.activeSelf == true)
        {
			hand.SetActive(false);
			CloseInventory();
        }
	}

	void OpenInventory()
	{
		homeScreen.gameObject.SetActive (false);
		inventoryScreen.gameObject.SetActive(true);

		EventSystem.current.SetSelectedGameObject(null);
		//sets weapon button as the first selected button. This may be an issue later on as objects are added or deleted. A solution would be to have the first object the player obtains cannot be deleted and therefore will always occupy the first slot.
		EventSystem.current.SetSelectedGameObject(weaponBtn);
	}

	void CloseInventory()
    {
		phone.GetComponent<Renderer>().material = offScreenMat;
		homeScreen.gameObject.SetActive(true);
		inventoryScreen.gameObject.SetActive(false);
		Time.timeScale = 0;

		Debug.Log("Active");
		EventSystem.current.SetSelectedGameObject(null);
		EventSystem.current.SetSelectedGameObject(inventoryBtn);
	}

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

	void OpenFlashlight()
    {


    }

	public void DisplayItem(string name)
	{
		if(name != null)
        {
			if(name == "Key")
            {
				keyText.enabled = true;
				weaponText.enabled = false;
			}

			else if(name == "Weapon")
            {
				weaponText.enabled = true;
				keyText.enabled = false;
			}
			
			CheckForUse(name);
		}
		
		Debug.Log("No items");
		
	}

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
			else if(name == "Weapon")
            {
				weaponText.enabled = false;
            }
		}
	}
}


