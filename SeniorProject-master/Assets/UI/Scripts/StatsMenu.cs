using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsMenu : MonoBehaviour
{
    //This script is used for the Stats menu in the main game's UI.
    //It is responsible for showing, hiding, and loading the stats menu
    //with proper values from the player.

    public bool statsOpen = false;

    public GameObject statsScreen;
    public GameObject button;
    private GameObject player;
    

    public Text attackVal;
    public Text stamVal;
    public Text dexVal;
    public Text healthVal;
    public Text moveSpeed;
    public Text vitalityVal;
    

    private void Start()
    {
        //Locate the player object in the game.
        player = GameObject.FindWithTag("Player");
        
    }
    // Update is called once per frame
    void Update()
    {
        //"K" button will open/close stats menu, based on bool statsOpen
        if (Input.GetKeyDown(KeyCode.K) )
        {
            if (statsOpen)
            {
                closeStats();
            }
            else
            {
                setStats();
                showStats();
            }
        }
    }

    //Gets the stat values from the player object and loads the
    //stats menu with these values.
    public void setStats()
    {
        attackVal.text = ": " + player.GetComponent<PlayerControl>().baseAttack;
        stamVal.text = ": " + player.GetComponent<PlayerControl>().maxStamina;
        dexVal.text = ": " + player.GetComponent<PlayerControl>().dexterity;
        healthVal.text = ": " + player.GetComponent<PlayerControl>().maxHealth;
        moveSpeed.text = ": " + player.GetComponent<PlayerControl>().movementSpeed;
        vitalityVal.text = ": " + player.GetComponent<PlayerControl>().vitality;


    }
    //Opens stats menu
    public void showStats()
    {
        statsScreen.SetActive(true);
        statsOpen = true;
    }
    //Closes stats menu
    public void closeStats()
    {
        statsScreen.SetActive(false);
        statsOpen = false;
    }
}
