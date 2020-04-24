using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//for file writing
using System.IO;

public class GameController : MonoBehaviour
{

    // Start is called before the first frame update
    //can be used to initialize enemies, players and positions

    public GameObject Slime; //holds a reference to the slime prefab

    public List<List<float>> slimeVectors; //holds the slime vectors in the current generation

    public List<List<float>> winningVectors;//holds all the slime vectors that won there battles
    public List<List<float>> losingVectors;//holds all the losing slime vectors

    public List<List<List<float>>> slimeGenerations; //this is a list of the winners for each generation
                                                     //note that a list of slimes is a list of lists,
                                                     //thus this list holds each generation of winning slimes

    //variables to hold the currently battling slimes
    private Slime slimeA;
    private Slime slimeB;

    //holds the attributes of the fighting slimes
    public List<float> slimeAVector;
    public List<float> slimeBVector;


    public int gen = 0;
    public int capacity = 12; //how many slimes exist in each generation
    //current constrain is that this is divisible by 3, so i can take the top two thirds and mate for another 3rd
    public float globalConstraint = 8;

    public bool battleOver = false;
    public bool winnerShowcase = false;

    void Start()
    {

        winningVectors = new List<List<float>>(); //holds the winning slimes of the generation
        losingVectors = new List<List<float>>(); //holds all of the losing slimes

        //generate the first generation of slimes
        slimeVectors = GenerateSlimes(capacity,globalConstraint);
        slimeGenerations = new List<List<List<float>>>();


    }

    // Update is called once per frame
    void Update()
    {
        //execute the following there are slimes currently fighting or slimes are left to fight, otherswise
        //all slimes in this generation hav fought
        if (!(slimeA == null && slimeB == null && slimeVectors.Count == 0))
        {


            //either one of the battling slimes is dead, or the have never been instantiated
            if (slimeA == null || slimeB == null && !battleOver)                                    //((slimeA == null && slimeB != null) || (slimeB == null && slimeA != null) && !battleOver)
            {
                //one is dead, current battle is over
                if ((slimeA == null && slimeB != null) || (slimeA != null && slimeB == null))
                {

                    //are we already processing the death of a slime ?
                    if (!winnerShowcase)
                    {

                        battleOver = true;
                        //determine which slime has won the battle,
                        //store it in the winning list, and kill it off, to begin the next battle
                        if (slimeA != null)
                        {
                            Debug.Log(slimeAVector[0] +  " : " + slimeAVector[1] + " : " + slimeAVector[2]);
                            winningVectors.Add(slimeAVector);
                            losingVectors.Add(slimeBVector);
                        }
                        else
                        {
                            winningVectors.Add(slimeBVector);
                            losingVectors.Add(slimeAVector);
                        }

                        //kill the remaining instance
                        Slime kill = slimeA == null ? slimeB : slimeA;
                        StartCoroutine(KillSlime(kill));

                        
                    }
                }

                //no slimes have been instantiated yet, instantiate slimes
                else
                {
                    Pick2Slimes();
                }
            }


        }

        //this section implies that the current generation has battled all of each other
        else{
            Debug.Log("no slimes left, generate new");
            slimeGenerations.Add(winningVectors);

            slimeVectors = NextGeneration(winningVectors);
            Debug.Log("Size of new generation" + slimeVectors.Count);
            //winningVectors.Clear();
        }
    }

    /*
     *pick 2 slimes from the current generation and set them to the current
    */
    public void Pick2Slimes()
    {
        slimeAVector = slimeVectors[Random.Range(0, slimeVectors.Count)]; //retuns int , if you pass integer arguemnts, type inferred

        slimeVectors.Remove(slimeAVector); //discard

        slimeBVector = slimeVectors[Random.Range(0, slimeVectors.Count)];

        slimeVectors.Remove(slimeBVector); //discard

        //instantiate the slimes
        slimeA = (Instantiate(Slime, new Vector3(1, -2, 0), Quaternion.identity)).GetComponent<Slime>();
        slimeB = (Instantiate(Slime, new Vector3(1, 4, 0), Quaternion.identity)).GetComponent<Slime>();
        //set the slimes to target each other
        slimeA.SetTarget(slimeB.gameObject.transform); //set the target to the scripts game object's transform
        slimeB.SetTarget(slimeA.gameObject.transform);

        //give the attribute to the two slimes instances

        //float maxHealth, float maxStamina, float baseAttack, float movementSpeed, float dexterity

        slimeA.SetStats(slimeAVector[0],slimeAVector[1],slimeAVector[2],slimeAVector[3],slimeAVector[4]);
        slimeB.SetStats(slimeBVector[0], slimeBVector[1], slimeBVector[2], slimeBVector[3], slimeBVector[4]);


        //enable them
        slimeA.gameObject.SetActive(true);
        slimeB.gameObject.SetActive(true);

        battleOver = false;
    }
    


