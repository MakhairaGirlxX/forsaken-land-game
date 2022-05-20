using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
 * This script runs the entire Player Movement and character controller other than looking around (handled in PlayerLook)
 * Handles: move, crouch, liedown, jump, sprint, lean left and right
 * Also set the delegate for Death that other scripts subscribe to in order to reset when the player respawns
 * Sounds are set here as well for crouching, running, walking, idling etc...
 * 
 * 
 * Right now Lie() doesn't work. I had it working earlier but the character collider is too low for the enemy raycast to detect so I took it out for now
 */
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

	private bool isJumping;
	public bool isSprinting;
	public bool isMoving = false;
	public bool isDown = false;
	public bool isCrouching = false;

	public bool isWalkingCheck = false;

	private int keyPressed = 0;
	private int downKeyPressed = 0;
	private int healthAmount = 100;

	private CharacterController charController;

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

	public delegate void MyDelegate();
	public static event MyDelegate onDeath;
	public GameObject enemy;
	public GameObject player;

	public bool isDead = false;

	bool resetBool = false;

	//on Awake(), the stillSound is called (specified in the inspector window) and the character height is set 
	private void Awake()
	{
		stillSound.Play ();
		charController = GetComponent <CharacterController>();
		charController.height = 4.0f;
	}

	private void Update()
	{
		//continuously update playerMovement and actions frame by frame
		PlayerMovement();
		//method to lean left and right (l and r trigger)
		Lean();
		//if the player is caught by the enemy in the janitor office or not, calls the onDeath delegate
		if(enemy.GetComponent<EnemyJanitorRoom>().isCaught || enemy.GetComponent<NewEnemyAI>().isCaught)
        {
			onDeath.Invoke();
			//invokes and subscribes to the onDeath delegate (can now be called across all scripts using this delegate)
			onDeath += Death;
			onDeath();
        }
		//once the game is ready to reset, unsubscribes to the onDeath delegate and starts the reset coroutine
        if (resetBool)
        {
			onDeath -= Death;
			StartCoroutine(Reset());
        }

		//if the crouch key is pressed, a timer starts to detect lying down
		if (Input.GetKeyDown (crouchInput)) {
			startTime = Time.time;
		} 
		else if (Input.GetKeyUp (crouchInput)) {
			//if the player isn't already lying down, allow the player to crouch
			if (!isDown) {
					CrouchInput();
					isDown = false;
					lieSound.Stop();				
			}
			isDown = false;

		}
		//if the crouch key is pressed and held for a certain amount of time the player will liedown
		if (Input.GetKey (crouchInput)) {
			if (Time.time - startTime > holdTime) {
				Lie ();
				isDown = true;
			}
		}
	
	}
	//method called by the delegate
	void Death()
    {
		//if the player has collected the janitor keys, the respawn point will be set inside the janitor office
		if (player.GetComponent<ItemList>().stop == true)
        {
			//sets the players position
			transform.position = new Vector3(44, 1, -17);
			//disables the player from moving while the game is resetting
			charController.enabled = false;
			resetBool = true;
		}
        else
        {
			//if the player hasn't collected the keys, the respawn point is set right before the player first runs into the enemy
			transform.position = new Vector3(44, 1, -23);
			charController.enabled = false;
			resetBool = true;
		}

	}
	//Once the character dies and is transformed to a new position, reset is called
	IEnumerator Reset()
    {
		//after 3 seconds, the player is reset as well
		yield return new WaitForSeconds(3f);
		charController.enabled = true;
		charController.height = 4.0f;
	}

	private void PlayerMovement()
	{
		//this checks the position of the joysticks on the controllers to determine which way the player moves
		if ((((Input.GetAxisRaw(horizontalInputName)) > 0.2) || ((Input.GetAxisRaw(horizontalInputName)) < -0.2)) || (((Input.GetAxisRaw(verticalInputName)) > 0.2) || ((Input.GetAxisRaw(verticalInputName)) < -0.2))) 
		{
			isMoving = true;
			isWalkingCheck = true;
			//if crouching but not moving, set the appropriate sound trigger
			if (isCrouching == true) {
				stillSound.Stop ();
				walkSound.Stop ();
				isWalkingCheck = false;
			}
			//not crouching and not moving
			stillSound.Play ();
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
		}
		//sets the movement speed and the character position in the game as they move across the space
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
		//jump and sprint are called here since they can't be continously updated
		JumpInput ();
		SprintInput ();	

	}

	//sprint
	private void SprintInput()
	{
		//if the character isn't jumping (touching the ground) and the sprint key is pressed, the movement speed and appropriate sounds are updated
		if (charController.isGrounded && Input.GetKeyDown ("joystick button 8")) 
		{
			isSprinting = true;
			movementSpeed = sprintSpeed;

			runSound.Play();
			walkSound.Stop ();
			stillSound.Stop ();
		} 
		//if the sprint button isn't being held down any longer
		else if (Input.GetKeyUp ("joystick button 8")) 
		{
			isSprinting = false;
			movementSpeed = 3.0f;

			runSound.Stop ();
			walkSound.Play ();
		}
	}

	//jump
	private void JumpInput()
	{
		//if the jump key is pressed and the player isn't jumping
		if (Input.GetKeyDown (jumpKey) && !isJumping)
		{
			isJumping = true;
	
			jumpSound.Play();
			walkSound.Stop ();
			stillSound.Stop ();
			runSound.Stop ();
			//Start coroutine for the jumping action
			StartCoroutine (JumpEvent ());
		}
	}

	private IEnumerator JumpEvent()
	{
		//sets the incline
		charController.slopeLimit = 90.0f;
		float timeInAir = 0.0f;
		//logic for the jumping action while the player is in the air and there is nothing above the player (jump curve is set in inspector)
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

	}
	//Doesn't work right now
	private void Lie()
	{
		/*Ray ray = new Ray(transform.position, Vector3.up);
		RaycastHit Hit;
		downKeyPressed++;

		isDown = true;

		if (isDown == true)
		{
		*/
			charController.height = 0f;
			movementSpeed = 1.0f;
			lieSound.Play();
			crouchSound.Stop();

		/*}

		if (downKeyPressed > 1)
		{

			if (Physics.Raycast(ray, out Hit))
			{
				float distanceUp = Vector3.Distance(transform.position, Hit.point);
				downKeyPressed = 1;
				isDown = true;
			}
			else if (Hit.collider == null)
			{
				isDown = false;
				charController.height = 3.0f;
				movementSpeed = 3.0f;
				//crouchSound.Stop ();
				downKeyPressed = 0;
			}
		}
		*/
	}
	//crouch
	private void CrouchInput()
	{
		//uses a raycast system to detect collisions above
		Ray ray = new Ray (transform.position, Vector3.up);
		RaycastHit Hit;
		//determines how many times the crouch button has been pressed
		keyPressed++;

		isCrouching = true;
		//lowers the player so it looks like they are crouching and sets the speed to a lower speed
		if (isCrouching == true) {
			Debug.Log("Crouching");
			movementSpeed = crouchSpeed;
			charController.height = 2.0f;

			crouchSound.Play ();
			stillSound.Stop ();
			walkSound.Stop ();

		} 

		if (keyPressed > 1) {
			//if there is something detected above the players head, they cannot return to a standing position
			if (Physics.Raycast(ray, out Hit))
			{
				float distanceUp = Vector3.Distance(transform.position, Hit.point);
				keyPressed = 1;
				isCrouching = true;
			}
			//otherwise if the ray doesn't detect anything, the player can stand and the keyPressed is set back to zero for future use
			else if (Hit.collider == null)
			{
				isCrouching = false;
				charController.height = 4.0f;
				movementSpeed = 3.0f;
				keyPressed = 0;
			}
		}
		
	}
	//leaning function which allows the player to lean to the left or right. It doesn't rotate the camera 45 degrees, it also moves the camera out from the player so they can see beyond wall corners
	public void Lean()
    {
		//if not leaning right
		if (isRight == false)
		{
			//if the lean trigger is pressed 
			if ((Input.GetAxisRaw(leanLeftInput)) != 0)
			{
				//move the rotation position to a certain angle at a certain speed (eulerAngles are because this is set in a 3-dimensional space)
				float currentAngle = Mathf.MoveTowardsAngle(leanPivot.transform.eulerAngles.x, maxLeanAngle, speed * Time.deltaTime);
				leanPivot.transform.eulerAngles = new Vector3(currentAngle, leanPivot.transform.eulerAngles.y, leanPivot.transform.eulerAngles.z);
				isLeft = true;

			}
			//set back to original position when the left trigger isn't being pressed anymore
			if ((Input.GetAxisRaw(leanLeftInput)) == 0)
			{
				float firstAngle = Mathf.MoveTowardsAngle(leanPivot.transform.eulerAngles.x, 0.0f, speed * Time.deltaTime);
				leanPivot.transform.eulerAngles = new Vector3(firstAngle, leanPivot.transform.eulerAngles.y, leanPivot.transform.eulerAngles.z);
				isLeft = false;
			}
		}

		//same as left trigger function but on the right
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
}