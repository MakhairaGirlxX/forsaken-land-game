using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Controls when the elevator doors are opened after the basement scene is loaded (next level)
 */
public class Basement : MonoBehaviour
{
    public GameObject elevator, elevatorLeftDoor, elevatorRightDoor;
    Animator rightDoorAnim, leftDoorAnim, elevatorAnim;
    // Start is called before the first frame update
    void Start()
    {
        rightDoorAnim = elevatorRightDoor.GetComponent<Animator>();
        leftDoorAnim = elevatorLeftDoor.GetComponent<Animator>();
        elevatorAnim = elevator.GetComponent<Animator>();       

        StartCoroutine(OpenDoors());       
        
    }
    IEnumerator OpenDoors()
    {
        yield return new WaitForSeconds(3f);
        rightDoorAnim.SetTrigger("RightDoorTrigger");
        leftDoorAnim.SetTrigger("LeftDoorTrigger");
        elevatorAnim.SetTrigger("Default");
    }
}