    /* Generate N random slimes with the given constraint
     * 
     * Variable Augments V = {v1,v2,....} are constrained
     * by ||V||^2 = p
     *
     * Where a variable augment is the movement from the base value
     * The protoypical slime is
     * given by {v1i ,v2i, v3i, ... }
     *
     */
    private List<List<float>> GenerateSlimes(int n, float p)
    {

        List<List<float>> newGeneration = new List<List<float>>();

        //give the slimes different attributes
        //(float maxHealth, float maxStamina, float baseAttack, float movementSpeed, float dexterity, chaseRadius

        //variable minumums
        float maxHealthMin = 20f;
        float maxStaminaMin = 15f;
        float baseAttackMin = 4f;
        float MovementSpeedMin = 2f;
        float dexterityMin = 8f;

        //put vars into a 'vector' shitty unity does not
        //support n-dimensional algebra :(
        List<float> minVector = new List<float>();
        minVector.Add(maxHealthMin);
        minVector.Add(maxStaminaMin);
        minVector.Add(baseAttackMin);
        minVector.Add(MovementSpeedMin);
        minVector.Add(dexterityMin);


        //initialize random augments
        //v1^2  + v2^2 + v3^2 + ... = p^2

        //in the following implementation are augments will only be baseAttack and movementSpeed


        for (int i = 0; i < n; i++)
        {

            //determine variable stats
            float baseAttack = Random.Range(.1f, p);
            float movementSpeed = Random.Range(.1f, p);

            //since we are only varying baseAttack and movementSpeed
            //all other variables augments will be set to 0, as to
            //have no impact on the augment vector

            //put vars into a 'vector' shitty unity does not
            //support n-dimensional algebra :(
            List<float> augmentVector = new List<float>();
            augmentVector.Add(.1f);
            augmentVector.Add(.1f);
            augmentVector.Add(baseAttack);
            augmentVector.Add(movementSpeed);
            augmentVector.Add(.1f);

            //get magnitude of the vector
            float magnitude = 0;
            for (int j = 0; j < augmentVector.Count; j++)
            {
                magnitude += Mathf.Pow(augmentVector[j], 2);
            }
            magnitude = Mathf.Sqrt(magnitude);
            //get the unit vector by dividing each compoenent by the magnitude (normalization)
            for (int j = 0; j < augmentVector.Count; j++)
            {
                augmentVector[j] /= magnitude;
                //augmentVector[j] *= p;
                //Debug.Log("Augment for " + i + " 's j" + augmentVector[j]);
            }

            //scale by the magnitude
            for (int j = 0; j < augmentVector.Count; j++)
            {
                augmentVector[j] *= p;
            }

            //combine the min vector and the augment vector
            List<float> finalVector = new List<float>();

            for (int j = 0; j < augmentVector.Count; j++)
            {
                finalVector.Add(augmentVector[j] + minVector[j]); 
            }

            newGeneration.Add(finalVector);

        }

        return newGeneration;
    }

    //Since I am taking the top half of the winning population
    //every mating process should produce 2 offspring, since
    // Pop = winners + losers
    // winners = pop /2
    //offspring from winners = pop/2/2, so in order to return
    //to the base population , each mating process should double
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


    //givent the vectors of two slimes, mate them together
    //with the given constraint p

    //for each attribute, find the midpoint
    //find the distance between the attributes
    //generate a random number between -distance/2 and distance/2 
    //add this number to the midpoint to get the basis for next attribute
    private List<float> Mate(List<float> x, List<float> y,float p)
    {
        List<float> offSpring = new List<float>();

        for (int i = 0; i < x.Count; i++)
        {
            float midPoint = (x[i] + y[i]) / 2;
            float distance = Mathf.Abs(x[i] - y[i]);
            float mutationValue = Random.Range(-1 * distance / 2, distance / 2);
            float result = midPoint + mutationValue;
            //cover my ass if we get negative values
            if (result <= 0) {
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

    //kill a slime after a fixed duration
    public IEnumerator KillSlime(Slime s)
    {
        winnerShowcase = true;
        yield return new WaitForSeconds(.2f);
        winnerShowcase = false;
        s.health = 0;

        //if slimes remain, pick 2 to battle
        if (slimeVectors.Count != 0)
        {
            Pick2Slimes();
        }
    }

    //when this gameobject ceases to exist write the data to resources/
    private void OnDestroy()
    {
        writeToFile();
    }

    private void writeToFile()
    {
        string fileName = "Assets/Simulation/Data/MyFile.txt";


        /*
        if (File.Exists(fileName))
        {
            Debug.Log(fileName + " already exists.");
            return;
        }
        */

        var sr = File.CreateText(fileName);
 
        for (int i = 0; i < slimeGenerations.Count; i++)
        {
            sr.WriteLine("Generation :" + i);
            for (int j = 0; j < slimeGenerations[0].Count; j++)
            {
                sr.WriteLine("");
                for (int k = 0; k < slimeGenerations[0][0].Count; k++)
                {
                    sr.Write(slimeGenerations[i][j][k]);
                    //don't write comma after last attribute
                    if (k < slimeGenerations[0][0].Count - 1) {
                        sr.Write(",");
                    }
                }
                sr.WriteLine("");
                
            }
        }



        sr.Close();
    }




}
