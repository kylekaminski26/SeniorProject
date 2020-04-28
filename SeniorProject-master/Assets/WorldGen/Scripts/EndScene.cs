using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScene : MonoBehaviour
{
    private GameManager gameManager;
    private Text enoughKills;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        enoughKills = GameObject.FindGameObjectWithTag("EnoughKills").GetComponent<Text>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //Get the kill goal
            int kills = gameManager.killGoal;
            if (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>().killCount > kills)
            {
                gameManager.prepareNextLevel();
            }
            else
            {
                enoughKills.gameObject.GetComponent<Animator>().SetTrigger("Active");
            }
        }
    }
}
