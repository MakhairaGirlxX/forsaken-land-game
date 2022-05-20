using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPrompts : MonoBehaviour
{
    public Text doorPrompt, peekPrompt, janitorPrompt, lockerPrompt, elevatorPrompt, keyPrompt, needKeyPrompt;
    public GameObject door_1, door_2, door_3, peekTrigger, janitorTrigger, lockerDoor, elevatorRightDoor, elevatorLeftDoor, playerObj, elevator;
    bool doorRange = false;
    bool doorRange_2 = false;
    bool doorRange_3 = false;
    bool lockerRange = false;
    bool elevatorRange = false;
    public bool gotKeys = false;
    public bool doorOpen = false;
    public bool janitorDoorOpen = false;
    public bool lockerDoorOpen = false;
    [SerializeField] private KeyCode useKey;
    Animator doorAnim, doorAnim_2, doorAnim_3, lockerDoorAnim, rightDoorAnim, leftDoorAnim, elevatorAnim;
    int useTime = 0;
    int useTime_2 = 0;
    int useTime_3 = 0;
    int lockerUseTime = 0;
    private CharacterController charController;

    public bool nextScene = false;

    // Start is called before the first frame update
    void Start()
    {
        //disable all UI text on Start
        doorPrompt.enabled = false;
        peekPrompt.enabled = false;
        janitorPrompt.enabled = false;
        lockerPrompt.enabled = false;
        elevatorPrompt.enabled = false;
        keyPrompt.enabled = false;
        needKeyPrompt.enabled = false;

        //get all the animators for UI objects like doors, locker, and elevator
        doorAnim = door_1.GetComponent<Animator>();
        doorAnim_2 = door_2.GetComponent<Animator>();
        doorAnim_3 = door_3.GetComponent<Animator>();
        lockerDoorAnim = lockerDoor.GetComponent<Animator>();
        rightDoorAnim = elevatorRightDoor.GetComponent<Animator>();
        leftDoorAnim = elevatorLeftDoor.GetComponent<Animator>();
        elevatorAnim = elevator.GetComponent<Animator>();

        charController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        //subscribe to onDeath to reset
       // PlayerMove.onDeath += ResetDoors;

        //if the player is in range of the first door and the open key is pressed
        if(doorRange == true && Input.GetKeyDown(useKey) && useTime == 0)
        {
            doorAnim.SetTrigger("DoorOpen");
            useTime = 1;
        }

        //if the player is in range of the second door and the open key is pressed
        if (doorRange_2 == true && Input.GetKeyDown(useKey))
        {
            DoorActions();
        }

        //if the player is in range of the third door and the open key is pressed
        if (doorRange_3 == true && Input.GetKeyDown(useKey))
        {
            DoorJanActions();
        }

        //if the player is in range of the locker door and the open key is pressed
        if (lockerRange == true && Input.GetKeyDown(useKey))
        {
            LockerActions();
        }

        //if the player is in range of the elevator button and the open key is pressed
        if (elevatorRange == true && Input.GetKeyDown(useKey))
        {
            //open elevator doors
            rightDoorAnim.SetTrigger("RightDoorTrigger");
            leftDoorAnim.SetTrigger("LeftDoorTrigger");
        }
        //if the player obtained the keys and interacted with the elevator buttons inside the elevator
        if(gotKeys == true && Input.GetKeyDown(useKey))
        {
            //closes the elevator doors
            rightDoorAnim.SetTrigger("RightDoorTriggerClose");
            leftDoorAnim.SetTrigger("LeftDoorTriggerClose");
            //coroutine to move the elevator
            StartCoroutine(ElevatorMove());
        }

    }
    //reset door positions on players death
    void ResetDoors()
    {
        lockerDoorAnim.SetTrigger("LockerDoorClose");
        lockerDoorOpen = false;

        doorAnim_2.SetTrigger("DoorJanClose");
        doorOpen = false;

        doorAnim_3.SetTrigger("DoorJanClose");
        janitorDoorOpen = false;
    }
    //opens and closes locker doors depending on how many times the useKey has been pressed
    public void LockerActions()
    {
        lockerUseTime++;

        if (lockerUseTime <= 1)
        {
            lockerDoorAnim.SetTrigger("LockerDoorOpen");
            lockerDoorOpen = true;
            Debug.Log("Locker open");
        }

        if (lockerUseTime > 1)
        {
            lockerDoorAnim.SetTrigger("LockerDoorClose");
            lockerDoorOpen = false;
             Debug.Log("Locker Close");
           lockerUseTime = 0;
        }
    }
    //same as above
    public void DoorJanActions()
    {
        useTime_3++;

        if (useTime_3 <= 1)
        {
            doorAnim_3.SetTrigger("DoorJanOpen");
            janitorDoorOpen = true;
        }

        if (useTime_3 > 1)
        {
            doorAnim_3.SetTrigger("DoorJanClose");
            janitorDoorOpen = false;
            useTime_3 = 0;
        }
    }
    //same as above
    public void DoorActions()
    {        
        useTime_2++;

        if(useTime_2 <= 1)
        {
            doorAnim_2.SetTrigger("DoorJanOpen");
            doorOpen = true;
        }

        if(useTime_2 > 1)
        {
            doorAnim_2.SetTrigger("DoorJanClose");
            doorOpen = false;
            useTime_2 = 0;
        }
    }
    //detects if the player collides with any of the colliders on UI objects
    void OnTriggerEnter(Collider col)
    {
        //collides with first door
        if(col.tag == "DoorTrigger" && useTime == 0)
        {
            doorPrompt.enabled = true;
            doorRange = true;
            StartCoroutine(PromptTime());
        } 
        //collides with second door
        else if(col.tag == "DoorTrigger_2")
        {
            doorRange_2 = true;
        }
        //collides with third door
        else if (col.tag == "DoorTrigger_3")
        {
            doorRange_3 = true;
        }
        //collides with 'peek prompt'
        else if(col.tag == "PeekTrigger")
        {
            charController.enabled = false;
            peekPrompt.enabled = true;
            StartCoroutine(PeekTime());
        } 
        //collides with 'find room' trigger 
        else if(col.tag == "FindRoomTrigger")
        {
            janitorPrompt.enabled = true;
            StartCoroutine(JanitorPrompt());           
        } 
        //collides with locker door
        else if(col.tag == "LockerDoor")
        {
            lockerRange = true;
            lockerPrompt.enabled = true;
            StartCoroutine(LockerTime());
        }
        //collides with elevator button
        else if(col.tag == "ElevatorButtonTrigger")
        {
            elevatorRange = true;
            elevatorPrompt.enabled = true;  
            StartCoroutine(ElevatorTime());
        } 
        //collides with the elevator buttons inside the elevator
        else if(col.tag == "BasementTrigger")
        {
            //if the player has the keys
            if(playerObj.GetComponent<ItemList>().stop == true)
            {
                keyPrompt.enabled = true;
                gotKeys = true;            
            }
            //if they don't have the keys prompts the user to find the keys
            else
            {
                needKeyPrompt.enabled = true;
                StartCoroutine(ElevatorPrompts());
            }
        }
    }
    //moves the elevator
    IEnumerator ElevatorMove()
    {
        yield return new WaitForSeconds(2f);
        keyPrompt.enabled = false;
        elevatorAnim.SetTrigger("ElevatorTrigger");
        //used to determine if the next scene should be loaded
        nextScene = true;
    }
    /*
     * All of these IEnumerators are to determine how long a prompt/text should stay active on the screen
     */
    IEnumerator ElevatorPrompts()
    {
        yield return new WaitForSeconds(2f);
        needKeyPrompt.enabled = false;
       
    }
    IEnumerator ElevatorTime()
    {
        yield return new WaitForSeconds(3f);
        elevatorPrompt.enabled = false;
    }

    IEnumerator LockerTime()
    {
        yield return new WaitForSeconds(2f);
        lockerPrompt.enabled = false;
    }

    IEnumerator JanitorPrompt()
    {
        yield return new WaitForSeconds(3f);
        Destroy(janitorTrigger);
        janitorPrompt.enabled = false;
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
    //out of range of UI objects (can't open doors or interact if not in range)
    void OnTriggerExit(Collider col)
    {
        if (col.tag == "LockerDoor")
        {
            lockerPrompt.enabled = false;
            lockerRange = false;
        }
        else if (col.tag == "DoorTrigger")
        {           
            doorRange = false;
        }
        else if (col.tag == "DoorTrigger_2")
        {
            doorRange_2 = false;
        }
        else if (col.tag == "DoorTrigger_3")
        {
            doorRange_3 = false;
        }
    }
}
