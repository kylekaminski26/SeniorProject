using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager instance; //used to make this GameObject a singleton

    public int gameLevel = 1;

    public GameObject[] enemyList;

    //needs to be tied in scene
    public PlayerControl player;

    private void Update()
    {
        if (player != null)
        {
            if (player.isActiveAndEnabled == false)
            {
                Destroy(gameObject);
                SceneManager.LoadScene("Menu");
            }
        }

        if (player == null)
        {
            //get player reference
            if (GameObject.Find("Player"))
            {
                player = GameObject.Find("Player").GetComponent<PlayerControl>();
            }
        }
    }

    void Awake()
    {

        if (instance == null)
        {
            instance = this; // In first scene, make us the singleton.
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
            Destroy(gameObject); // On reload, singleton already set, so destroy duplicate.
    }

   
}
