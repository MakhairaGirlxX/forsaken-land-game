using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Loads the next level (basement) when the player has collected the janitor keys and used them in the elevator
 */
public class LoadBasement : MonoBehaviour
{
    public GameObject player;

    void Update()
    {
        if (player.GetComponent<UIPrompts>().nextScene)
        {
            StartCoroutine(Load());
        }
    }

    IEnumerator Load()
    {
        yield return new WaitForSeconds(6f);
        Debug.Log("loaded");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
