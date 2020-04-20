using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/* Effects Map
 * 0 - Hit Effect 1
 * 1 - Heal Effect 1
 * 2 - Dash Effect
 * 3 .....
 */

/* Audio Map
 * 
 */

public class PlayerControl : Battler
{
    //Public prefabs needed for instantiaiting game objects
    public GameObject projectilePrefab;
    public GameObject dashEffectPrefab;

    /* Inherited by Battler
    public GameObject hitEffectPrefab;
    public GameObject healEffectPrefab;
    */

    /* Timers
     * In general, these are hard coded. We hope to be able to
     * get the values from the animator rather than hard coding it.
     * Will update once that function is created.
     * Note: Should probably create a function for instantiating
     * temproray assets (like effects) that take a gameobject
     * and time (float) to instantiate then destroy.
     * 
     * Inherited by Battler:
     *  private float timeUntilNextAttack;
     */
    private float timeUntilNextFire;
    private float timeUntilNextDash;

    //Player attributes
    /* These are inherited from Battler
     * 
    public float health;
    public float maxHealth;
    public float stamina;
    public float maxStamina;
    public float dexterity;
     */


    //Player components
    /* Inherited from Battler
    private Animator animator; 
    private Rigidbody2D rb;
    private AudioSource[] aud;
    */

    //Point at which projectiles are fire from
    private Transform firePoint;


    /* Variables concerning the ranged attack functions
     * These variables will be adjusted by weapons/level system.
     */
    private float projectileSpeed;
    private float fireRate;
    private float projectileDamage;

    /* Variables concerning movement
     * For sprint/dash, we may only have one in the final project.
     * Also, these may be hard coded, since we do not have plans
     * on having these values be modified during the game.
     * 
     */
    //private float movementSpeed; Inherited by Battler
    private Vector2 movement;
    private bool isTired;
    private float sprintMultiplier;
    private float dashTime;
    private float dashMultiplier;

    /* Varaibles concerning melee attacks.
     * Inherited from Battler
       private float baseAttack;
     */
    private float attackRate;


    void Start()
    {
        currentState = BattlerState.idle;
        firePoint = GetComponent<Transform>(); //Center of player
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        aud = GetComponentsInChildren<AudioSource>();

        maxHealth = health = 20f;
        maxStamina = stamina = 100f;
        dexterity = 1f;

        projectileSpeed = 10f;
        projectileDamage = 1f;
        fireRate = 0.375f;
        timeUntilNextFire = 0.0f;

        attackRate = 0.375f;
        lastAttackTime = 0.0f;
        baseAttack = 1f;

        movementSpeed = 5.0f;
        sprintMultiplier = 1.5f;
        dashTime = 0.05f;
        timeUntilNextDash = 0.0f;
        dashMultiplier = 3f;
        movement = Vector2.zero;
        isTired = false;

        //Defaults for the animator once the game starts
        animator.SetFloat("Horizontal", 0.0f);
        animator.SetFloat("Vertical", -1.0f);
    }

    void Update()
    {
        // Tried as a void function. Was throwing errors. IDK why. If you can do it better, by all means.
        lastAttackTime = UpdateTimers(lastAttackTime);
        timeUntilNextFire = UpdateTimers(timeUntilNextFire);
        timeUntilNextDash = UpdateTimers(timeUntilNextDash);

        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Left Click");
        }

        if (health <= 0)
        {
            currentState = BattlerState.dead;
            //Die();
        }

        if (stamina < 100f)
        {
            StaminaRegen();
        }

        if (isTired && stamina > 30f)
            isTired = false;

        if ((Input.GetButtonDown("Fire1") && (lastAttackTime == 0f && stamina > 30f))
            && (currentState != BattlerState.hitStun || currentState != BattlerState.dead))
        {
            MeleeAttack();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && stamina >= 30f && timeUntilNextDash == 0f) //Will change from fire2 to a dash button
        {
            InitiateDash();
        }

        //Will change the way this timer works for consistency
        /*
        if ((Input.GetButtonDown("Fire2") && (timeUntilNextFire == 0f)) 
            && (currentState != BattlerState.hitStun || currentState != BattlerState.dead))
        {
            RangedAttack();
        }
        */

