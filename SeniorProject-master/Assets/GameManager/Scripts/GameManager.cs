using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager instance; //used to make this GameObject a singleton

    public int gameLevel = 1;

    public GameObject[] enemyPrefabList;

    public int killGoal = 5;

    //needs to be tied in scene
    public PlayerControl player;

    //variables concerning evo debugging
    public List<List<float>> instanceVectors; 



    private void Update()
    {
        if (player != null) //player has dead
        {
            if (player.isActiveAndEnabled == false)
            {
                Destroy(gameObject);
                SceneManager.LoadScene("Menu");
            }
        }


        //why is this in update? - Matt
        if (player == null)
        {
            //get player reference
            if (GameObject.Find("Player"))
            {
                player = GameObject.Find("Player").GetComponent<PlayerControl>();
            }
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this; // In first scene, make us the singleton.
            DontDestroyOnLoad(gameObject);


        }
        else if (instance != this)
            Destroy(gameObject); // On reload, singleton already set, so destroy duplicate.
    }


    //added by Matt
    void Start()
    {
        //Generate random initializations for evolutionary algorithm
        instanceVectors = new List<List<float>>();
        generateIntialEnemyVectors();

    }



    //generates as many random enemy vectors as the kill goal of
    //the first level
    void generateIntialEnemyVectors()
    {
        for (int k = 0; k < killGoal; k++)
        {
            List<float> v = new List<float>(){
                Random.Range(.1f, 1), Random.Range(.1f, 1), Random.Range(.1f, 1),
                Random.Range(.1f, 1), Random.Range(.1f, 1), Random.Range(.1f, 1)
            };

            //get magnitude of the vector
            float innerProduct = 0;
            for (int i = 0; i < v.Count; i++)
            {
                innerProduct += Mathf.Pow(v[i], 2);
            }
            float magnitude = Mathf.Sqrt(innerProduct);

            //get the unit vector by dividing each compoenent by the magnitude (normalization)
            for (int i = 0; i < v.Count; i++)
            {
                v[i] /= magnitude;
            }


            //now V is a unit vector in the positive domain of a 6-sphere
            instanceVectors.Add(v);
        }
      


    }

   
}
