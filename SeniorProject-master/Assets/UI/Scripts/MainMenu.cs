using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class MainMenu : MonoBehaviour
{

    AudioSource aud;

    public void NewGame()
    {
        aud = GetComponent<AudioSource>();
        aud.Play();
	ChangeScene();
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene("Main Scene");
    }

    public void QuitGame() 
    {
        //Debug.Log("QUIT!");
        #if UNITY_EDITOR
            // Application.Quit() does not work in the editor so
            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
            UnityEditor.EditorApplication.isPlaying = false;
        #else
                 Application.Quit();
        #endif
    }


}
