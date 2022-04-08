using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonFunctionDelegates : MonoBehaviour
{
    public GameObject weaponBtn, keyBtn;
    [SerializeField] private KeyCode useKey;

    ItemList item;

    private List<GameObject> btns = new List<GameObject>();

    void Start()
    {
        btns.Add(keyBtn);
        btns.Add(weaponBtn);

        foreach (GameObject i in btns)
        {
            int btnNum = 0;

            Button iBtn = i.GetComponent<Button>();
            iBtn.onClick.AddListener(delegate { DisplayItem(btnNum); });

            btnNum++;
        }
    }

    public void DisplayItem(int id)
    {
      //  item.DisplayText(id);

     //   CheckForUse(id);
    }

    /*
    public void CheckForUse(int id)
    {
        if(Input.GetKeyDown(useKey))
        {
            item.RemoveItems(id);
        }
    }
    */


}
