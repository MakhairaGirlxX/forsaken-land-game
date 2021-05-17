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

	private NavMeshAgent agent;
	private bool isFollowing = false;
	private int i = 0;
	private float timer = 5;
	private bool timerRunning = false;

	void Start () {
		rb = GetComponent <Rigidbody>();
		agent = GetComponent<NavMeshAgent> ();
		NextPoint ();
	}

	void NextPoint()
	{
		if (!isFollowing) {
			Debug.Log ("is not following");
			if (points.Length == 0) {
				return;
			} 
			else if (i <= 2) {
				agent.destination = points [i].position;
				StartCoroutine (WaitOnPoint ());
			} else if (i > 2 && i < points.Length) {			
				agent.destination = points [i].position;
				StartCoroutine (WaitOnPoint ());
			}
		}

	}
	//janitor waits for seconds at point, only counts to 3 (may need to add a loop for mopping action)
	IEnumerator WaitOnPoint()
	{
		yield return new WaitForSeconds (5);
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		if (i <= 2) {
			i++;
			NextPoint ();
		}
		else if (i > 2) {
			i = Random.Range (2, 6);
			NextPoint ();
		}
	}
	// Update is called once per frame
	void Update () {

		switch(select)
		{
		case (EnemyMoves.Follow):
			Follow();
			break;
		}	

		RaycastHit hit;
		Vector3 ray = transform.TransformDirection (Vector3.forward);
		if (Physics.Raycast (transform.position, ray, out hit, 10)) {
			if (hit.collider.gameObject.tag == "Player") {
				//Debug.Log ("Hit Player");
				Debug.DrawRay (transform.position, pos.position, Color.green, 4, false);
				select = EnemyMoves.Follow;
			} else {
				//hit something that isn't player
				ResetPath();
			}
			//hit nothing
	}
}
	//you were working on calling patrol if player cannot be seen for 5 seconds by the enemy
	private void ResetPath()
	{
		timerRunning = true;
		if (timerRunning) {
			if (timer > 0) {
				//timer -= Time.deltaTime;
				Debug.Log (timer);
			} else {
				timer = 0;
				timerRunning = false;
				i = 0;
				NextPoint ();
			}
		}
	}

	private void Follow()
	{
		isFollowing = true;
		agent.destination = playerPos.position;
		agent.speed = 2f;
		Debug.Log ("isFollowing");

	}
}

public enum EnemyMoves
{
	Default,
	Follow
};
