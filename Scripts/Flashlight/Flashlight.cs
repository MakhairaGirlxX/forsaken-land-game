using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (Input.GetKeyDown(flashlightToggle))
        {
            FlashLightInput();
           
        }
    }

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
