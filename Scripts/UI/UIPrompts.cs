using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPrompts : MonoBehaviour
{
    public Text doorPrompt, peekPrompt, janitorPrompt;
    public GameObject door_1, door_2, peekTrigger;
    bool doorRange = false;
    bool doorRange_2 = false;
    public bool doorOpen = false;
    [SerializeField] private KeyCode useKey;
    Animator doorAnim, doorAnim_2;
    int useTime = 0;
    int useTime_2 = 0;
    private CharacterController charController;

    // Start is called before the first frame update
    void Start()
    {
        doorPrompt.enabled = false;
        peekPrompt.enabled = false;
        janitorPrompt.enabled = false;

        doorAnim = door_1.GetComponent<Animator>();
        doorAnim_2 = door_2.GetComponent<Animator>();

        charController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(doorRange == true && Input.GetKeyDown(useKey) && useTime == 0)
        {
            doorAnim.SetTrigger("DoorOpen");
            useTime = 1;
        }
        if(doorRange_2 == true && Input.GetKeyDown(useKey))
        {
            DoorActions();
        }
       
    }

    public void DoorActions()
    {        
        useTime_2++;

        if(useTime_2 <= 1)
        {
            doorAnim_2.SetTrigger("DoorJanOpen");
            Debug.Log("Door open");
            doorOpen = true;
        }

        if(useTime_2 > 1)
        {
            doorAnim_2.SetTrigger("DoorJanClose");
            Debug.Log("Door closed");
            doorOpen = false;
            useTime_2 = 0;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "DoorTrigger" && useTime == 0)
        {
            Debug.Log("Yes");
            doorPrompt.enabled = true;
            doorRange = true;
            StartCoroutine(PromptTime());
        } else if(col.tag == "DoorTrigger_2")
        {
            doorRange_2 = true;
        } 
        else if(col.tag == "PeekTrigger")
        {
            charController.enabled = false;
            peekPrompt.enabled = true;
            StartCoroutine(PeekTime());
        } else if(col.tag == "JanitorTrigger")
        {

        }
    }
    IEnumerator PeekTime()
    {  
        yield return new WaitForSeconds(3f);
        Destroy(peekTrigger);
        charController.enabled = true;
        peekPrompt.enabled = false;
    }

    IEnumerator PromptTime()
    {
        yield return new WaitForSeconds(4f);
        doorPrompt.enabled = false;
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "DoorTrigger")
        {
            
        }
        else if (col.tag == "PeekTrigger")
        {

        }
        else if (col.tag == "JanitorTrigger")
        {

        }
    }
}
