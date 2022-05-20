using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
/*
 * Handles the start menu functionality
 */
public class StartMenu : MonoBehaviour
{
    public GameObject quitBtn, playBtn;

    void Start()
    {
        //quit button setup
        Button qBtn = quitBtn.GetComponent<Button>();
        qBtn.onClick.AddListener(Quit);
        //play button setup
        Button pBtn = playBtn.GetComponent<Button>();
        pBtn.onClick.AddListener(Play);
        //sets highlighted button to the play button on start
        EventSystem.current.SetSelectedGameObject(playBtn);
    }
    //if play is selected, next scene is loaded by increasing the build index
    void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    //quits the game
    void Quit()
    {
        Application.Quit();
    }
}
