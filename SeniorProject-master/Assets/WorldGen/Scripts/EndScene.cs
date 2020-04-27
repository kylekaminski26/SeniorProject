using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScene : MonoBehaviour
{
    private GameManager gameManager;
    public Text enoughKills;

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
                gameManager.gameLevel++;
                gameManager.killGoal = gameManager.gameLevel / 2;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else
            {
                enoughKills.gameObject.GetComponent<Animator>().SetTrigger("Active");
            }
        }
    }
}
