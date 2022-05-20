using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour 
{
	[SerializeField] private string mouseXInputName, mouseYInputName;
	[SerializeField] private float mouseSensitivity;

	[SerializeField] private Transform playerBody;

	private float xAxisClamp;

	private void Awake()
	{
		LockCursor ();
		xAxisClamp = 0.0f;
	}

	//locks cursor to the movement of where the player looks
	private void LockCursor()
	{
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void Update()
	{
		//continuouly update where the player is looking
		CameraRotation ();
	}

	private void CameraRotation()
	{
		float mouseX = Input.GetAxis (mouseXInputName) * mouseSensitivity * Time.deltaTime;
		float mouseY = Input.GetAxis (mouseYInputName) * mouseSensitivity * Time.deltaTime;

		xAxisClamp += mouseY;
		//boundaries where the player can look (what degrees)
		if (xAxisClamp > 90.0f) 
		{
			xAxisClamp = 90.0f;
			mouseY = 0.0f;
			ClampXAxisRotationToValue (270.0f);
		}
		else if (xAxisClamp < -90.0f) 
		{
			xAxisClamp = -90.0f;
			mouseY = 0.0f;
			ClampXAxisRotationToValue (90.0f);
		}

		//rotates camera and player based on values
		transform.Rotate (Vector3.left * mouseY);
		playerBody.Rotate (Vector3.up * mouseX);
	}
	//look boundary specifications
	private void ClampXAxisRotationToValue (float value)
	{
		Vector3 eulerRotation = transform.eulerAngles;
		eulerRotation.x = value;
		transform.eulerAngles = eulerRotation;
	}
}
