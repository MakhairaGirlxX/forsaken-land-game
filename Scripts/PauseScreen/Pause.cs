using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
/*
 * Handles if the game is paused or not
 */
public class Pause : MonoBehaviour
{
    [SerializeField] private KeyCode pauseKey, backKey;
    public GameObject quitBtn, resumeBtn, pauseScreen;
   
    void Start()
    {
       // Time.timeScale = 0;
        //quit button setup
        Button startBtn = quitBtn.GetComponent<Button>();
        startBtn.onClick.AddListener(OpenStartScreen);

        //resume button setup
        Button rBtn = resumeBtn.GetComponent<Button>();
        rBtn.onClick.AddListener(Resume);

        EventSystem.current.SetSelectedGameObject(resumeBtn);
    }

  
    void Update()
    {
        if (Input.GetKeyDown(pauseKey) && pauseScreen.gameObject.activeSelf == false)
        {
            pauseScreen.SetActive(true);
           // Time.timeScale = 1;
        } else if ((Input.GetKeyDown(pauseKey) || Input.GetKeyDown(backKey)) && pauseScreen.gameObject.activeSelf == true)
        {
            pauseScreen.SetActive(false);
            //Time.timeScale = 0;
        }
    }
    //opens the start screen by decrementing the build index by one
    void OpenStartScreen()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
    //resumes the game
    void Resume()
    {
        pauseScreen.SetActive(false);
        Time.timeScale = 1;
    }
}
