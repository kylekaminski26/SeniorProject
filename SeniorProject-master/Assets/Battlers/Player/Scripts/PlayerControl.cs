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

   

    /* Variables concerning the ranged attack functions
     * These variables will be adjusted by weapons/level system.
     */
    private float projectileSpeed;
    private float fireRate;
    private float projectileDamage;
    //Point at which projectiles are fire from
    private Transform firePoint;

    /* Variables concerning movement
     * For sprint/dash, we may only have one in the final project.
     * Also, these may be hard coded, since we do not have plans
     * on having these values be modified during the game.
     * 
     */
   
    private Vector2 movement;
    private float dashTime;
    private float dashMultiplier;

    private float attackRate;

    public int killCount = 0;


    void Start()
    {
        currentState = BattlerState.idle;
        firePoint = GetComponent<Transform>(); //Center of player

        maxHealth = health = 1000f;
        maxStamina = stamina = 1000f;
        dexterity = .05f;
        vitality = .005f;

        projectileSpeed = 10f;
        projectileDamage = 1f;
        fireRate = 0.375f;
        timeUntilNextFire = 0.0f;

        attackRate = 0.375f;
        lastAttackTime = 0.0f;
        baseAttack = 8f;

        movementSpeed = 5.0f;
        dashTime = 0.05f;
        timeUntilNextDash = 0.0f;
        dashMultiplier = 3f;
        movement = Vector2.zero;

        //Defaults for the animator once the game starts
        animator.SetFloat("Horizontal", 0.0f);
        animator.SetFloat("Vertical", -1.0f);
    }

    void Update()
    {
        base.Update();

        // Tried as a void function. Was throwing errors. IDK why. If you can do it better, by all means.
        lastAttackTime = UpdateTimers(lastAttackTime);
        timeUntilNextFire = UpdateTimers(timeUntilNextFire);
        timeUntilNextDash = UpdateTimers(timeUntilNextDash);

        if (Input.GetButtonDown("Fire1"))
        {
            //Debug.Log("Left Click");
        }

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
    }

    void FixedUpdate()
    {

        base.FixedUpdate();

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
        rb.MovePosition(rb.position + movement * movementSpeed * Time.fixedDeltaTime);

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

    //this is for shooting ..
    Vector3 ChooseDirection()
    // Used to rotate sprites according to direction of movement
    // Used for certain effects and instantiating projectiles
    // It is to be assumed that sprites are rotated facing north.
    {
        float temp = Mathf.Atan2(animator.GetFloat("Vertical"), animator.GetFloat("Horizontal")) * Mathf.Rad2Deg;
        return new Vector3(0, 0, temp - 90);
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

    /*
    void RangedAttack()
    {
        // Note: the float value represents the time of the animation. 
         / Hopefully it will be replaced rather than hard coded.
         //
        timeUntilNextFire = fireRate;
        animator.SetTrigger("Shooting");
        currentState = BattlerState.attack;
        Invoke("Shoot", 0.375f);
    }
    */

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

    //Overwritten in Player for enemy heuristics
    //the hurtbox was collided with (this is called from a child's hurtbox script)
    public override void OnHurtboxCollision(Collider2D collision)
    {
        //check if collider is of tag hitbox

        if (collision.gameObject.CompareTag("Hitbox"))
        {
            //get the battler associated with the hit
            Battler attacker = (Battler)collision.transform.parent.gameObject.GetComponent<Battler>();
            if (attacker.currentState != BattlerState.dead)
            {
                TakeDamage(attacker.GetDamage());
                //start knockback (these are from the parent)
                if (currentState != BattlerState.dead)
                {
                    StartCoroutine(KnockCo(attacker.transform));
                }

                //if I was attacked by an enemy then increment their damage dealt to player
                //by there baseAttack
                Debug.Log("eyyyy");
                if (attacker as Enemy != null)
                {
                    ((Enemy)attacker).incrementDamageDealt();
                }

            }

         


        }
    }


}
