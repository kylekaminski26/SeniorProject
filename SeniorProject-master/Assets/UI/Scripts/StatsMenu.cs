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
   

    public Text attackVal;
    public Text stamVal;
    public Text dexVal;
    public Text healthVal;
    public Text ammoVals;
    

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        
    }
    // Update is called once per frame
    void Update()
    {
        ammoVals.text = player.GetComponent<PlayerControl>().currentAmmo + "/" +
            player.GetComponent<PlayerControl>().maxAmmo;
        if (Input.GetKeyDown(KeyCode.K) )
        {
            if (statsOpen)
            {
                CloseStats();
            }
            else
            {
                SetStats();
                ShowStats();
            }
        }
    }

    public void SetStats()
    {
        attackVal.text = ": " + player.GetComponent<PlayerControl>().baseAttack;
        stamVal.text = ": " + player.GetComponent<PlayerControl>().maxStamina;
        dexVal.text = ": " + player.GetComponent<PlayerControl>().dexterity;
        healthVal.text = ": " + player.GetComponent<PlayerControl>().maxHealth;
    }

    public void ShowStats()
    {
        statsScreen.SetActive(true);
        statsOpen = true;
    }

    public void CloseStats()
    {
        statsScreen.SetActive(false);
        statsOpen = false;
    }
}
