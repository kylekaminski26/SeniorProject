using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillCount : MonoBehaviour
{

    public GameObject player;
    public GameObject gameManager;

    public Text killCount;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        gameManager = GameObject.FindWithTag("GameController");
    }

    // Update is called once per frame
    void Update()
    {
        killCount.text = "Kills:\n" + player.GetComponent<PlayerControl>().killCount + " / " +
            gameManager.GetComponent<GameManager>().killGoal;
    }
}
