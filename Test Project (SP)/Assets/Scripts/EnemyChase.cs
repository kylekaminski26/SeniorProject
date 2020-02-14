using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    public float moveSpeed; // WILL LATER BE CHANGED TO MOBILITY
    public GameObject chaseWaypoint;
    private Animator animator;
    private Rigidbody2D rb;

    private bool bIsChasable;

    // Used to determine what sprite to be used when player is idle
    public static bool IsFacingLeft = false;
    public static bool IsFacingRight = false;
    public static bool IsFacingDown = false;
    public static bool IsFacingUp = false;

    void Start()
    {
        bIsChasable = false;

        animator = gameObject.GetComponent<Animator>();
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    void OnTriggerEnter2D(Collider2D collision)//Player enters the chase range
    {
        if(collision.tag == "Player")
        {
            Debug.Log("In range");//Chase
            bIsChasable = true;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)//Player exits the chase range
    {
        if(collision.tag == "Player")
        {
            Debug.Log("Out of range");//Stop Chasing
            bIsChasable = false;
        }

    }

    void Update()
    {
        if(bIsChasable == true)
        {
           rb.velocity = (9/10f)*(new Vector2(chaseWaypoint.GetComponentInParent<Transform>().position.x, chaseWaypoint.GetComponentInParent<Transform>().position.y)
                         - new Vector2(gameObject.transform.position.x, gameObject.transform.position.y));
        }
        else
        {
            rb.velocity = new Vector2(0f, 0f);//sets target to current position
        }

        if (rb.velocity.x < 0)
        {
            IsFacingLeft = true;
            IsFacingRight = false;
            IsFacingDown = false;
            IsFacingUp = false;
        }
        else if (rb.velocity.x > 0)
        {
            IsFacingLeft = false;
            IsFacingRight = true;
            IsFacingDown = false;
            IsFacingUp = false;
        }

        if (rb.velocity.y < 0)
        {
            IsFacingLeft = false;
            IsFacingRight = false;
            IsFacingDown = true;
            IsFacingUp = false;
        }
        else if (rb.velocity.y > 0)
        {
            IsFacingLeft = false;
            IsFacingRight = false;
            IsFacingDown = false;
            IsFacingUp = true;
        }

        // Adds animations
        animator.SetFloat("Horizontal", rb.velocity.x);
        animator.SetFloat("Vertical", rb.velocity.y);
        animator.SetFloat("Speed", rb.velocity.sqrMagnitude);

        animator.SetBool("Facing Down", IsFacingDown);
        animator.SetBool("Facing Up", IsFacingUp);
        animator.SetBool("Facing Left", IsFacingLeft);
        animator.SetBool("Facing Right", IsFacingRight);
    }
}


