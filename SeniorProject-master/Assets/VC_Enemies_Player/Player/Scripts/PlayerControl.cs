using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlayerState
{
    idle,
    walk,
    attack,
    hitStun,
    dead
}


public class PlayerControl : MonoBehaviour
{
    public GameObject projectilePrefab;
    private PlayerState currentState;

    //Player attributes
    public float health;
    public float maxHealth;
    public float stamina;
    public float maxStamina;
    public GameObject hitEffectPrefab;
    public GameObject healEffectPrefab;

    private float armor;
    private float strength;

    //Player components
    private Transform firePoint; //Intention is to fire from the center of the player
    private Animator animator; 
    private Rigidbody2D rb;
    private AudioSource[] aud;


    // Variables concerning the ranged attack functions
    private float projectileSpeed;
    private float fireRate;
    private float timeUntilNextFire;
    private float projectileDamage;

    // Variables concerning movement
    private float movementSpeed;
    private Vector2 movement;
    private float sprintMultiplier;
    private bool isTired;

    private float timeUntilNextAttack;
    private float attackRate;
    private float attackDamage;


    void Start()
    {
        currentState = PlayerState.idle;
        firePoint = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        aud = GetComponentsInChildren<AudioSource>();

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
        attackDamage = 1f;

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
        if(stamina < 100f)
        {
            stamina += 10f * Time.deltaTime;
        }
        if (isTired && stamina > 30f)
            isTired = false;

        if ((Input.GetButtonDown("Fire1") && (Time.time > timeUntilNextAttack && stamina > 30f)) 
            && (currentState != PlayerState.hitStun || currentState != PlayerState.dead))
        {
            timeUntilNextAttack = Time.time + attackRate;
            currentState = PlayerState.attack;
            DecreaseStamina(30f, false);
            animator.SetTrigger("Attacking");
            PlayAudio(1, 0.13f, 0.4f);
            currentState = PlayerState.idle;
        }
        /*
        if ((Input.GetButtonDown("Fire2") && Time.time > timeUntilNextFire) 
            && (currentState != PlayerState.hitStun || currentState != PlayerState.dead))
        {
            timeUntilNextFire = Time.time + fireRate;
            animator.SetTrigger("Shooting");
            currentState = PlayerState.attack;
            Invoke("Shoot", 0.375f);
        }
        */

        if ((Input.GetKey(KeyCode.LeftShift) && !isTired) && currentState == PlayerState.walk)
        {
            animator.speed = sprintMultiplier;
        }
        else
        {
            animator.speed = 1.0f;
        }
    }

    void FixedUpdate()
    {
        if (currentState == PlayerState.walk || currentState == PlayerState.idle)
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
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
            animator.SetBool("Moving", true);
            currentState = PlayerState.walk;
        }
        else
        {
            animator.SetBool("Moving", false);
            currentState = PlayerState.idle;
        }

        // Movement
        movement.Normalize();
        if ((Input.GetKey(KeyCode.LeftShift) && !isTired) && currentState == PlayerState.walk)
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

        currentState = PlayerState.walk;
        Destroy(projectile, 1.0f);
    }

    Vector3 ChooseArrowDirection()
    {
        float temp = Mathf.Atan2(animator.GetFloat("Vertical"), animator.GetFloat("Horizontal")) * Mathf.Rad2Deg;
        return new Vector3(0, 0, temp - 90);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy") && currentState != PlayerState.hitStun)
        {
            currentState = PlayerState.hitStun;
            TakeDamage(other.gameObject.GetComponentInParent<Enemy>().attackDamage);
            /* Need to use component in parent to utilize this script. Otherwise
             * we need to add a knockback/damage script to the hitboxes of the enemy entity as well
             * rather than having it all on the single enemy object.
             */
            if(health == 0)
            {
                currentState = PlayerState.dead;
                Die();
            }
            else
            {
                GameObject combatEffect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
                Destroy(combatEffect, 0.35f);
                Knockback(other.transform);
                StartCoroutine(KnockCo(rb));
            }
        }
        if (other.gameObject.CompareTag("Potion"))
        {
            GameObject healEffect = Instantiate(healEffectPrefab, transform.position, Quaternion.identity);
            Destroy(healEffect, 0.5f);
        }
    }
    void Knockback(Transform tr)
    {
        if (rb != null)
        {
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
            currentState = PlayerState.walk;
        }
    }

    void TakeDamage(float dmg)
    {
        health -= dmg;
        if(health < 0)
        {
            health = 0;
        }
    }

    void Heal(float hp)
    {
        health += hp;
        if(health > maxHealth)
        {
            health = maxHealth;
        }
    }

    void Die()
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
        this.gameObject.SetActive(false);
    }

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

}
