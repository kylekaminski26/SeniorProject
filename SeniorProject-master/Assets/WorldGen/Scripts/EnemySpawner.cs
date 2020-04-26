using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private GameManager gameManager;
    private GameObject enemiesContainer;

    private int rand;
    private double random;
    public float waitTime = 1f;
    public double percentChance = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, waitTime);
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        enemiesContainer = GameObject.FindGameObjectWithTag("EnemiesContainer");

        //only generate enemies percentChance of times
        random = Random.Range(0.0f, 1.0f);
        if (random < percentChance)
        {
            Spawn();
        }
    }

    public void Spawn()
    {
        rand = Random.Range(0, gameManager.enemyList.Length);
        GameObject b = Instantiate(gameManager.enemyList[rand], transform.position, Quaternion.identity);
        b.transform.parent = enemiesContainer.transform;
    }
}
