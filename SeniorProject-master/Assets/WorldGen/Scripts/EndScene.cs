using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScene : MonoBehaviour
{
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
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
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}
