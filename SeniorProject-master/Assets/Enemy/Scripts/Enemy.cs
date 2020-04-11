using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    idle,
    walk,
    hitStun,
    attack,
    dead
}

//Knockback and damage taken by other colliders needs to be implemented
// Must figure out whether it will apply for all of be specific
public class Enemy : MonoBehaviour
{
    /* Do we want enemies to do the same amount of damage when you bump into them
     * as you would if you got hit by their attack?
     * Also, are enemies going to have projectile attacks?
     */

    public EnemyState currentState;
    public float health;
    public float baseAttack;
    public float movementSpeed;
    public float damage;
    public Animator animator;
    public Rigidbody2D rb;

    public void Knockback(Transform tr)
    {
        if (rb != null)
        {
            rb.isKinematic = false;
            currentState = EnemyState.hitStun;
            Vector2 difference = transform.position - tr.position;
            difference = difference.normalized * 5.0f;
            rb.AddForce(difference, ForceMode2D.Impulse);
        }
    }
    public IEnumerator KnockCo(Rigidbody2D rb)
    {
        if(rb != null)
        {
            yield return new WaitForSeconds(0.2f);
            rb.velocity = Vector2.zero;
            currentState = EnemyState.idle;
            rb.isKinematic = true;
        }
    }
}
