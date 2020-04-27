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
    //adjusts level, adjusts killGoal
    //adjusts enemy scaling
    //does enemy evolution
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

        NextGeneration();

        player.killCount = 0;

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


    //Find the top half of the current population
    //Calculate how many new enemies need to be created to
    //satisfy the kill quota of the next level
    //So that the next generation is the top half of the previous
    //plus the offspring produced to meet the quota
    
    private void NextGeneration()
    {
        //get all the enemies that fought this level
        List<List<float>> prime = new List<List<float>>();
        //foreach (Enemey )
        //sort the current generation based on heuristic
        GameObject elc = GameObject.Find("enemyListContainer");
        //get all battler game objects
        Enemy[] es = elc.GetComponentsInChildren<Enemy>();

        //sort the enemies based on there heuristic
        //highest to lowest
        for (int i = 0; i < es.Length; i++) {
            for (int j = es.Length-1; j > i; j--) {
                if (es[j].performanceHeuristic() > es[j-1].performanceHeuristic()) {
                    Enemy tmp = es[j - 1];
                    es[j - 1] = es[j];
                    es[j] = tmp;
                }
            }
        }
        //Get the previous killGoals number of required instance vectors
        for (int i = 0; i < (killGoal-2) / 2; i++) {
            prime.Add(es[i].GetStats());
        }

        //now this list is the top half of the previous generation,
        //we wish to get to the next killGoal and we have half the previous
        //we must generate (killGoal - prevKillGoal/2) offspring
        Debug.Log("Wheat separated from Chaf"+prime.Count);

        List<List<float>> offSpring = new List<List<float>>();
        int k = 0;
        while (k < (killGoal - (killGoal - 2) / 2)) {
            //pick 2 random prime enemies
            List<float> x = prime[Random.RandomRange(0, prime.Count - 1)];
            List<float> y = prime[Random.RandomRange(0, prime.Count - 1)];
            //produce offspring
            offSpring.Add(Mate(x,y));
            k++;
        }

        //add the offspring to the primes
        k = 0;
        while (k < offSpring.Count)
        {
            prime.Add(offSpring[k]);
            k++;
        }

        //set the primes to the instanceVectors
        instanceVectors = prime;

    }
    


    //givent the vectors of two slimes, mate them together
    //with the given constraint p
    //for each attribute, find the midpoint
    //find the distance between the attributes
    //generate a random number between -distance/2 and distance/2 
    //add this number to the midpoint to get the basis for next attribute
    
    private List<float> Mate(List<float> x, List<float> y)
    {
        List<float> newborn = new List<float>();

        
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
            newborn.Add(result);
        }


        //ensure that the magnitude of the offspring is valid, by way of normalization
        //get magnitude of the vector
        float magnitude = 0;
        for (int i = 0; i < newborn.Count; i++)
        {
            magnitude += Mathf.Pow(newborn[i], 2);
        }
        magnitude = Mathf.Sqrt(magnitude);

        //get the unit vector by dividing each compoenent by the magnitude (normalization)
        //and scaling it by the desired magnitude
        for (int i = 0; i < newborn.Count; i++)
        {
            newborn[i] *= rho / magnitude;
        }

        return newborn;
    }
    



}
