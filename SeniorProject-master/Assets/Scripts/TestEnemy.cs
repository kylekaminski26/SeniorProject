using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : Enemy
{   
    void Start()
    {
        health = 10;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //TO-DO: Add hitstun component. We need enemy behavior for this tho.
        if (other.gameObject.CompareTag("player_attack"))
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
                animator.SetTrigger("Hurt");
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
            {
                animator.SetTrigger("Hurt");
            }
        }

    }
    void Die()
    {
        rb.velocity = Vector2.zero;
        animator.SetTrigger("Dead");
        Destroy(this.gameObject, 1.0f);
    }
}
