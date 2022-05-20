using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
/*
 * This script is the second enemy script. Once the player collects the janitor keys, this script is enabled and NewEnemyAI.cs is disabled
 * I made a separate script so I could limit the functionality of the enemy without changing too much in the other script
 * The enemy will patrol inside the room, looking for their keys
 * The player can hide in the locker or under the desk
 * If they hide under the desk, the enemy will eventually find them
 * If they hide in the locker, they will go undiscovered and the enemy will eventually leave and disappear
 */
public class EnemyJanitorRoom : MonoBehaviour
{
    Animator enemyController;
    private NavMeshAgent agent;
    private bool isFollowing = false;

    public Transform[] points;
    public Transform playerPos;
    public GameObject deskTrig;
    public GameObject lockerTrig;
    public GameObject door;
    public GameObject player;
    public AwarenessType selectAwareness;
    public Text janitorPrompt;

    public GameObject enemy;
   
    Animator doorAnim;
    bool doorOpen = false;

    int i = 0;

    public bool isCaught = false;
    public bool restarted = false;

    //when the script is enabled, call Start()
    void OnEnable()
    {
        Start();
    }

    void Start()
    {
        // gameObject.SetActive(true);
        // gameObject.GetComponent<NewEnemyAI>().enabled = false;
        enemyController = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        janitorPrompt.enabled = false;
        doorAnim = door.GetComponent<Animator>();
        isCaught = false;
        restarted = false;
       // doorAnim.SetTrigger("DoorJanOpen");

        // i = 0;
        EnterRoom();
    }
    //Start the room patrol (similar to NewEnemyAI.cs)
    void EnterRoom()
    {
        if(!isFollowing || !isCaught)
        {
            if(points.Length == 0)
            {
                return;
            }
            else
            {
                //sets the enemy patrol point to position i
                agent.destination = points[i].position;
                enemyController.SetTrigger("isIdle");
                StartCoroutine(WaitOnPoint());
            }
        }
    }

    IEnumerator WaitOnPoint()
    {
        enemyController.SetTrigger("isWalking");
        yield return new WaitForSeconds(5);
        //if i doesn't exceed the array length
        if(i < points.Length)
        {
            i++;
            //have to call EnterRoom() each time or else it won't reset, if this is put in Update() it'll skip over numbers because the methods are called too quickly
            Debug.Log("i: " + i);
            //on point two, enable the text "Where are my keys?" in the UI
            if (i == 2)
            {
                janitorPrompt.enabled = true;
                EnterRoom();
            }
            //on point 3, set the enemy to the animation of "isLooking"
            else if (i == 3)
            {
                enemyController.SetTrigger("isLooking");
                EnterRoom();
            }
            //on point 4, if the player is under the desk, they are caught, if they aren't (should be in the locker), the enemy will keep patroling
            else if (i == 4)
            {
                janitorPrompt.enabled = false;
                if (deskTrig.GetComponent<DeskTrigger>().underDesk)
                {
                    selectAwareness = AwarenessType.Caught;
                }
                else
                {
                    EnterRoom();
                }           
               
            }
            //on point 5, the enemy will disappear so the player can freely roam the level
            else if (i == 5)
            {
                enemy.SetActive(false);
                EnterRoom();
            }
        }
        //double assurance that the enemy will disappear once the patrol is complete
        else
        {
            enemy.SetActive(false);
        }
        Debug.Log("Array Length:" + points.Length);
       
    }

    // Update is called once per frame
   void Update()
    {
        //shorter switch case because the enemy is now limited in their functionality
        switch (selectAwareness)
        {
            case (AwarenessType.Follow):
                enemyController.SetTrigger("isFollowing");
                Follow();
                break;
            case (AwarenessType.Caught):
                enemyController.SetTrigger("isCaught");
                Caught();
                break;
        }
      //if the player is not in the locker (hiding spot), the door to the janitor office is open, and the player is not under the desk (another hiding spot)
        if (lockerTrig.GetComponent<LockerTriggerUpdate>().inLocker == false && doorOpen && deskTrig.GetComponent<DeskTrigger>().underDesk == false)
        {
             selectAwareness = AwarenessType.Follow;           
        }
        //if the locker door is open, the enemy will attack
        if(player.GetComponent<UIPrompts>().lockerDoorOpen == true)
        {
            selectAwareness = AwarenessType.Follow;
        }
        //set in update or else there are problems with calling i as -1 and incrementing it
        if (i == 0)
        {
            doorAnim.SetTrigger("DoorJanOpen");
            doorOpen = true;
        }


        //raycast system to detect if the player is in range to be caught or not (set up the same as NewEnemyAI.cs)
        RaycastHit rayHit;
        Vector3 ray = transform.TransformDirection(Vector3.forward) * 10;

        int mask = 1 << LayerMask.NameToLayer("Player");

        bool hit = Physics.Raycast(transform.position, ray, out rayHit, 5f, mask);
        //Raycast is used here to determine when the enemy should return to patroling if the player is unseen
        if (hit)
        {
            //raycast hits player
            if (rayHit.collider.gameObject.layer == LayerMask.NameToLayer("Player") || (rayHit.collider.gameObject.tag == "Player"))
            {
                Debug.DrawRay(transform.position + new Vector3(0f, 1.5f, 0f), ray, Color.blue, 8);
                if (rayHit.distance < 1.5f)
                {
                    Debug.DrawRay(transform.position + new Vector3(0f, 1.5f, 0f), ray, Color.red, 8);
                    selectAwareness = AwarenessType.Caught;
                }
            }
        }
    }

   private void Follow()
    {
        agent.destination = playerPos.position;
        agent.speed = 2f;
    }

    private void Caught()
    {
        //reset the enemy position to before they entered the room
        transform.position = new Vector3(37, 0, -22);
        isCaught = true;
        isFollowing = false;

        StartCoroutine(Reset());

    }
    IEnumerator Reset()
    {
        yield return new WaitForSeconds(1f);
        //close door on reset
        doorAnim.SetTrigger("DoorJanClose");
        doorOpen = false;
        isCaught = false;
        restarted = true;
    }

    public enum AwarenessType
    {
        Default,
        Follow,
        Caught,
    };
}
 