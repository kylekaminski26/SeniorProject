using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    idle,
    patrol,
    returning,
    fleeing,
    search,
    walk,
    hitStun,
    attacking,
    dead
}

public class Enemy : MonoBehaviour
{
    public EnemyState currentState;
    public Vector3 initialPos;
    public bool targetVisible;
    public float maxHealth;
    public float health;
    public float attackDamage;
    public float mobility;
    public Animator animator;
    public Rigidbody2D rb;
    public AudioSource[] aud;

    public GameObject hitEffect;

    public void ChangeState(EnemyState state)
    {
        currentState = state;
    }
    public void Knockback(Transform tr)
    {
        if (rb != null)
        {
            rb.isKinematic = false;
            ChangeState(EnemyState.hitStun);
            Vector2 difference = transform.position - tr.position;
            difference = difference.normalized * 5.0f;
            rb.AddForce(difference, ForceMode2D.Impulse);
        }
    }

    // Might need to have a variable that saves the current state prior to
    // entering hitstun to make sure it returns to it after the knockback.
    public IEnumerator KnockCo(Rigidbody2D rb)
    {
        if(rb != null)
        {
            yield return new WaitForSeconds(0.2f);
            rb.velocity = Vector2.zero;
            ChangeState(EnemyState.idle);
            rb.isKinematic = true;
        }
    }

    public void changeAnim(Vector2 direction)
    {
        direction = direction.normalized;

        if (direction != Vector2.zero)
        {
            animator.SetFloat("Horizontal", direction.x);
            animator.SetFloat("Vertical", direction.y);
            animator.SetBool("Moving", true);
        }
    }

    public void MoveAndAnimate(Vector3 moveVector, bool toward)
    {
        float direction = 1f;
        if (!toward)
            direction = -1f;

        Vector3 newPos = Vector3.MoveTowards(transform.position, moveVector, direction * mobility * Time.deltaTime);
        changeAnim(newPos - transform.position);
        rb.MovePosition(newPos);
    }

    public void PlayAudio(int index, float startTime, float endTime)
    {
        AudioSource audio = aud[index];
        audio.time = startTime;
        audio.Play();
        audio.SetScheduledEndTime(AudioSettings.dspTime + (endTime - startTime));
    }
}
