using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private GameManager gameManager;
    private RoomTemplates templates;

    private int rand;
    private double random;
    public float waitTime = 1f;

    // Start is called before the first frame update
    void Start()
    {
        //Destroy spawner object after 'waitTime'
        Destroy(gameObject, waitTime);
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        templates = GameObject.FindGameObjectWithTag("RoomTemplate").GetComponent<RoomTemplates>();

        Spawn();

    }

    public void Spawn()
    {
        rand = Random.Range(0, gameManager.enemyPrefabList.Length);
        GameObject enemy = Instantiate(gameManager.enemyPrefabList[rand], transform.position, Quaternion.identity);
        Enemy enemyInstance = enemy.GetComponent<Enemy>();
        Debug.Log("instanceVector count " + gameManager.instanceVectors.Count);
        int ndx = Random.Range(0, gameManager.instanceVectors.Count);
        List<float> enemyData = gameManager.instanceVectors[ndx];
        //gameManager.instanceVectors.RemoveAt(ndx); cant guarentee all vectors will be used
        //due to the nature of this implementation (but okay)
        enemyInstance.SetStats(enemyData[0],enemyData[1],enemyData[2],enemyData[3],enemyData[4],enemyData[5]);
        enemy.transform.parent = templates.enemyListContainer.transform;
    }

 
}
