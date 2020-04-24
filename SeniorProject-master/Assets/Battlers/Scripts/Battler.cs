using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//State variables for Battler Constraints and animations
//Hitstun and attack are used in enemy AI
public enum BattlerState
{
    idle,
    walk,
    hitStun,
    attack,
    dead,
    dash
}

/*This class holds all of the common elements
 * between the player and the enemy
 * This includes initial battle variables,
 * states, and routines for recieving damage
 *
 * Any entity that fights will inherit from this parent class
 * All battler must have : a RigidBody2D, an Animator, a Sprite Collider, and Sprite Renderer
 * and a child object called Hurtbox which contains a Hurtbox script
 *
 * The current Battler Model considers the following Variables {health, stamina, movement speed, dexterity }
 * Current Assumptions Stamina regeneration is not considered and is fixed
 *
 * Each Battler can have 1:M hitboxes
 * Each Battler Must have 1 BoxCollider2D Hurtbox
 * 
 */

public class Battler : MonoBehaviour
{
    //Battling Attributes 
    public float maxHealth;
    public float maxStamina;
    public float baseAttack;
    public float movementSpeed;
    public float dexterity; //rate of stamina regen (effectively controls attack rate) --> [0,1]
    public float vitality; //rate of health regen
    //consider maxMovement Speed (buffs may allow speed beyond max for fixed time)

    //State attributes
    public float stamina;
    public float health;
    public float lastAttackTime;
    public BattlerState currentState;

    //prefab component references
    public Animator animator;
    public Rigidbody2D rb;
    public AudioSource[] aud;
    public GameObject[] effects;

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
        maxHealth = health = 25f;
        maxStamina = stamina = 25f;
        baseAttack = 15f;
        movementSpeed = 2f;
        dexterity = .1f;
        vitality = .1f;

        currentState = BattlerState.idle;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        aud = GetComponentsInChildren<AudioSource>();

        lastAttackTime = -1;
    }


    // Update is called once per frame
    //Stamina Regen? (Dex)
    //Health Regen? (Vitality)
    protected void Update()
    {
        if (health <= 0)//&& currentState != BattlerState.dead)
        {
            currentState = BattlerState.dead;
            Die();
        }

        //stamina regen
        if (stamina < maxStamina)
        {
            StaminaRegen(dexterity);
        }

        //health regen
        if (health < maxHealth)
        {
            HealthRegen(vitality);
        }


    }

    //the hurtbox was collided with (this is called from a child's hurtbox script)
    public void OnHurtboxCollision(Collider2D collision)
    {
        //check if collider is of tag hitbox
  
        if (collision.gameObject.CompareTag("Hitbox"))
        {
            //get the battler associated with the hit
            Battler attacker = (Battler)collision.transform.parent.gameObject.GetComponent<Battler>();
            if (attacker.currentState != BattlerState.dead) { 

                TakeDamage(attacker.GetDamage());
                //start knockback (these are from the parent)
                if (currentState != BattlerState.dead)
                {
                    Knockback(attacker.transform);
                    StartCoroutine(KnockCo());
                }
             }

        }
    }

    protected void DecreaseStamina(float num, bool timedDecrease)
    // timeDecrease: To remove stamina over time. This decreases the total stamina
    // by delta time to even out the decrease. Set true if you need this.
    {
        if (timedDecrease)
        {
            stamina -= num * Time.deltaTime;

        }
        else
        {
            stamina -= num;

        }

        if (stamina < 0f)
        {
            stamina = 0f;
        }
    }

    //Regenerate the Battler's stamina
    //Based on a percentage of there dexterity
    //This is should be called every frame in update or fixedUpdate
    void StaminaRegen(float dexPercentage)
    {
        stamina += (dexPercentage * maxStamina) * Time.deltaTime;
    }

    //Regenerate the Battler's Health
    void HealthRegen(float healthPercentage)
    {
        health += (healthPercentage * maxHealth) * Time.deltaTime;
    }


    //perform knockback as a result of this Battler being attacked by another
    public void Knockback(Transform tr)
    {
        if (rb != null)
        {
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
            yield return new WaitForSeconds(0.4f);
            rb.velocity = Vector2.zero;
            currentState = BattlerState.idle;
        }
    }

    //Die override Okay
    void Die()
    {
        rb.velocity = Vector2.zero; //Unsure whether neceessary
        animator.SetTrigger("Dead");
        rb.isKinematic = false; //will not move if hit during death animation
        AudioSource deathAudio = aud[0]; //0=death audio 
        deathAudio.Play();
        StartCoroutine(DeadCo());
    }

    //override okay
    private IEnumerator DeadCo()
    {
        yield return new WaitForSeconds(0.75f);
        this.gameObject.SetActive(false);

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

    public void ChangeState(BattlerState state)
    {
        currentState = state;
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

    public void ChangeAnim(Vector2 direction)
    {
        direction = direction.normalized;

        if (direction != Vector2.zero)
        {
            animator.SetFloat("Horizontal", direction.x);
            animator.SetFloat("Vertical", direction.y);
            animator.SetBool("Moving", true);
        }
    }

    /* Is this ever used?
    public void MoveAndAnimate(Vector3 moveVector, bool toward)
    {
        float direction = 1f;
        if (!toward)
            direction = -1f;

        Vector3 newPos = Vector3.MoveTowards(transform.position, moveVector, direction * movementSpeed * Time.deltaTime);
        ChangeAnim(newPos - transform.position);
        rb.MovePosition(newPos);
    }
    */

    
    public void PlayAudio(int index, float startTime, float endTime)
    {
        AudioSource audio = aud[index];
        audio.time = startTime;
        audio.Play();
        audio.SetScheduledEndTime(AudioSettings.dspTime + (endTime - startTime));
    }
    
}
