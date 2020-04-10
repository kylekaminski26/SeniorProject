using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//TO-Dos: Need to ensure all states are being accounted for
// Need to fix code so it is more compact and portable
// Need to move some elements to the Enemy Class if they can be used elsewhere
public class Slime : Enemy
{
    private float chaseRadius;
    private float attackRadius;
    private Transform target;

    void Start()
    {
        currentState = EnemyState.idle;
        target = GameObject.FindWithTag("Player").transform;
        health = 20f;
        movementSpeed = 1f;
        chaseRadius = 5f;
        attackRadius = 1f;
        damage = 4f;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }
    
    void FixedUpdate()
    {
        CheckDistance();
        if(currentState == EnemyState.idle)
        {
            animator.SetBool("Moving", false);
        }
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
                    Knockback(other.transform);
                    StartCoroutine(KnockCo(rb));
                }
            }

            if (other.gameObject.CompareTag("player_projectile") && other.isTrigger)
            {
                health--;
                if (health <= 0)
                {
                    Die();
                }
            }
        }
    }

    void Die()
    {
        currentState = EnemyState.dead;
        rb.velocity = Vector2.zero;
        animator.SetTrigger("Dead");
        Destroy(this.gameObject, 1.0f);
    }

    void CheckDistance()
    {
        if (Vector3.Distance(target.position, transform.position) <= chaseRadius
            && Vector3.Distance(target.position, transform.position) > attackRadius)
        {
            if ((currentState == EnemyState.idle || currentState == EnemyState.walk)
                && (currentState != EnemyState.hitStun || currentState != EnemyState.dead))
            {
                Vector3 temp = Vector3.MoveTowards(transform.position, target.position, movementSpeed * Time.deltaTime);
                changeAnim(temp - transform.position);
                rb.MovePosition(temp);
                currentState = EnemyState.walk;
            }
        }
        else
        {
            currentState = EnemyState.idle;
        }
    }

    void changeAnim(Vector2 direction)
    {
        direction = direction.normalized;

        if (direction != Vector2.zero)
        {
            animator.SetFloat("Horizontal", direction.x);
            animator.SetFloat("Vertical", direction.y);
            animator.SetBool("Moving", true);
        }
    }
}
