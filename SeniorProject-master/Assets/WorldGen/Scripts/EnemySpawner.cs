using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private GameManager gameManager;

    private int rand;
    private double random;
    public float waitTime = 1f;

    // Start is called before the first frame update
    void Start()
    {
        //Destroy spawner object after 'waitTime'
        Destroy(gameObject, waitTime);
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        Spawn();
    }

    public void Spawn()
    {
        rand = Random.Range(0, gameManager.enemyList.Length);
        GameObject enemy = Instantiate(gameManager.enemyList[rand], transform.position, Quaternion.identity);
        enemy.transform.parent = gameManager.enemyListContainer.transform;
    }
}
