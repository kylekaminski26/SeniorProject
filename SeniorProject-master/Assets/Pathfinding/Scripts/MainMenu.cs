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
        SceneManager.LoadScene(1);
    }

    public void QuitGame() 
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }

    
}
