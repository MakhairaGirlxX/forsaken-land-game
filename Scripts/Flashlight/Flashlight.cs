using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Sets the flashlight on and off depending on if the flashlight button is used
 */
public class Flashlight : MonoBehaviour
{
    public GameObject light;
    public KeyCode flashlightToggle;
    int keyPressed = 0;
    // Start is called before the first frame update
    void Start()
    {
        light.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //if flashlight key is pressed
        if (Input.GetKeyDown(flashlightToggle))
        {
            FlashLightInput();
           
        }
    }
    //turns on and off flashlight depending on how many times the interact key was pressed
    private void FlashLightInput()
    {
        keyPressed++;
        Debug.Log(keyPressed);

        if(keyPressed == 1)
        {
            light.SetActive(true);
        } else if(keyPressed == 2)
        {
            light.SetActive(false);
            keyPressed = 0;
        }
    }
}
