using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public int health;      // Player's health
    public int stamina;     // Player's stamina
    public int armor;       // Player's armor
    public float strength;  // Player's strength
    public float mobility;  // Player's mobility

    int damageDelay = 0;    // Variable used to delay the damage taken by hazards

    // Start is called before the first frame update
    void Start()
    {
        health = 20; 
        stamina = 100;
        armor = 5;
        strength = 1f;
        mobility = 1f;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*  OnTriggerStay is called once per physics update and runs the code if a collider/rigidbody are in contact
    
        Currently used to test damage done by hazards

    void OnTriggerStay2D(Collider2D other)
    {
        if(other.tag == "Hazard")
        {
            damageDelay--;
            if(damageDelay <= 0)
            {
                health--;
                Debug.Log("Health:" + health);
                damageDelay = 80;
            }
        }
    }

    */
}
