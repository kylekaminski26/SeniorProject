using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//State variables for Battler Constraints and animations
public enum BattlerState
{
    idle,
    walk,
    hitStun,
    attack,
    dead,
    dash
}

/*This class holds all of the common elements between the player and
 * the enemies This includes initial battle variables, states, and common routines
 *
 * All battlers will be instantiated with the following attributes
 * {maxHealth,maxStamina,baseAttack,movementSpeed,dexterity,vitality}
 *
 * Any battler entity must have the following
 * RigidBody2D
 * Animator
 * Sprite Collider2D (pushbox)
 * Sprite Renderer
 * Audio Source
 * A child object called Hurtbox (a sprite collider2D) with the Hurtbox script
 * 
 * Hitboxes and damage are programmed in the inheriting class and are not the
 * responsibility of the battler, however all implementation of battler
 * must attack under some constraints
 *
 * An attack corresponds to the temporary enabling of a hitbox,
 * when a battler comes in contact with a collider called hitbox it takes
 * damage from its owner.
 *  --Matt 4/25
 *
 * 
 */

public class Battler : MonoBehaviour
{

    public static float MAX_MAXHEALTH = 50F; 
    public static float MAX_MAXSTAMINA = 100F; 
    public static float MAX_BASEATTACK = 30F; 
    public static float MAX_MAXMOVEMENTSPEED = 10F; 
    public static float MAX_DEXTERITY = .1f; 
    public static float MAX_VITALITY = .1f; 

    public static float MIN_MAXHEALTH = 15f;
    public static float MIN_MAXSTAMINA = 50f;
    public static float MIN_BASEATTACK = 3f;
    public static float MIN_MAXMOVEMENTSPEED = 2f;
    public static float MIN_DEXTERITY = .05f;
    public static float MIN_VITALITY = .0001f;



    //Battling Attributes 
    public float maxHealth;
    public float maxStamina;
    public float baseAttack;
    public float movementSpeed;
    public float dexterity; //percentage of max stamina regenerated per frame (effectively controls attack rate) --> [0,1]
    public float vitality; //percentage of max health regenerated per frame --> [0,1]

    //State attributes
    public float stamina;
    public float health;

    //battler control variables
    public float lastAttackTime;        //need this be here --Matt 4/25?
    public BattlerState currentState;

    //movement direction
    public Vector3 movementDirection;
    public Vector3 lastTransformPosition;

    //prefab component references
    public Animator animator;
    public Rigidbody2D rb;
    public AudioSource[] aud;
    public SpriteRenderer sr;


    //public GameObject[] effects; Not utilized ... --Matt 4/25

    /*
     *  Initialize the battler with default values
     *
     *
     */
    protected void Awake()
    {
        maxHealth = health = MIN_MAXHEALTH;
        maxStamina = stamina = MIN_MAXSTAMINA;
        baseAttack = MIN_BASEATTACK;
        movementSpeed = MIN_MAXMOVEMENTSPEED;
        dexterity = MIN_DEXTERITY;
        vitality = MIN_VITALITY;

        currentState = BattlerState.idle;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        aud = GetComponentsInChildren<AudioSource>();
        sr = GetComponent<SpriteRenderer>();

        lastAttackTime = -1;

        //movement direction (Regardless of implementation of movement)
        movementDirection = new Vector3(0,0,0);
        lastTransformPosition = transform.position;

    }


    
    protected void Update()
    {
        if (health <= 0 && currentState != BattlerState.dead)
        {
            currentState = BattlerState.dead;
            Die();
        }

        //stamina regen
        if (stamina < maxStamina)
        {
            StaminaRegen();
        }

        //health regen
        if (health < maxHealth)
        {
            HealthRegen();
        }



    }

    protected void FixedUpdate()
    {
      
        //update movement Direction
        movementDirection = transform.position - lastTransformPosition;
        lastTransformPosition = transform.position;

        //Need this to animate when it is moving
        ChangeAnim(movementDirection);
        
    }


    //the hurtbox was collided with (this is called from a child's hurtbox script)
    public virtual void OnHurtboxCollision(Collider2D collision)
    {
        //check if collider is of tag hitbox
  
        if (collision.gameObject.CompareTag("Hitbox"))
        {
 

            //get the battler associated with the hit
            Battler attacker = (Battler)collision.transform.parent.gameObject.GetComponent<Battler>();
            if (attacker.currentState != BattlerState.dead && attacker !=  this) { //you cant attack yourself
                TakeDamage(attacker.GetDamage());
                //start knockback (these are from the parent)
                if (currentState != BattlerState.dead)
                {
                    StartCoroutine(KnockCo(attacker.transform,attacker));
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
    void StaminaRegen()
    {
        stamina += (dexterity * maxStamina) * Time.deltaTime;
    }

    //Regenerate the Battler's Health
    protected void HealthRegen()
    {
        health += (vitality * maxHealth) * Time.deltaTime;
    }

    //Co-routine for handling the termination of knockback
    public IEnumerator KnockCo(Transform tr,Battler b)
    {
        if (rb != null)
        {
            currentState = BattlerState.hitStun;
            Vector2 difference = transform.position - tr.position;
            difference = difference.normalized * 3f*((b.baseAttack/Battler.MAX_BASEATTACK)+1);
            rb.AddForce(difference, ForceMode2D.Impulse);
            yield return new WaitForSeconds(.4f);
            rb.velocity = Vector2.zero;
            //knockback coroutine may reset the switch to death state
            if (currentState != BattlerState.dead)
            {
                currentState = BattlerState.idle;
            }
        }
    }

    
    protected void Die()
    {
        rb.velocity = Vector2.zero; //Unsure whether neceessary
        animator.SetTrigger("Dead");
        rb.isKinematic = false; //will not move if hit during death animation
        AudioSource deathAudio = aud[0]; //0=death audio 
        deathAudio.Play();
        StartCoroutine(DeadCo());
    }

    
    private IEnumerator DeadCo()
    {
        yield return new WaitForSeconds(0.75f);
        this.gameObject.SetActive(false);

        //fresh 'sghetti
        //related to main game
        //so we can track how many kills the player has
        if (GameObject.Find("Player"))
        {
            PlayerControl player = GameObject.FindWithTag("Player").GetComponent<PlayerControl>();
            player.killCount += 1;
        }


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
    public void SetStats(float maxHealth, float maxStamina, float baseAttack, float movementSpeed, float dexterity,float vitality)
    {
        this.maxHealth = maxHealth;
        this.health = maxHealth;
        this.maxStamina = maxStamina;
        this.stamina = maxStamina;
        this.baseAttack = baseAttack;
        this.movementSpeed = movementSpeed;
        this.dexterity = dexterity;
        this.vitality = vitality;
    }


    public List<float> GetStats()
    {
        List<float> statList = new List<float>();
        statList.Add(maxHealth);
        statList.Add(maxStamina);
        statList.Add(baseAttack);
        statList.Add(movementSpeed);
        statList.Add(dexterity);
        statList.Add(vitality);

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

    
    public void PlayAudio(int index, float startTime, float endTime)
    {
        AudioSource audio = aud[index];
        audio.time = startTime;
        audio.Play();
        audio.SetScheduledEndTime(AudioSettings.dspTime + (endTime - startTime));
    }
    
}
