using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
 * This script is designed as the first part of the Enemy AI system, it consists of a patrolling fucntion
 * The enemy can follow the player if certain conditions are met
 * The enemy can also 'investigate' the player if certain conditions are met (will walk towards the player location)
 * If the player gets in close range they will be 'caught' and will respawn
 * Once the player gets the keys in the Janitor, this script will be disabled to allow for EnemyJanitorRoom.cs to be enabled for the next set of enemy actions and conditions
 * */

public class NewEnemyAI : MonoBehaviour
{
    //array to store the location that the enemy needs to go to (patrol pattern)
    public Transform[] points;
    public Transform playerPos;

    public Rigidbody rb;
    public AwarenessType selectAwareness;
    public GameObject player;
    public GameObject wideCollider;
    public GameObject hidingPlace;
    public GameObject otherSide;
    public GameObject janitorRoomTrig;

    private NavMeshAgent agent;
    private int i = 0;

    Animator enemyController;

    bool hitPlayer = false;
    bool inRange = false;
    bool isFollowing = false;
    bool isInvestigating = false;

    float timer = 10;

    public bool isCaught = false;

    void Start()
    {
        //get the animator to call animations (takes off of gameObject assigned to enemyController)
        enemyController = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        //initialize the NavMesh so the enemy won't run into walls or hiding spots
        agent = GetComponent<NavMeshAgent>();
        //set enemy to true everytime the game is reset which happens when a player dies
        gameObject.SetActive(true);
        //calling patrol
        NextPoint();
    }

    //Start Patrol
    void NextPoint()
    {
        //the enemy can't be following or looking for the player in order to patrol in their default pattern
        if (!isFollowing && !isInvestigating)
        {
            //check if there are any points
            if (points.Length == 0)
            {
                return;
            }
            else if (i <= 2)
            {
                //set the enemy position to a specific location (points are assigned in the points[] array in the inspector window in Unity)
                agent.destination = points[i].position;
                //sets the animation trigger in the Animation window (see animation tree by double-clicking on 'EnemyTemp' in the hiearchy and opening the Animation window)
                enemyController.SetTrigger("isWalking");
                //starts a coroutine so the enemy idles on a point for a given amoutn of time
                StartCoroutine(WaitOnPoint());
            }
            else if (i > 2 && i < points.Length)
            {
                agent.destination = points[i].position;
                enemyController.SetTrigger("isWalking");
                StartCoroutine(WaitOnPoint());
            }
        }
    }

