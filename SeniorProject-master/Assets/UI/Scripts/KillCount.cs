using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillCount : MonoBehaviour
{
    //The purpose oif this script is to keep track of the player's kills,
    //and show it compared to the required number of kills to complete the level.

    public GameObject player; //Keeps track of Player's kills
    public GameObject gameManager; //This is required to find the amount of kills needed to complete the level

    public Text killCount;

    // Start is called before the first frame update
    void Start()
    {
        //Find player object
        player = GameObject.FindWithTag("Player");
        //Find gameManager object
        gameManager = GameObject.FindWithTag("GameController");
    }

    // Update is called once per frame
    void Update()
    {
        //Sets text in top right equal to the amount of kills the player has out of
        //the amount of kills required to complete the level.
        killCount.text = "Kills:\n" + player.GetComponent<PlayerControl>().killCount + " / " +
            gameManager.GetComponent<GameManager>().killGoal;
    }
}
