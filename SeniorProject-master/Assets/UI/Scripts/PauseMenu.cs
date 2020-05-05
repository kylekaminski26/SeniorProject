using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    //The purpose of this script it to handle the functionality
    //of the Pause Menu in the game. It is responsible for showing,
    //hiding, and handling actions of the pause menu.

    public static bool gameIsPaused = false;

    //Finds the pause menu game object to be used
    public GameObject pauseMenuUI;

    // Update is called once per frame
    void Update()
    {
        //When "Escape" key is pressed, open or close menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    //Resumes the game (by closing the pause menu)
    public void Resume()
    {
        //closes pause menu
        pauseMenuUI.SetActive(false);
        //resumes game and allows actions to occur again
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    //Pauses the game (by opening the pause menu)
    void Pause()
    {
        //opens pause menu
        pauseMenuUI.SetActive(true);

        //This prevents any action or movement from occurring 
        Time.timeScale = 0f;


        gameIsPaused = true;
    }

    //Loads the starting menu scene (exits game)
    public void LoadMenu()
    {
    //TimeScale is set back to 1 since it would be 0 in the pause menu
	Time.timeScale = 1f;

	gameIsPaused = false;

    //This will load the scene called "Menu" which is the start menu scene.
    SceneManager.LoadScene("Menu");
    }

    //No functionality, since quitting the game in the unity editor
    //does't actually do anything, so I didnt bother keeping the code in.
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
    }

    
}
