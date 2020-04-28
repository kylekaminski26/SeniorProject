using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsMenu : MonoBehaviour
{

    public bool statsOpen = false;

    public GameObject statsScreen;
    public GameObject button;
    private GameObject player;
    private GameObject healthStamBars;

    public Text attackVal;
    public Text stamVal;
    public Text dexVal;
    public Text healthVal;
    private float health;
    private float stamina;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        healthStamBars = GameObject.FindWithTag("HealthStamBars");
    }
    // Update is called once per frame
    void Update()
    {
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

    public void setStats()
    {
        attackVal.text = ": " + player.GetComponent<PlayerControl>().baseAttack;
        stamVal.text = ": " + player.GetComponent<PlayerControl>().maxStamina;
        dexVal.text = ": " + player.GetComponent<PlayerControl>().dexterity;
        healthVal.text = ": " + player.GetComponent<PlayerControl>().maxHealth;
    }

    public void showStats()
    {
        statsScreen.SetActive(true);
        statsOpen = true;
    }

    public void closeStats()
    {
        statsScreen.SetActive(false);
        statsOpen = false;
    }
}