    //Idles on a point
    IEnumerator WaitOnPoint()
    {
        //sets enemy animation to Idle
        enemyController.SetTrigger("isIdle");
        //wait for 10 seconds on a point before going to the next one
        yield return new WaitForSeconds(10);

        //intentionally set the first two points to be the same to allow for unique animations in the future as well as making sure the enemy is on the same 2 points during every reset
        if (i <= 2)
        {
            i++;
            //calls NextPoint() again to increment i and change the position
            NextPoint();
        }
        else if (i > 2)
        {
            //after the first two points the enemy can patrol randomly between the six assigned points
            i = Random.Range(2, 6);
            NextPoint();
        }
    }
    //Updates every frame
    void Update()
    {
        //changes the action the enemy should take (in update so the enemy can break away from any other actions immediately)
        switch (selectAwareness)
        {
            case (AwarenessType.Follow):
                enemyController.SetTrigger("isFollowing");
                //ref allows the timer to be continously updated
                 Follow();
                break;
            case (AwarenessType.Investigate):
                enemyController.SetTrigger("isInvestigating");
                Investigate();
                break;
            case (AwarenessType.Caught):
                enemyController.SetTrigger("isCaught");
                Caught();
                break;
        }
        //if the player gets the keys in the janitor's office, this function will be called to stop all the coroutines to allow the script to be fully disabled
        //disabling of this script can be found in the ItemList script
        if (player.GetComponent<ItemList>().stop == true)
        {
            Debug.Log("Stopped");
            StopAllCoroutines();
        }

        //if the door to the janitor's office is closed and the player is inside the room, the enemy returns to patrolling
        if (!player.GetComponent<UIPrompts>().janitorDoorOpen && janitorRoomTrig.GetComponent<JanitorRoomTrigger>().inJanitorRoom)
        {
            selectAwareness = AwarenessType.Default;
            i = 2;
            agent.speed = 3.5f;
            agent.stoppingDistance = 0;
            //had to make the enemy dissappear to be reset as a fail safe during my demonstration so I can get past the enemy easier 
            //this is set to true again when Start() is called
            gameObject.SetActive(false);
            Start();
        } 

       //Raycast system is setup to detect what the enemy is looking at. The ray comes out of the enemies head to a specified distance
        RaycastHit rayHit;
        Vector3 ray = transform.TransformDirection(Vector3.forward) * 10;
        
        /*the layer mask tells the enemy what objects the ray is hitting
         * in this example, the enemy is told to recognize the player and wall layers
         * the binary or was used to combine masks properly 
         * */
        int mask = 1 << LayerMask.NameToLayer("Player");
        mask |= 1 << LayerMask.NameToLayer("Wall");

        //sets up where the ray hits and uses the layermask specified above to tell the ray what it's hitting
        bool hit = Physics.Raycast(transform.position, ray, out rayHit, 5f, mask);

        //Raycast is used here to determine when the enemy should return to patroling if the player is unseen
        if (hit)
        {           
            //raycast hits player (two check system for layer and collider for double assurance)
            if (rayHit.collider.gameObject.layer == LayerMask.NameToLayer("Player") || (rayHit.collider.gameObject.tag == "Player"))
            {
                //draws a blue ray if the player is hit by the ray (for testing purposes only)
                Debug.DrawRay(transform.position + new Vector3(0f, 4.0f, 0f), ray, Color.blue, 8);
                hitPlayer = true;
                //the ray hits the player and the player comes within a shorter distance, the player will then be Caught
                if (rayHit.distance < 1.5f)
                {
                    //color red for testing purposes
                    Debug.DrawRay(transform.position + new Vector3(0f, 4.0f, 0f), ray, Color.red, 8);
                    selectAwareness = AwarenessType.Caught;
                }
            }           
            //raycast hits wall or anything else
            else
            {
                Debug.DrawRay(transform.position + new Vector3(0f, 1.5f, 0f), ray, Color.red, 8);
                hitPlayer = false;

                //if the enemy is following the player, start a timer
                if (isFollowing == true)
                {
                    if (timer > 0)
                    {
                        timer -= Time.deltaTime;
                       // Debug.Log(timer);
                    }
                    //if the player remains unseen for timer=10 seconds, set the enemy back to patrolling
                    else
                    {
                        timer = 0;
                        isFollowing = false;

                        selectAwareness = AwarenessType.Default;
                        i = 2;
                        //set speed and stopping distance for enemy
                        agent.speed = 3.5f;
                        agent.stoppingDistance = 0;
                        Start();
                    }
                }
                //if the enemy is following the player, start a timer (same concept as above)
                if (isInvestigating == true)
                {
                    if (timer > 0)
                    {
                        timer -= Time.deltaTime;
                    }
                    else
                    {
                        timer = 0;
                        isInvestigating = false;

                        selectAwareness = AwarenessType.Default;
                        i = 2;
                        agent.speed = 3.5f;
                        agent.stoppingDistance = 0;
                        Start();
                    }
                }
            }
        }
        //raycast hits nothing
        else
        {
            Debug.DrawRay(transform.position + new Vector3(0f, 1.5f, 0f), ray, Color.green, 8);
            hitPlayer = false;           
        }        
        //call awareness meter for special detection circumstances
        AwarenessMeter();
        
    }
    
