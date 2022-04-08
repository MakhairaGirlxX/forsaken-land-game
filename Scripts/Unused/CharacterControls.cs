using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControls : MonoBehaviour {

	private CharacterController charController;
	[SerializeField] private string horizontalInputName;
	[SerializeField] private string verticalInputName;
	[SerializeField] private float movementSpeed;
	private bool isMoving = false;
	public Inputs i;

	void Awake()
	{
		charController = GetComponent <CharacterController>();
		charController.height = 2.0f;
		//Debug.Log ("Called");
		i = new Inputs(charController);
	}
	
	// Update is called once per frame
	void Update () {
		getPlayerMovement ();
		i.getInput();
		/*PlayerMovement();			
		//LeanInput ();
		//PivotTo (pos);
		if (Input.GetKeyDown (crouchInput)) {
			startTime = Time.time;
			isDown = false;
		} 
		else if (Input.GetKeyUp (crouchInput)) {
			if (!isDown) {
				c.CrouchInput ();
				isDown = false;
				lieSound.Stop ();
			}
			isDown = false;
		}
		if (Input.GetKey (crouchInput)) {
			if (Time.time - startTime > holdTime) {
				ld.Lie ();
				isDown = true;
			}
		}	
		*/
	}

	private void getPlayerMovement()
	{

			float horizInput = Input.GetAxisRaw (horizontalInputName) * movementSpeed;
			float vertInput = Input.GetAxisRaw (verticalInputName) * movementSpeed;

			Vector3 forwardMovement = charController.transform.forward * vertInput;
			Vector3 rightMovement = charController.transform.right * horizInput;

			charController.SimpleMove (forwardMovement + rightMovement);

	}
}

[System.Serializable]
public class Inputs : IInput
{
	public float movementSpeed;
    private CharacterController charC;

	public SprintControl s;

	public Inputs(CharacterController charC)
	{
		Debug.Log (movementSpeed);
		this.charC = charC; 
		s = new SprintControl (charC);
	}

	public void getInput()
	{
		//s.getPlayerInput ();
	}

	public Inputs next = null;

}
		

[System.Serializable]
public class SprintControl
{
	public float sprintSpeed;
	public KeyCode sprintKey;

	private CharacterController charC;
	//private Inputs i;

	public SprintControl(CharacterController charC)
	{
		this.charC = charC;
	//	i = new Inputs (charC);
	}

	public void getPlayerInput(float m)
	{
		if (charC.isGrounded && Input.GetKeyDown ("joystick button 8")) 
		{
			//isSprinting = true;
			m = sprintSpeed;
		} 

		else if (Input.GetKeyUp ("joystick button 8")) 
		{
			//isSprinting = false;
			m = 3.0f;
		}
	}
}

interface IInput
{
	void getInput();
}
