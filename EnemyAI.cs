using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour {

	public GameObject janitor;
	public Transform pos;
	NavMeshAgent agent;
	
	// Use this for initialization

	void Start () {
		agent = GetComponent<NavMeshAgent> ();
	}

	
	// Update is called once per frame
	void Update () {
		RaycastHit hit;
		Vector3 ray = transform.TransformDirection (Vector3.forward);
		if (Physics.Raycast (transform.position, ray, out hit, 10)) {
			if (hit.collider.gameObject.tag == "Player") {
				//Debug.Log ("Hit Player");
				Debug.DrawRay (transform.position, pos.position, Color.green, 4, false);
				agent.destination = pos.position;
			} else {
				//Debug.Log ("Not Player");
			}

		}
		//Debug.Log ("Not Detected");	
	}
}
