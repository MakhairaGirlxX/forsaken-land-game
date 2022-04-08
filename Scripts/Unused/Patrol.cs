using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//agent.speed controls speed in navmesh
//stopping distance may have to be controlled or changed with animations
//for Navmesh make top piece separate from doorway so it can be baked through doorways
public class Patrol : MonoBehaviour {

	//change to Serialized private fields
	public Transform[] points;
	public Transform pos;
	public Transform playerPos;
	public Rigidbody rb;
	public EnemyMoves select;
	public GameObject player;

	private NavMeshAgent agent;
	private bool isFollowing = false;
	private bool isInvestigating = false;
	private int i = 0;
	private float timer = 10;
	private bool timerRunning = false;

	Animator enemyController;


	void Start () {
		enemyController = GetComponent<Animator>();
		rb = GetComponent <Rigidbody>();
		agent = GetComponent<NavMeshAgent> ();
		NextPoint ();
	}

	void NextPoint()
	{		
		if (!isFollowing && !isInvestigating) {			
			if (points.Length == 0) {
				return;
			} 
			else if (i <= 2) {
				agent.destination = points [i].position;
				enemyController.SetTrigger("isWalking");
				StartCoroutine (WaitOnPoint ());
			} else if (i > 2 && i < points.Length) {			
				agent.destination = points [i].position;
				enemyController.SetTrigger("isWalking");
				StartCoroutine (WaitOnPoint ());
			}
		}

	}

	IEnumerator WaitOnPoint()
	{
		enemyController.SetTrigger("isIdle");
		yield return new WaitForSeconds (5);
		//sets character movement to zero
		//rb.velocity = Vector3.zero;
		//rb.angularVelocity = Vector3.zero;

		if (i <= 2) {
			i++;
			NextPoint ();
		}
		else if (i > 2) {
			i = Random.Range (2, 6);
			NextPoint ();
		}
	}
		
	void Update () {

		//add more enemy functions here
		switch(select)
		{
		case (EnemyMoves.Follow):
				enemyController.SetTrigger("isFollowing");
				Follow();
			break;
		case (EnemyMoves.Investigate):
				enemyController.SetTrigger("isInvestigating");
				Investigate ();
			break;
		case(EnemyMoves.Caught):
				enemyController.SetTrigger("isCaught");
				Caught ();
			break;
		}

		//bit mask for layer 2
		//int layerMask = 1 << 3;
		//~ casts against everything but whats in layer 2
		//layerMask = ~layerMask;

		RaycastHit rayHit;
		Vector3 ray = transform.TransformDirection (Vector3.forward) * 10;

		int mask = 1 << LayerMask.NameToLayer("Player");
		mask |= 1 << LayerMask.NameToLayer("Wall");

		bool hit = Physics.Raycast(transform.position, ray, out rayHit, 5f, mask);
		//Debug.DrawRay(transform.position + new Vector3(0f, 1.5f, 0f), ray, Color.green, 8);

		if (hit) {		
			//raycast hits player
			if(rayHit.collider.gameObject.layer == LayerMask.NameToLayer("Player") ||  (rayHit.collider.gameObject.tag == "Player"))
            {
				Debug.Log("Ray hit distance: " + rayHit.distance);
				//determines how long the timer is depending on the distance but should set a coroutine to wait before following
				if (rayHit.distance > 5f)
				{
					Debug.DrawRay(transform.position + new Vector3(0f, 1.5f, 0f), ray, Color.green, 8);
					StartCoroutine(WaitToFollow(10));					
				}
				else if(rayHit.distance > 3f)
                {
					Debug.DrawRay(transform.position + new Vector3(0f, 1.5f, 0f), ray, Color.green, 8);
					StartCoroutine(WaitToFollow(5));
				}
				else if(rayHit.distance > 1.5f)
                {
					Debug.DrawRay(transform.position + new Vector3(0f, 1.5f, 0f), ray, Color.green, 8);
					StartCoroutine(WaitToFollow(2));
				}
				else if (rayHit.distance < 1.5f)
				{
					Debug.DrawRay(transform.position + new Vector3(0f, 1.5f, 0f), ray, Color.red, 8);
					select = EnemyMoves.Caught;
				}
				//select = EnemyMoves.Follow;				
			}
			//raycast hits wall or anything else
			//put more stuff in the red layer
            else
            {
				Debug.DrawRay(transform.position + new Vector3(0f, 1.5f, 0f), ray, Color.red, 8, true);
				//hit nothing or something that is not player
				//if player isn't seen for 10 seconds then return to default position
				//this was a separate if statement
				if (isFollowing == true)
				{
					if (timer > 0)
					{
						timer -= Time.deltaTime;
					}
					else
					{
						timer = 0;
						timerRunning = false;
						isFollowing = false;

						select = EnemyMoves.Default;
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
					}
					else
					{
						timer = 0;
						timerRunning = false;
						isInvestigating = false;

						select = EnemyMoves.Default;
						i = 2;
						agent.speed = 3.5f;
						agent.stoppingDistance = 0;
						Start();
					}
				}
			}
			/**/
		} 		
	}

	IEnumerator WaitToFollow(int timer)
    {
		yield return new WaitForSeconds(timer);
		select = EnemyMoves.Follow;
    }

	private void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.tag == "Player" && (player.GetComponent<PlayerMove>().isWalkingCheck)) {
			select = EnemyMoves.Investigate;
		}
	}

	private void Follow()
	{
		isFollowing = true;
		isInvestigating = false;

		Debug.Log("Follow");

		agent.destination = playerPos.position;
		agent.speed = 2f;
	}

	private void Investigate()
	{
		isInvestigating = true;
		isFollowing = false;

		Debug.Log("Investigate");

		agent.destination = playerPos.position;
		agent.speed = 1f;
		agent.stoppingDistance = 2f;
	}

	private void Caught()
	{
		Debug.Log ("Caught!");
		//Game Over
	}
}

public enum EnemyMoves
{
	Default,
	Follow,
	Investigate,
	Caught,
};