    void OnTriggerEnter(Collider col)
    {
        //if the player is detected by the collider attached to the enemy, not hidden (see Hidden.cs), in the room (see EnterRoomCollider.cs) and not in the Janitor Office (see JanitorRoomTrigger.js)
        if (col.gameObject.tag == "Player" && !hidingPlace.GetComponent<Hidden>().hidden && wideCollider.GetComponent<EnterRoomCollider>().inWideRange == true && janitorRoomTrig.GetComponent<JanitorRoomTrigger>().inJanitorRoom == false)
        {
            inRange = true;
           selectAwareness = AwarenessType.Follow;
        }
    }
    //inRange is used to detect if the player is out of range of the collider
    void OnTriggerExit(Collider col)
    {
        inRange = false;
    }
    

    private void AwarenessMeter()
    {
        //player is in the room, not crouching and not hidden behind something and they are not being followed, and are not in the janitor room
        if (!isFollowing && wideCollider.GetComponent<EnterRoomCollider>().inWideRange == true && !player.GetComponent<PlayerMove>().isCrouching && !hidingPlace.GetComponent<Hidden>().hidden && janitorRoomTrig.GetComponent<JanitorRoomTrigger>().inJanitorRoom == false)
        {
           // Investigate the player (see investigate method)
            selectAwareness = AwarenessType.Investigate;
        } 
        //player is sprinting (follow because their footsteps are too loud)
        //Wide collider checks to see if the player is in the room (in place to make sure that if the player is detected by the enemy collider they won't be followed if they are out of the room)
        else if (wideCollider.GetComponent<EnterRoomCollider>().inWideRange && player.GetComponent<PlayerMove>().isSprinting && janitorRoomTrig.GetComponent<JanitorRoomTrigger>().inJanitorRoom == false)
        {
           // Follow
            selectAwareness = AwarenessType.Follow;
        }
    }
    //this timer was set up to reduce the amount of times the timer is called, however it requires references in the switch case to update which is too taxing on the system
    /*
    private void StartTimer(ref float timer)
    {
        if (!hitPlayer)
        {
            if (timer > 0)
            {
                Debug.Log(timer);
                timer -= Time.deltaTime;
            }
            else
            {
                timer = 0;
                isFollowing = false;
                isInvestigating = false;

                selectAwareness = AwarenessType.Default;
                i = 2;
                agent.speed = 3.5f;
                agent.stoppingDistance = 0;
                Start();
            }
        }
    }
    */

    //Follow method called when the switch case is set to Follow
    private void Follow()
    {
        isFollowing = true;
        isInvestigating = false;

       //sets the enemy destination to the players position
        agent.destination = playerPos.position;
        //increases the speed
        agent.speed = 2f; 
    }

    //heads to players position but stops at a distance
    private void Investigate()
    {
        isInvestigating = true;
        isFollowing = false;

        agent.destination = playerPos.position;
        agent.speed = 1f;
        agent.stoppingDistance = 2f;
    }

    private void Caught()
    {
        isCaught = true;
        isFollowing = false;
        isInvestigating = false;
        selectAwareness = AwarenessType.Default;
        i = 2;
        agent.speed = 3.5f;
        agent.stoppingDistance = 0;
        transform.position = new Vector3(37, 0, -22);

        //if the player is cuaght, the coroutine Reset is called
        StartCoroutine(Reset());
        
    }
    //resets the enemy to the patrol point after 1 second (can't reset without waiting otherwise Update() will be called and the enemy will continue to follow the player)
    IEnumerator Reset()
    {
        yield return new WaitForSeconds(1f);

        Debug.Log("called reset");
        isCaught = false;
        isInvestigating = false;
        isFollowing = false;
        selectAwareness= AwarenessType.Default;
        Start();
    }

    //interface for awareness switch case
    public enum AwarenessType
    {
        Default,
        Follow,
        Investigate,
        Caught,
    };

}
