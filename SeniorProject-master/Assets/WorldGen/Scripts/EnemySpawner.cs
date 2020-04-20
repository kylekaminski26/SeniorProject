using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private GameManager gameManager;

    private int rand;
    private double random;
    public float waitTime = 1f;
    public double percentChance = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, waitTime);
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

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
        Instantiate(gameManager.enemyList[rand], transform.position, Quaternion.identity);
    }
}
