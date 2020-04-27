using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager instance; //used to make this GameObject a singleton

    public int gameLevel = 1;

    public GameObject[] enemyPrefabList;

    public int killGoal;
    public static int MAX_KILLGOAL = 30;
    public float rho; 

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
        //Generate random initializations for evolutionary algorithm
        killGoal = 6;
        rho = killGoal / MAX_KILLGOAL;
        instanceVectors = new List<List<float>>();
        generateInitialEnemyVectors();
        

        if (instance == null)
        {
            instance = this; // In first scene, make us the singleton.
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
            Destroy(gameObject); // On reload, singleton already set, so destroy duplicate.
    }

    //call this when ready to go to the next level
    public void prepareNextLevel()
    {
        gameLevel++;

        //Adjust the kill goal based on the level
        if (gameLevel < 13)
        {
            killGoal = gameLevel * 2 + 4;
        }
        else
        {
            killGoal = MAX_KILLGOAL;
        }

        //adjust rho (enemy scaling)
        rho = killGoal / MAX_KILLGOAL;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);


    }

    //generates as many random enemy vectors as the kill goal of
    //the first level
    void generateInitialEnemyVectors()
    {
        Debug.Log("hgjhjkjhkjhkj"+killGoal);
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

            //Battler Constant Rangers
            List<float> c = new List<float>() {
                (rho)*Battler.MAX_MAXHEALTH - Battler.MIN_MAXHEALTH,
                (rho)*Battler.MAX_MAXSTAMINA - Battler.MIN_MAXSTAMINA,
                (rho)*Battler.MAX_BASEATTACK - Battler.MIN_BASEATTACK,
                (rho)*Battler.MAX_MAXMOVEMENTSPEED - Battler.MIN_MAXMOVEMENTSPEED,
                (rho)*Battler.MAX_DEXTERITY - Battler.MIN_DEXTERITY,
                (rho)*Battler.MAX_VITALITY - Battler.MIN_VITALITY
            };


            //get the vector product of the (unit vector and the scaled ranges vector)
            for (int i = 0; i < v.Count; i++)
            {
                v[i]*=c[i] ;
            }

            v[0] += Battler.MIN_MAXHEALTH;
            v[1] += Battler.MIN_MAXSTAMINA;
            v[2] += Battler.MIN_BASEATTACK;
            v[3] += Battler.MIN_MAXMOVEMENTSPEED;
            v[4] += Battler.MIN_DEXTERITY;
            v[5] += Battler.MIN_VITALITY;

            //now V is a unit vector in the positive domain of a 6-sphere
            instanceVectors.Add(v);
        }

    }


    //Since I am taking the top half of the winning population
    //every mating process should produce 2 offspring, since
    // Pop = winners + losers
    // winners = pop /2
    //offspring from winners = pop/2/2, so in order to return
    //to the base population , each mating process should double
    /*
    private List<List<float>> NextGeneration(List<List<float>> winners)
    {

        List<List<float>> winnersCopy = new List<List<float>>(winners);

        while (winners.Count != 0)
        {

            //pick 2 slimes from the current generation and set them to the current
            List<float> x = winners[Random.Range(0, winners.Count)]; //retuns int , if you pass integer arguemnts, type inferred

            winners.Remove(x); //discard

            List<float> y = winners[Random.Range(0, winners.Count)];

            winners.Remove(y); //discard

            List<float> offSpringA = Mate(x, y, globalConstraint);
            List<float> offSpringB = Mate(x, y, globalConstraint);
            winnersCopy.Add(offSpringA);
            winnersCopy.Add(offSpringB);

        }
        Debug.Log("new generation generated");
        return winnersCopy;
    }
    */


    //givent the vectors of two slimes, mate them together
    //with the given constraint p
    //for each attribute, find the midpoint
    //find the distance between the attributes
    //generate a random number between -distance/2 and distance/2 
    //add this number to the midpoint to get the basis for next attribute
    /*
    private List<float> Mate(List<float> x, List<float> y, float p)
    {
        List<float> offSpring = new List<float>();

        for (int i = 0; i < x.Count; i++)
        {
            float midPoint = (x[i] + y[i]) / 2;
            float distance = Mathf.Abs(x[i] - y[i]);
            float mutationValue = Random.Range(-1 * distance / 2, distance / 2);
            float result = midPoint + mutationValue;
            //cover my ass if we get negative values
            if (result <= 0)
            {
                result = .1f;
            }
            offSpring.Add(result);
        }


        //ensure that the magnitude of the offspring is valid, by way of normalization
        //get magnitude of the vector
        float magnitude = 0;
        for (int i = 0; i < offSpring.Count; i++)
        {
            magnitude += Mathf.Pow(offSpring[i], 2);
        }
        magnitude = Mathf.Sqrt(magnitude);

        //get the unit vector by dividing each compoenent by the magnitude (normalization)
        //and scaling it by the desired magnitude
        for (int i = 0; i < offSpring.Count; i++)
        {
            offSpring[i] *= p / magnitude;
        }

        return offSpring;
    }
    */



}