        /* To answer Matt's question: When the sprint button is pressed,
         * we want the character to move faster, both the position and
         * the animation. Once it is pressed, the speed at which the animator
         * runs will be increased. However, the else statement is needed to 
         * change the animator speed back if the sprint button is not being
         * pressed.
        
        if ((Input.GetKey(KeyCode.LeftShift) && !isTired) && currentState == BattlerState.walk)
        {
            animator.speed = sprintMultiplier;
        }
        else
        {
            animator.speed = 1.0f;
        }
        */
    }

    void FixedUpdate()
    {
        if (currentState == BattlerState.dash)
        {
            if (timeUntilNextDash == 0f)
                currentState = BattlerState.idle;
            rb.MovePosition(rb.position + movement * (movementSpeed * dashMultiplier) * Time.fixedDeltaTime);
        }
        else if (currentState == BattlerState.walk || currentState == BattlerState.idle)
        {
            Move();
        }

    }

    void Move()
    {
        //Added ChangeAnim, however, it turns out that this code
        // is really messy and unorganized. If you are reading this,
        // this is not fixed yet.

        movement = Vector2.zero;
        // Input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Adds animations
        if (movement != Vector2.zero)
        {
            ChangeAnim(movement);
        }
        else
        {
            animator.SetBool("Moving", false);
            currentState = BattlerState.idle;
        }

        // Movement
        movement.Normalize();
        /* Probably will replace sprinting with dashing
         
        else if ((Input.GetKey(KeyCode.LeftShift) && !isTired) && currentState == PlayerState.walk)
        {
            rb.MovePosition(rb.position + movement * (movementSpeed * sprintMultiplier) * Time.fixedDeltaTime);
            DecreaseStamina(40f, true);
            if(stamina == 0f)
            {
                isTired = true;
            }
        }
        */
        //else
        //{
        rb.MovePosition(rb.position + movement * movementSpeed * Time.fixedDeltaTime);
        //}
    }

    void Shoot()
    {
        Quaternion projectileRotation = Quaternion.Euler(ChooseDirection());
        Vector2 temp = new Vector2(animator.GetFloat("Horizontal"), animator.GetFloat("Vertical"));

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, projectileRotation);
        projectile.GetComponent<Projectile>().SetDamage(projectileDamage);
        Rigidbody2D projectileRB = projectile.GetComponent<Rigidbody2D>();
        projectileRB.velocity = temp.normalized * projectileSpeed;

        currentState = BattlerState.walk;
        Destroy(projectile, 1.0f);
    }

    Vector3 ChooseDirection()
    // Used to rotate sprites according to direction of movement
    // Used for certain effects and instantiating projectiles
    // It is to be assumed that sprites are rotated facing north.
    {
        float temp = Mathf.Atan2(animator.GetFloat("Vertical"), animator.GetFloat("Horizontal")) * Mathf.Rad2Deg;
        return new Vector3(0, 0, temp - 90);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.gameObject.CompareTag("Enemy") && currentState != BattlerState.hitStun)
        {
            //This will be changed to where it gets the damage from the
            // hitbox rather than from the baseAttack of the enemy.
            if (health - other.gameObject.GetComponentInParent<Battler>().baseAttack <= 0)
            {

                TakeDamage(other.gameObject.GetComponentInParent<Battler>().baseAttack);
                Die();
            }
            else
            {
                TakeDamage(other.gameObject.GetComponentInParent<Battler>().baseAttack);
            }
             //* Need to use component in parent to utilize this script. Otherwise
             //* we need to add a knockback/damage script to the hitboxes of the enemy entity as well
             //* rather than having it all on the single enemy object.
             
            if (currentState != BattlerState.dead)
            {
                //GameObject combatEffect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
                //Destroy(combatEffect, 0.35f);
                Knockback(other.transform);
                StartCoroutine(KnockCo(rb));
            }
        }
        
        /*
        if (other.gameObject.CompareTag("Potion"))
        {
            GameObject healEffect = Instantiate(healEffectPrefab, transform.position, Quaternion.identity);
            Heal(other.gameObject.GetComponentInParent<Consumable>().hpRestore);
            Destroy(healEffect, 0.5f);
        }
        */
    }

    //Double check and make sure that isKinematic does not break
    //the playercontrol script if inherited from Battler
    new void Knockback(Transform tr)
    {
        if (rb != null)
        {
            currentState = BattlerState.hitStun;
            Vector2 difference = transform.position - tr.position;
            difference = difference.normalized * 4.0f;
            rb.AddForce(difference, ForceMode2D.Impulse);
        }
    }
    private IEnumerator KnockCo(Rigidbody2D rb)
    {
        if (rb != null)
        {
            yield return new WaitForSeconds(0.4f);
            rb.velocity = Vector2.zero;
            currentState = BattlerState.idle;
        }
    }

    void Heal(float hp)
    {
        health += hp;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    new void Die()
    {
        rb.velocity = Vector2.zero; //Unsure whether neceessary
        animator.SetTrigger("Dead");
        BoxCollider2D[] bc2d = GetComponents<BoxCollider2D>();
        AudioSource deathAudio = aud[0];
        deathAudio.Play();
        foreach (BoxCollider2D col in bc2d)
        {
            col.enabled = false;
        }
        StartCoroutine(DeadCo());
    }

    private IEnumerator DeadCo()
    {
        yield return new WaitForSeconds(0.75f);
        SceneManager.LoadScene("Menu");
        this.gameObject.SetActive(false);
    }

    void StaminaRegen()
    {
        stamina += (10f * dexterity) * Time.deltaTime;
    }
    void DecreaseStamina(float num, bool timedDecrease)
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
    new void PlayAudio(int index, float startTime, float endTime)
    {
        AudioSource audio = aud[index];
        audio.time = startTime;
        audio.Play();
        audio.SetScheduledEndTime(AudioSettings.dspTime + (endTime - startTime));
    }

    void MeleeAttack()
    {
        lastAttackTime = attackRate;
        currentState = BattlerState.attack;
        DecreaseStamina(30f, false);
        animator.SetTrigger("Attacking");
        PlayAudio(1, 0.13f, 0.4f);
        currentState = BattlerState.idle;
    }

    void RangedAttack()
    {
        /* Note: the float value represents the time of the animation. 
         * Hopefully it will be replaced rather than hard coded.
         */
        timeUntilNextFire = fireRate;
        animator.SetTrigger("Shooting");
        currentState = BattlerState.attack;
        Invoke("Shoot", 0.375f);
    }

    float UpdateTimers(float timer)
    {
        if (timer > 0.0f)
        {
            timer -= Time.deltaTime;
        }
        else if (timer <= 0.0f)
        {
            timer = 0.0f;
        }
        return timer;
    }

    void InitiateDash()
    {
        if (movement != Vector2.zero)
        {
            Quaternion dashEffectRotation = Quaternion.Euler(ChooseDirection());
            GameObject dashEffect = Instantiate(dashEffectPrefab, transform.position, dashEffectRotation);
            Destroy(dashEffect, 0.229f);
            DecreaseStamina(30f, false);
        }
        currentState = BattlerState.dash;
        timeUntilNextDash = dashTime;
    }
}
