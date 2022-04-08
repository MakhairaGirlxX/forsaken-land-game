using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRotate : MonoBehaviour
{
    [SerializeField] private string leanLeftInput, leanRightInput;

    float maxAngle = 45.0f;
    float speed = 45.0f;
    public GameObject cube;


    bool isRight = false;
    bool isLeft = false;
    // Update is called once per frame
    void Update()
    {
        
       // Vector3 direction = objectToLookAt.transform.position - position;
       // Quaternion targetRotation = Quaternion.LookRotation(direction);
       /*
        //rotate left
        if ((Input.GetAxisRaw(leanLeftInput)) != 0 && !isRight)
        {
           float currentAngle = Mathf.MoveTowardsAngle(cube.transform.eulerAngles.x, 45.0f, speed * Time.deltaTime);
           cube.transform.eulerAngles = new Vector3(currentAngle, cube.transform.eulerAngles.y , cube.transform.eulerAngles.z);

            isLeft = true;
        }

        //rotate right
        if ((Input.GetAxisRaw(leanRightInput)) != 0 && !isLeft)
        {
            float rightAngle = Mathf.MoveTowardsAngle(cube.transform.eulerAngles.x, -45.0f, speed * Time.deltaTime);
            cube.transform.eulerAngles = new Vector3(rightAngle, cube.transform.eulerAngles.y, cube.transform.eulerAngles.z);

            isRight = true;
        }

        //rotate back
        if ((Input.GetAxisRaw(leanLeftInput)) == 0 || (Input.GetAxisRaw(leanRightInput)) == 0)
        {
            float firstAngle = Mathf.MoveTowardsAngle(cube.transform.eulerAngles.x, 0.0f, speed * Time.deltaTime);
            cube.transform.eulerAngles = new Vector3(firstAngle, cube.transform.eulerAngles.y, cube.transform.eulerAngles.z);

            isLeft = false;
            isRight = false;
        }
       */

        
        if (isRight == false)
        {
            if ((Input.GetAxisRaw(leanLeftInput)) != 0)
            {
                 float currentAngle = Mathf.MoveTowardsAngle(cube.transform.eulerAngles.x, 45.0f, speed * Time.deltaTime);
                 cube.transform.eulerAngles = new Vector3(currentAngle, cube.transform.eulerAngles.y , cube.transform.eulerAngles.z);
                 isLeft = true;
                
            }
            if ((Input.GetAxisRaw(leanLeftInput)) == 0)
            {
                float firstAngle = Mathf.MoveTowardsAngle(cube.transform.eulerAngles.x, 0.0f, speed * Time.deltaTime);
                cube.transform.eulerAngles = new Vector3(firstAngle, cube.transform.eulerAngles.y, cube.transform.eulerAngles.z);
                isLeft = false;
            }
        }


        if (isLeft == false)
        {
            if ((Input.GetAxisRaw(leanRightInput)) != 0)
            {
                 float rightAngle = Mathf.MoveTowardsAngle(cube.transform.eulerAngles.x, -45.0f, speed * Time.deltaTime);
                 cube.transform.eulerAngles = new Vector3(rightAngle, cube.transform.eulerAngles.y, cube.transform.eulerAngles.z);
                 isRight = true;             
            }

            if ((Input.GetAxisRaw(leanRightInput)) == 0)
            {
                float firstAngle = Mathf.MoveTowardsAngle(cube.transform.eulerAngles.x, 0.0f, speed * Time.deltaTime);
                cube.transform.eulerAngles = new Vector3(firstAngle, cube.transform.eulerAngles.y, cube.transform.eulerAngles.z);
                isRight = false;
            }
        }
       // leanPivot.transform.localRotation = Quaternion.AngleAxis(curAngle, Vector3.forward);
        /*
         *  transform.Rotate(new Vector3(45, 0, 0) * speed * Time.deltaTime, Space.Self);
            if (Mathf.Round(transform.eulerAngles.x) == 45)
            {
                rb.freezeRotation = true;
            }
        */
    }
}
