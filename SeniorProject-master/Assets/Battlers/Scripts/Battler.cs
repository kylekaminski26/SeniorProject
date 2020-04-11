using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum BattlerState
{
    idle,
    walk,
    hitStun,
    attack,
    dead
}

/*This class holds all of the common elements
 * between the player and the enemy
 * This includes initial battle variables,
 * states, and routines for recieving damage
 *
 * Any entity that fights will inherit from this parent class
 * All battler must have : a RigidBody2D, an Animator, a Sprite Collider, and Sprite Renderer
 *
 * The current Battler Model considers the following Variables {health, stamina, movement speed, dexterity }
 * Current Assumptions Stamina regeneration is not considered and is fixed
 *
 * Each Battler can have 1:M hitboxes 
 */

public class Battler : MonoBehaviour
{
    //Battling Attributes 
    public float maxHealth;
    public float maxStamina;
    public float baseAttack;
    public float movementSpeed;
    public float dexterity;
    //consider maxMovement Speed (buffs may allow speed beyond max for fixed time)

    //State attributes
    public float stamina;
    public float health;
    public float lastAttackTime;
    public BattlerState currentState;

    //prefab component references
    public Animator animator;
    public Rigidbody2D rb;

    /* When a MonoBehaviour is initialized,there is a sequence of function calls
     * that take place
     * 1. Constructor
     * 2. Awake
     * 3. Start
     *
     * In the game manager's start I initialize and set variables on the battlers,
     * so its important that there default settings are set in awake and then
     * overwritten in game manager's start
     */
    protected void Awake()
    {
        maxHealth = health = 10f;
        maxStamina = stamina = 6f;
        baseAttack = 4f;
        movementSpeed = 2f;
        dexterity = 1f; //lower is better (for now)

        currentState = BattlerState.idle;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        lastAttackTime = -1;
    }


    // Update is called once per frame
    void Update()
    {
        //did the battler die?
        if (health == 0)
        {
            Die();
        }
    }

    //determine if the Battler will take damage
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //check if collider is of tag hitbox
        if (collision.gameObject.tag == "Hitbox")
        {
            
            //get the battler associated with the hit
            Battler attacker = (Battler)collision.transform.parent.gameObject.GetComponent<Battler>();
   
            //is the attacker in an attack state?
            if (attacker.GetBattlerState() == BattlerState.attack)
            {
                //deal damage (from parent class)
                TakeDamage(attacker.GetDamage());
                //start knockback (these are from the parent)
                Knockback(attacker.transform);
                StartCoroutine(KnockCo());


            }
        }
    }


    //perform knockback as a result of this Battler being attacked by another
    public void Knockback(Transform tr)
    {
        if (rb != null)
        {
            rb.isKinematic = false;
            currentState = BattlerState.hitStun;
            Vector2 difference = transform.position - tr.position;
            difference = difference.normalized * 5.0f;
            rb.AddForce(difference, ForceMode2D.Impulse);
        }
    }

    //Co-routine for handling the termination of knockback
    public IEnumerator KnockCo()
    {
        if (rb != null)
        {
            yield return new WaitForSeconds(0.2f);
            rb.velocity = Vector2.zero;
            currentState = BattlerState.idle;
            rb.isKinematic = true;
        }
    }

    //All battlers can die!
    public void Die()
    {
        //currentState = BattlerState.dead;
        //rb.velocity = Vector2.zero;
        //animator.SetTrigger("Dead");
        Destroy(this.gameObject, 0f);
    }

    //All Battlers can take damage
    public void TakeDamage(float dmg)
    {
        health -= dmg;
        if (health < 0)
        {
            health = 0;
        }
    }

    //How much damage (All things considered this battler deals
    public float GetDamage()
    {
        return baseAttack;
    }

    //get the battler's state
    public BattlerState GetBattlerState()
    {
        return currentState;
    }

    public void SetStats(float maxHealth, float maxStamina, float baseAttack, float movementSpeed, float dexterity)
    {
        this.maxHealth = maxHealth;
        this.health = maxHealth;
        this.maxStamina = maxStamina;
        this.stamina = maxStamina;
        this.baseAttack = baseAttack;
        this.movementSpeed = movementSpeed;
        this.dexterity = dexterity;
    }


    public List<float> GetStats()
    {
        List<float> statList = new List<float>();
        statList.Add(maxHealth);
        statList.Add(maxStamina);
        statList.Add(baseAttack);
        statList.Add(movementSpeed);
        statList.Add(dexterity);

        return statList;
    }

    public string PrintStats()
    {
        return "" + maxHealth + ":" + maxStamina + ":" + baseAttack + ":" + movementSpeed + ":" + dexterity;
    }


}
