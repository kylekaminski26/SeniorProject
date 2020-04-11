using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Slime : Enemy
{
    private GameObject target;
    private float angle;
    private float patrolTimer;
    private float attackDuration;
    private float timeUntilNextAttack;

    float timeUntilNextMove;
    float searchTime;
    Vector3 movementVector;

    void Start()
    {
        currentState = EnemyState.idle;
        targetVisible = false;
        target = GameObject.FindWithTag("Player");
        maxHealth = 3f;
        health = maxHealth;
        attackDamage = 2f;
        mobility = 0.5f;
        initialPos = transform.position;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        aud = GetComponentsInChildren<AudioSource>();

        patrolTimer = 0.0f;
        angle = 0f;
        timeUntilNextMove = 0f;
        searchTime = 0f;
        timeUntilNextAttack = 0.0f;
        attackDuration = 0.0f;
        movementVector = new Vector3(0, 0, transform.position.z);
    }

    void Update()
    {
        if(currentState == EnemyState.idle || currentState == EnemyState.patrol)
            patrolTimer += Time.deltaTime;
        Debug.DrawLine(transform.position, target.transform.position);
    }

    void FixedUpdate()
    {
        AI(currentState);
        /*In terms of fixed update, I need to change several of these if statements to else ifs so 
          certain else  statements do not run as a result of the enemy not being in a specific state rather than several states 

        if((health/maxHealth) <= 0.15)
        {
            Flee();
        }
        */
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(currentState != EnemyState.dead)
        {
            if (other.gameObject.CompareTag("player_attack") && currentState != EnemyState.hitStun)
            {
                health--;
                if (health <= 0)
                {
                    Die();
                }
                else
                {
                    Vector3 center = new Vector3(transform.position.x + 0.2f, transform.position.y - 0.6f); // Will need to have different layer for effects
                    GameObject effect = Instantiate(hitEffect, center, Quaternion.identity);
                    Destroy(effect, 0.35f);
                    PlayAudio(1, 0.0f, 0.280f);
                    Knockback(other.transform);
                    StartCoroutine(KnockCo(rb));
                }
            }

            if (other.gameObject.CompareTag("player_projectile"))
            {
                health--;
                if (health <= 0)
                {
                    Die();
                }
                else
                    PlayAudio(1, 0.0f, 0.280f);
            }
        }
    }

    void Die()
    {
        ChangeState(EnemyState.dead);
        rb.velocity = Vector2.zero;
        animator.SetTrigger("Dead");
        PlayAudio(0, 0.0f, 0.56f);
        Destroy(this.gameObject, 1.0f);
    }

    void Chase()
    {
        MoveAndAnimate(target.transform.position, true);
    }

    void Patrol(float i)
    {
        Vector3 temp = new Vector3(transform.position.x + 1 * Mathf.Cos(i), transform.position.y + 1 * Mathf.Sin(i), transform.position.z);
        MoveAndAnimate(temp, true);
    }

    void Search()
    {
        Vector2 direction = new Vector2((target.transform.position.x - transform.position.x), (target.transform.position.y - transform.position.y));
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, 8f);
        Debug.DrawRay(transform.position, direction);
        foreach(RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject.CompareTag("Wall"))
            {
                break;
            }

            if (hit.collider.gameObject.transform.parent != null)
            {
                if (GameObject.ReferenceEquals(hit.collider.gameObject.transform.parent.gameObject, target))
                {
                    float distance = Vector2.Distance(target.transform.position, transform.position);
                    if (distance < 6f)
                    {
                        ChangeState(EnemyState.attacking);
                    }
                    else
                        Chase();
                    searchTime = 0f;
                }
                else if (searchTime < 5f)
                {
                    timeUntilNextMove -= (Time.deltaTime / hits.Length);
                    searchTime += (Time.deltaTime/hits.Length);
                    if (timeUntilNextMove < 0f)
                    {
                        movementVector = new Vector3(Random.Range(-5.0f, 5.0f) + transform.position.x, Random.Range(-10.0f, 10.0f) + transform.position.y, transform.position.z);
                        timeUntilNextMove = 1f;
                    }
                    MoveAndAnimate(movementVector, true);
                }
                else
                {
                    ChangeState(EnemyState.returning);
                }
            }
        }  
    }

    void Flee()
    {
        ChangeState(EnemyState.returning);
        MoveAndAnimate(target.transform.position, false);
    }

    void ReturnToOrigin()
    {
        ChangeState(EnemyState.returning);
        MoveAndAnimate(initialPos, true);
    }

    void AI(EnemyState state)
    {
        float distance  = Vector2.Distance(target.transform.position, transform.position);

        switch (state)
        {
            case EnemyState.idle:
                animator.SetBool("Moving", false);
                if (targetVisible)
                {
                    ChangeState(EnemyState.attacking);
                }
                if (patrolTimer > 5.0f)
                {
                    patrolTimer = 0.0f;
                    ChangeState(EnemyState.patrol);
                }
                break;
            case EnemyState.patrol:
                angle = angle + Time.deltaTime;
                Patrol(angle);
                if (targetVisible)
                {
                    ChangeState(EnemyState.attacking);
                }
                if (patrolTimer > 4.0f)
                {
                    patrolTimer = 0.0f;
                    ChangeState(EnemyState.idle);
                }
                break;
            case EnemyState.attacking:
                if (targetVisible && distance < 5f)
                {
                    SpinAttack();
                }
                else if (targetVisible)
                {
                    Chase();
                }
                else if (!targetVisible && timeUntilNextAttack < 3f)
                {
                    timeUntilNextMove = 0f;
                    searchTime = 0f;
                    ChangeState(EnemyState.search);
                }
                break;
            case EnemyState.search:
                Search();
                break;
            case EnemyState.returning:
                if(transform.position == initialPos)
                {
                    ChangeState(EnemyState.idle);
                }
                else if (targetVisible)
                {
                    ChangeState(EnemyState.attacking);
                }
                else
                {
                    ReturnToOrigin();
                }
                break;
        }
    }

    void IsHealthLow()
    {
        if(health/maxHealth == 0.15)
        {
            ChangeState(EnemyState.fleeing);
        }
        else
        {
            return;
        }
    }

    void SpinAttack()
    {
        timeUntilNextAttack -= Time.deltaTime;
        if (timeUntilNextAttack < 0f)
        {
            animator.SetTrigger("Spin Attack");
            movementVector = target.transform.position;
            AnimatorClipInfo[] info = animator.GetCurrentAnimatorClipInfo(0);
            attackDuration = 1.5f; //info[0].clip.length; constantly returning 0.5 when the animation is 1.5 seconds
            timeUntilNextAttack = 3f + attackDuration;
        }
        else if (timeUntilNextAttack > 3f)
        {
            Vector3 newPos = Vector3.MoveTowards(transform.position, movementVector, 5f * Time.deltaTime);
            changeAnim(newPos - transform.position);
            rb.MovePosition(newPos);
        }
        else if (timeUntilNextAttack < 3f)
        {
            Chase();
        }
        else
            return;

    }
}
