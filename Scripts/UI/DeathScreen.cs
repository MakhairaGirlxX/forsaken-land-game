using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
 * Handles the death screen when the player dies
 */
public class DeathScreen : MonoBehaviour
{
    public Image deathScreen;
    public Text screenText;
    float timer = 5;
    bool resetBool = false;

    void Start()
    {
        //subscribes to the PlayerMove.onDeath delegate w
        //when player dies, enable screen
        PlayerMove.onDeath += EnableScreen;
    } 
    void Update()
    {
        //PlayerMove.onDeath += EnableScreen;
    }

    void EnableScreen()
    {
        deathScreen.enabled = true;
        screenText.enabled = true;
        StartCoroutine(Reset());
    }
    //resets and disables the death screen after 5 seconds
    IEnumerator Reset()
    {
        yield return new WaitForSeconds(5f);
       // PlayerMove.onDeath -= EnableScreen;
        deathScreen.enabled = false;
        screenText.enabled = false;

    }
}
