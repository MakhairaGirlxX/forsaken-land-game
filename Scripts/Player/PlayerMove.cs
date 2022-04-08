using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour 

	{

	[SerializeField] private string horizontalInputName;
	[SerializeField] private string verticalInputName;
	[SerializeField] private float movementSpeed;

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
	public bool isSprinting;
	public bool isMoving = false;
	private bool isDown = false;
	public bool isCrouching = false;

	public bool isWalkingCheck = false;

	private int keyPressed = 0;
	private int healthAmount = 100;

	private CharacterController charController;

	MagicType type;

	public GameObject leanPivot;
	[SerializeField] private string leanLeftInput, leanRightInput;
	private bool isLeft = false;
	private bool isRight = false;
	public float maxLeanAngle;
	public float speed;

	float startTime = 0f;
	float holdTime = 1.0f;

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
		charController.height = 3.0f;
		}

	private void Update()
	{
		PlayerMovement();
		Lean();

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

		private void PlayerMovement()
		{

		if ((((Input.GetAxisRaw(horizontalInputName)) > 0.2) || ((Input.GetAxisRaw(horizontalInputName)) < -0.2)) || (((Input.GetAxisRaw(verticalInputName)) > 0.2) || ((Input.GetAxisRaw(verticalInputName)) < -0.2))) 
		{
			isMoving = true;
			isWalkingCheck = true;
			//crouch but not move
			if (isCrouching == true) {
				stillSound.Stop ();
				walkSound.Stop ();
				isWalkingCheck = false;
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
				isWalkingCheck = false;
			}
			//not crouching but moving
			walkSound.Play ();
			isWalkingCheck = false;
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
			//Debug.Log ("run sound");
			walkSound.Stop ();
			stillSound.Stop ();
			//Debug.Log ("it kinda worked!");
		} 

		else if (Input.GetKeyUp ("joystick button 8")) 
		{
			isSprinting = false;
			movementSpeed = 3.0f;

			runSound.Stop ();
			walkSound.Play ();
			//Debug.Log ("it worked!");
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
		//Debug.Log ("walk sound jump");

	}

	private void Lie()
	{
		charController.height = 1.0f;
		movementSpeed = 1.0f;

		lieSound.Play ();
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
			Debug.Log("Crouching");
			movementSpeed = crouchSpeed;
			charController.height = 2.0f;

			crouchSound.Play ();
			stillSound.Stop ();
			walkSound.Stop ();

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
				charController.height = 3.0f;
				movementSpeed = 3.0f;
				//crouchSound.Stop ();
				keyPressed = 0;
			}
		}
	}
	public void Lean()
    {

		if (isRight == false)
		{
			if ((Input.GetAxisRaw(leanLeftInput)) != 0)
			{
				float currentAngle = Mathf.MoveTowardsAngle(leanPivot.transform.eulerAngles.x, maxLeanAngle, speed * Time.deltaTime);
				leanPivot.transform.eulerAngles = new Vector3(currentAngle, leanPivot.transform.eulerAngles.y, leanPivot.transform.eulerAngles.z);
				isLeft = true;

			}
			if ((Input.GetAxisRaw(leanLeftInput)) == 0)
			{
				float firstAngle = Mathf.MoveTowardsAngle(leanPivot.transform.eulerAngles.x, 0.0f, speed * Time.deltaTime);
				leanPivot.transform.eulerAngles = new Vector3(firstAngle, leanPivot.transform.eulerAngles.y, leanPivot.transform.eulerAngles.z);
				isLeft = false;
			}
		}


		if (isLeft == false)
		{
			if ((Input.GetAxisRaw(leanRightInput)) != 0)
			{
				float rightAngle = Mathf.MoveTowardsAngle(leanPivot.transform.eulerAngles.x, -maxLeanAngle, speed * Time.deltaTime);
				leanPivot.transform.eulerAngles = new Vector3(rightAngle, leanPivot.transform.eulerAngles.y, leanPivot.transform.eulerAngles.z);
				isRight = true;
			}

			if ((Input.GetAxisRaw(leanRightInput)) == 0)
			{
				float firstAngle = Mathf.MoveTowardsAngle(leanPivot.transform.eulerAngles.x, 0.0f, speed * Time.deltaTime);
				leanPivot.transform.eulerAngles = new Vector3(firstAngle, leanPivot.transform.eulerAngles.y, leanPivot.transform.eulerAngles.z);
				isRight = false;
			}
		}
	}
	/*
	public void Lean()
    {
		if (isTriggerRight == false)
		{
			if ((Input.GetAxisRaw(leanLInputName)) != 0)
			{
				curAngle = Mathf.MoveTowardsAngle(curAngle, maxAngle, speed * Time.deltaTime, Space.self);


				if (isTriggerLeft == false)
				{
					isTriggerLeft = true;
				}
			}

			if ((Input.GetAxisRaw(leanLInputName)) == 0)
			{
				isTriggerLeft = false;
				curAngle = Mathf.MoveTowardsAngle(curAngle, 0f, speed * Time.deltaTime, Space.self);
			}

		}


		if (isTriggerLeft == false)
		{
			if ((Input.GetAxisRaw(leanRInputName)) != 0)
			{
				curAngle = Mathf.MoveTowardsAngle(curAngle, -maxAngle, speed * Time.deltaTime);
				if (isTriggerRight == false)
				{
					isTriggerRight = true;
				}
			}

			if ((Input.GetAxisRaw(leanRInputName)) == 0)
			{
				isTriggerRight = false;
				curAngle = Mathf.MoveTowardsAngle(curAngle, 0f, speed * Time.deltaTime);
			}

		}
		leanPivot.transform.localRotation = Quaternion.AngleAxis(curAngle, Vector3.forward);
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
	public void AccessInventory()
	{
		if (Input.GetKeyDown (inventoryKey) && inventoryImg.gameObject.activeSelf == true) {
			inventoryImg.gameObject.SetActive (false);
			Time.timeScale = 1;
		}

		else if (Input.GetKeyDown (inventoryKey) && inventoryImg.gameObject.activeSelf == false) {
			inventoryImg.gameObject.SetActive (true);
			Time.timeScale = 0;
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