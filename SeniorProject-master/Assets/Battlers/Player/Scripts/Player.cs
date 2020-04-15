using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Player : Battler
{
    public GameObject projectilePrefab;
    public GameObject hitEffectPrefab;
    public GameObject healEffectPrefab;

    private Collider2D Hitbox; // should be generalized in parent 

    //Player components
    private Transform firePoint; //Intention is to fire from the center of the player
    //private Animator animator; in battler 
    //private Rigidbody2D rb; in battler
    private AudioSource[] aud;


    // Variables concerning the ranged attack functions
    private float projectileSpeed;
    private float fireRate;
    private float timeUntilNextFire;
    private float projectileDamage;

    // Variables concerning movement
    //private float movementSpeed; //in battler parent
    private Vector2 movement;
    private float sprintMultiplier;
    private bool isTired;

    private float timeUntilNextAttack;// should be in battler
    private float attackRate; //should be in battler


    void Start()
    {
        
        firePoint = GetComponent<Transform>();
        aud = GetComponentsInChildren<AudioSource>();

        Hitbox = GetComponentInChildren<CircleCollider2D>();
        Hitbox.enabled = false;

        //below are set in battler 

        //animator = GetComponent<Animator>();
        //rb = GetComponent<Rigidbody2D>();
        //currentState = BattlerState.idle;


        maxHealth = 20f;
        maxStamina = 100f;
        health = maxHealth;
        stamina = maxStamina;
        //armor = 5;
        //strength = 5;

        projectileSpeed = 10f;
        projectileDamage = 1f;
        fireRate = 0.375f;
        timeUntilNextFire = 0.0f;

        attackRate = 0.375f;
        timeUntilNextAttack = 0.0f;

        movementSpeed = 5.0f;
        sprintMultiplier = 1.5f;
        movement = Vector2.zero;
        isTired = false;

        //Player looks and attacks downwards at start of game
        animator.SetFloat("Horizontal", 0.0f);
        animator.SetFloat("Vertical", -1.0f); 
    }

    void Update()
    {

        //should both probably be moved to battler (stamina refresh and isTired Check
        if(stamina < 100f)
        {
            stamina += 10f * Time.deltaTime;
        }
        if (isTired && stamina > 30f)
            isTired = false;

        /////////////
        ///Melee
        /////////////
        if ((Input.GetButtonDown("Fire1") && (Time.time > timeUntilNextAttack && stamina > 30f)) 
            && (currentState != BattlerState.hitStun || currentState != BattlerState.dead))
        {
            timeUntilNextAttack = Time.time + attackRate;
            currentState = BattlerState.attack;
            DecreaseStamina(30f, false);
            animator.SetTrigger("Attacking");
            PlayAudio(1, 0.13f, 0.4f);
        }

        ////////////
        //Sprinting
        ///////////
        if ((Input.GetKey(KeyCode.LeftShift) && !isTired) && currentState == BattlerState.walk)
        {
            animator.speed = sprintMultiplier;
        }

        /* Ranged Attack disabled ... Not consistent with hitbox hurtbox scheme atm
       if ((Input.GetButtonDown("Fire2") && Time.time > timeUntilNextFire) 
           && (currentState != PlayerState.hitStun || currentState != PlayerState.dead))
       {
           timeUntilNextFire = Time.time + fireRate;
           animator.SetTrigger("Shooting");
           currentState = PlayerState.attack;
           Invoke("Shoot", 0.375f);
       }
       */

        else //why? what does this do?
        {
            animator.speed = 1.0f;
        }
    }

    void FixedUpdate()
    {
        //State management

        if (currentState == BattlerState.walk || currentState == BattlerState.idle)
        {
            Move();
        }

        //toggle hitbox collider based on state
        if (currentState == BattlerState.attack)
        {
            //enable hitbox
            StartCoroutine(AttackCo());
        }
    }

    void Move()
    {
        movement = Vector2.zero;
        // Input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Adds animations
        if (movement != Vector2.zero)
        {
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
            animator.SetBool("Moving", true);
            currentState = BattlerState.walk;
        }
        else
        {
            animator.SetBool("Moving", false);
            currentState = BattlerState.idle;
        }

        // Movement
        movement.Normalize();
        if ((Input.GetKey(KeyCode.LeftShift) && !isTired) && currentState == BattlerState.walk)
        {
            rb.MovePosition(rb.position + movement * (movementSpeed * sprintMultiplier) * Time.fixedDeltaTime);
            DecreaseStamina(40f, true);
            if(stamina == 0f)
            {
                isTired = true;
            }
        }
        else
        {
            rb.MovePosition(rb.position + movement * movementSpeed * Time.fixedDeltaTime);
        }
    }

    void Shoot()
    {
        Quaternion projectileRotation = Quaternion.Euler(ChooseArrowDirection());
        Vector2 temp = new Vector2(animator.GetFloat("Horizontal"), animator.GetFloat("Vertical"));

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, projectileRotation);
        projectile.GetComponent<Projectile>().SetDamage(projectileDamage);
        Rigidbody2D projectileRB = projectile.GetComponent<Rigidbody2D>();
        projectileRB.velocity = temp.normalized * projectileSpeed;

        currentState = BattlerState.walk;
        Destroy(projectile, 1.0f);
    }

    Vector3 ChooseArrowDirection()
    {
        float temp = Mathf.Atan2(animator.GetFloat("Vertical"), animator.GetFloat("Horizontal")) * Mathf.Rad2Deg;
        return new Vector3(0, 0, temp - 90);
    }
   
    /* Move to Battler
    void Heal(float hp)
    {
        health += hp;
        if(health > maxHealth)
        {
            health = maxHealth;
        }
    }
    */

    void DecreaseStamina(float num, bool timedDecrease)
    {
        if(timedDecrease)
        {
            stamina -= num * Time.deltaTime;

        }
        else
        {
            stamina -= num;

        }

        if(stamina < 0f)
        {
            stamina = 0f;
        }
    }
    void PlayAudio(int index, float startTime, float endTime)
    {
        AudioSource audio = aud[index];
        audio.time = startTime;
        audio.Play();
        audio.SetScheduledEndTime(AudioSettings.dspTime + (endTime - startTime));
    }

    //Co-routine for handling the termination of Attack State
    public IEnumerator AttackCo()
    {
        if (rb != null)
        {
            Hitbox.enabled = true;
            yield return new WaitForSeconds(0.2f);
            Hitbox.enabled = false;
            currentState = BattlerState.idle;
        }
    }

}
