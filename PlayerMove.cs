using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour 

	{

	[SerializeField] private string horizontalInputName;
	[SerializeField] private string verticalInputName;
	[SerializeField] private float movementSpeed;
	//[SerializeField] public float speed = 50f;

	[SerializeField] private AnimationCurve jumpFallOff;
	[SerializeField] private float jumpMultiplier;
	[SerializeField] private KeyCode jumpKey;

	[SerializeField] private KeyCode sprintKey;
	[SerializeField] private float sprintSpeed;

	[SerializeField] private float crouchSpeed;

	[SerializeField] public float maxAngle = 10.0f;
	[SerializeField] private KeyCode crouchInput;

	[SerializeField] private string magicKey;

	private bool isJumping;
	private bool isSprinting;
	private bool isMoving = false;
	private bool isDown = false;
	private bool isCrouching = false;

	private int keyPressed = 0;
	private int healthAmount = 100;

	private CharacterController charController;

	MagicType type;


	//lean
	//[SerializeField] public Transform Pivot;
	[SerializeField] public Transform leanPivot;
	//public Transform child;
	[SerializeField] private string leanLInputName;
	[SerializeField] private string leanRInputName;
	private bool isTriggerLeft = false;
	private bool isTriggerRight = false;



//	float curAngle = 0.0f;
//	float movAngle = 0.0f;

	float startTime = 0f;
	float holdTime = 3.0f;

	private Vector3 pos;

	public AudioSource runSound;
	public AudioSource walkSound;
	public AudioSource crouchSound;
	public AudioSource stillSound;
	public AudioSource lieSound;
	public AudioSource jumpSound;


		private void Awake()
		{
		stillSound.Play ();
		charController = GetComponent <CharacterController>();
		charController.height = 2.0f;

		//if (leanPivot == null && transform.parent != null) leanPivot = transform.parent;

		//pos = transform.position;

		}

	private void Update()
	{
			PlayerMovement();
			makeMove ();
			//LeanInput ();
			//PivotTo (pos);
		if (Input.GetKeyDown (crouchInput)) {
			startTime = Time.time;
			isDown = false;
		} 
		else if (Input.GetKeyUp (crouchInput)) {
			if (!isDown) {
				CrouchInput ();
				isDown = false;
				lieSound.Stop ();
			}
			isDown = false;
		}
		if (Input.GetKey (crouchInput)) {
			if (Time.time - startTime > holdTime) {
				Lie ();
				isDown = true;
			}
		}
	
	}
	/*
	public void PivotTo(Vector3 position)
	{
		Vector3 offset = transform.position - position;
		foreach (Transform child in transform)
			child.transform.position += offset;
		Debug.Log ("work");
		transform.position = position;
	}
	*/

		private void PlayerMovement()
		{

		if ((((Input.GetAxisRaw(horizontalInputName)) > 0.2) || ((Input.GetAxisRaw(horizontalInputName)) < -0.2)) || (((Input.GetAxisRaw(verticalInputName)) > 0.2) || ((Input.GetAxisRaw(verticalInputName)) < -0.2))) 
		{
			isMoving = true;
			//crouch but not move
			if (isCrouching == true) {
				stillSound.Stop ();
				walkSound.Stop ();
			}
			//not crouching but still
			stillSound.Play ();
			//Debug.Log ("Still sound");
		}

		else
		{	
			isMoving = false;
			//crouch and move
			if (isCrouching == true) {
				walkSound.Stop ();
				stillSound.Stop ();
			}
			//not crouching but moving
			walkSound.Play ();
			//Debug.Log ("walk sound");
		}

		if (isMoving) 
		{
			float horizInput = Input.GetAxisRaw (horizontalInputName) * movementSpeed;
			float vertInput = Input.GetAxisRaw (verticalInputName) * movementSpeed;

			Vector3 forwardMovement = transform.forward * vertInput;
			Vector3 rightMovement = transform.right * horizInput;

			charController.SimpleMove (forwardMovement + rightMovement);
		} 
		else 
		{
			charController.SimpleMove (new Vector3 (0, 0, 0));
		}

		JumpInput ();
		SprintInput ();	

	}


	//sprint
	private void SprintInput()
	{
		if (charController.isGrounded && Input.GetKeyDown ("joystick button 8")) 
		{
			isSprinting = true;
			movementSpeed = sprintSpeed;

			runSound.Play();
			Debug.Log ("run sound");
			walkSound.Stop ();
			stillSound.Stop ();
			Debug.Log ("it kinda worked!");
		} 

		else if (Input.GetKeyUp ("joystick button 8")) 
		{
			isSprinting = false;
			movementSpeed = 3.0f;

			runSound.Stop ();
			walkSound.Play ();
			Debug.Log ("it worked!");
		}
	}

	//jump
	private void JumpInput()
	{
		if (Input.GetKeyDown (jumpKey) && !isJumping)
		{
			isJumping = true;
	
			jumpSound.Play();
			walkSound.Stop ();
			stillSound.Stop ();
			runSound.Stop ();
			StartCoroutine (JumpEvent ());
		}
	}

	private IEnumerator JumpEvent()
	{
		charController.slopeLimit = 90.0f;
		float timeInAir = 0.0f;

		do
		{
			float jumpForce = jumpFallOff.Evaluate(timeInAir);
			charController.Move(Vector3.up * jumpForce * jumpMultiplier * Time.deltaTime);
			timeInAir += Time.deltaTime;

			yield return null;
		}
		while(!charController.isGrounded && charController.collisionFlags != CollisionFlags.Above);

		charController.slopeLimit = 45.0f;
		isJumping = false;
		//walkSound.Play ();
		Debug.Log ("walk sound jump");

	}

	private void Lie()
	{
		//this does change the height but the difference of height at 1 (crouch) and 0.01 (lie down) does not make a difference so this has to be changed eventually (the heights)
		charController.height = 0.01f;
		movementSpeed = 1.0f;

		lieSound.Play ();
		Debug.Log ("lie sound");
		crouchSound.Stop ();
		Debug.Log ("Held");

	}

	private void CrouchInput()
	{
		Ray ray = new Ray (transform.position, Vector3.up);
		RaycastHit Hit;

		keyPressed++;

		isCrouching = true;

		if (isCrouching == true) {
			movementSpeed = crouchSpeed;
			charController.height = 1.0f;

			crouchSound.Play ();
			Debug.Log ("crouch sound");
			stillSound.Stop ();
			walkSound.Stop ();
			//transform.position = new Vector3(transform.position.x, transform.position.y - 0.4f, transform.position.z);

		} 

		if (keyPressed > 1) {
			isCrouching = false;
			if (Physics.Raycast(ray, out Hit))
			{
				float distanceUp = Vector3.Distance(transform.position, Hit.point);
				keyPressed = 1;
				isCrouching = true;
			}
			else if (Hit.collider == null)
			{
				charController.height = 2.0f;
				movementSpeed = 3.0f;
				//crouchSound.Stop ();
				keyPressed = 0;
			}
		}
	}


	public void makeMove()
	{
		if (Input.GetAxisRaw (magicKey) != 0) {
			MagicFactory.getMagicType ();
			//Debug.Log ("Move");
		} else if (Input.GetAxisRaw (magicKey) == 0) {
			//Debug.Log ("Reset Move");
		}
	}

	/*
	private void LeanInput()
	{
		if (isTriggerRight == false) {
			if ((Input.GetAxisRaw (leanLInputName)) != 0) {
				curAngle = Mathf.MoveTowardsAngle (curAngle, maxAngle, speed * Time.deltaTime);


				if (isTriggerLeft == false) {
					isTriggerLeft = true;
				}
			}

			if ((Input.GetAxisRaw (leanLInputName)) == 0) {
				isTriggerLeft = false;
				curAngle = Mathf.MoveTowardsAngle (curAngle, 0f, speed * Time.deltaTime);
			}
			
		}


		if (isTriggerLeft == false) {
			if ((Input.GetAxisRaw (leanRInputName)) != 0) {
				curAngle = Mathf.MoveTowardsAngle (curAngle, -maxAngle, speed * Time.deltaTime);
				if (isTriggerRight == false) {
					isTriggerRight = true;
				}
			}

			if ((Input.GetAxisRaw (leanRInputName)) == 0) {
				isTriggerRight = false;
				curAngle = Mathf.MoveTowardsAngle (curAngle, 0f, speed * Time.deltaTime);
			}

		}
		leanPivot.transform.localRotation = Quaternion.AngleAxis (curAngle, Vector3.forward);
}
*/
		



}