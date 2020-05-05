using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class MainMenu : MonoBehaviour
{
    //The purpose of this script is to give functionality to the main
    //menu scene. It provides the audio and allows it to change
    //scenes (to the Main scene, the main game)

    AudioSource aud;

    //Functionality for New Game button with audio and calls ChangeScene()
    public void NewGame()
    {
        aud = GetComponent<AudioSource>();
        aud.Play();
	ChangeScene();
    }

    //Changed the scene from Menu to Main Scene (the game)
    public void ChangeScene()
    {
        SceneManager.LoadScene("Main Scene");
    }
    //Functionality for Quit button to quit the game
    public void QuitGame() 
    {
        //Quit button does not do anything inside the Unity editor.
        //If it were in a .exe form, this would close the window holding the game.
        Debug.Log("QUIT!");
        Application.Quit();
    }

    
}
