using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NewEnemyAI : MonoBehaviour
{
    //final fixes:
    /*
     * bake a proper NavMesh
     * Fix doorways so they are in pieces
     * Caught calls the game over screen
     */
    public Transform[] points;
    public Transform playerPos;
    //don't know if i should keep rb
    public Rigidbody rb;
    public AwarenessType selectAwareness;
    public GameObject player;
    public GameObject wideCollider;
    public GameObject hidingPlace;
    public GameObject otherSide;

    private NavMeshAgent agent;
    private int i = 0;

    Animator enemyController;

    bool hitPlayer = false;
    bool inRange = false;
    bool isFollowing = false;
    bool isInvestigating = false;

    float counter = 0;
    float timer = 10;

    bool countSlow = false;
    bool countMed = false;
    bool countFast = false;

    void Start()
    {
        enemyController = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        NextPoint();
    }

    //Start Patrol
    void NextPoint()
    {
        if (!isFollowing && !isInvestigating)
        {
            Debug.Log("Patrol");
            if (points.Length == 0)
            {
                return;
            }
            else if (i <= 2)
            {
                agent.destination = points[i].position;
                enemyController.SetTrigger("isWalking");
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
        enemyController.SetTrigger("isIdle");
        yield return new WaitForSeconds(10);

        if (i <= 2)
        {
            i++;
            NextPoint();
        }
        else if (i > 2)
        {
            i = Random.Range(2, 6);
            NextPoint();
        }
    }

    void Update()
    {
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

        //raycast doesn't work right now
        //so the player is always not seen and only follows until the timer hits zero then goes back to default and immediately investigates if the conditions are met
        //if the player is seen it should follow through until they are caught
        RaycastHit rayHit;
        Vector3 ray = transform.TransformDirection(Vector3.forward) * 10;

        int mask = 1 << LayerMask.NameToLayer("Player");
        mask |= 1 << LayerMask.NameToLayer("Wall");

        bool hit = Physics.Raycast(transform.position, ray, out rayHit, 5f, mask);
        //Raycast is used here to determine when the enemy should return to patroling if the player is unseen
        if (hit)
        {           
            //raycast hits player
            if (rayHit.collider.gameObject.layer == LayerMask.NameToLayer("Player") || (rayHit.collider.gameObject.tag == "Player"))
            {
                Debug.DrawRay(transform.position + new Vector3(0f, 1.5f, 0f), ray, Color.blue, 8);
                Debug.Log("Seen by raycast");
                hitPlayer = true;
                if (rayHit.distance < 1.5f)
                {
                    Debug.Log("Close");
                    Debug.DrawRay(transform.position + new Vector3(0f, 1.5f, 0f), ray, Color.red, 8);
                    selectAwareness = AwarenessType.Caught;
                }
            }           
            //raycast hits wall or anything else
            else
            {
                Debug.DrawRay(transform.position + new Vector3(0f, 1.5f, 0f), ray, Color.red, 8);
                hitPlayer = false;
                //if the raycast hits the door, the door is not open, the player is on the other side and the enemy is following or investigating reset
                if(!player.GetComponent<UIPrompts>().doorOpen && otherSide.GetComponent<OtherSide>().otherSide && (isFollowing || isInvestigating))
                {
                    Debug.Log("triggered");
                    isFollowing = false;
                    isInvestigating = false;

                    selectAwareness = AwarenessType.Default;
                    i = 2;
                    agent.speed = 3.5f;
                    agent.stoppingDistance = 0;
                    Start();
                }

                if (isFollowing == true)
                {
                    if (timer > 0)
                    {
                        timer -= Time.deltaTime;
                        Debug.Log(timer);
                    }
                    else
                    {
                        timer = 0;
                        isFollowing = false;

                        selectAwareness = AwarenessType.Default;
                        i = 2;
                        agent.speed = 3.5f;
                        agent.stoppingDistance = 0;
                        Start();
                    }
                }

                if (isInvestigating == true)
                {
                    if (timer > 0)
                    {
                        timer -= Time.deltaTime;
                        Debug.Log(timer);
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
        //hits nothing
        else
        {
            Debug.DrawRay(transform.position + new Vector3(0f, 1.5f, 0f), ray, Color.green, 8);
            hitPlayer = false;           
        }        

        AwarenessMeter();
        
    }
    
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player" && !hidingPlace.GetComponent<Hidden>().hidden)
        {
            Debug.Log("seen by collider");
            inRange = true;
           selectAwareness = AwarenessType.Follow;
        }
    }

    void OnTriggerExit(Collider col)
    {
        inRange = false;
    }
    

    private void AwarenessMeter()
    {
        //player is in the room, not crouching and not hidden behind something and they are not being followed
        if (!isFollowing && wideCollider.GetComponent<EnterRoomCollider>().inWideRange == true && !player.GetComponent<PlayerMove>().isCrouching && !hidingPlace.GetComponent<Hidden>().hidden)
        {
            Debug.Log("Awareness Meter Investigate Triggered");
            selectAwareness = AwarenessType.Investigate;
        } 
        //player is sprinting (follow because their footsteps are too loud)
        else if (wideCollider.GetComponent<EnterRoomCollider>().inWideRange && player.GetComponent<PlayerMove>().isSprinting)
        {
            selectAwareness = AwarenessType.Follow;
        }
    }

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

    private void Follow()
    {
        isFollowing = true;
        isInvestigating = false;

        Debug.Log("Follow");

        agent.destination = playerPos.position;
        agent.speed = 2f;
        
        //timer counts down if unseen and goes back to patrolling
       
        

    }

    //heads to players position but stops at a distance
    private void Investigate()
    {
        isInvestigating = true;
        isFollowing = false;

        Debug.Log("Investigate");

        agent.destination = playerPos.position;
        agent.speed = 1f;
        agent.stoppingDistance = 2f;

        //timer counts down if unseen and goes back to patrolling       

    }

    private void Caught()
    {
        Debug.Log("Caught!");
        //Game Over
    }

    public enum AwarenessType
    {
        Default,
        Follow,
        Investigate,
        Caught,
    };

}
