using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // WILL LATER BE CHANGED TO MOBILITY
    public Rigidbody2D rb;
    public Animator animator;
    Vector2 movement;

    // Used to determine what sprite to be used when player is idle
    public static bool IsFacingLeft = false;
    public static bool IsFacingRight = false;
    public static bool IsFacingDown = false;
    public static bool IsFacingUp = false;


    // Update is called once per frame
    void Update()
    {
        // Input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Determines what way the player is facing (used for idle)
        if (movement.x < 0)
        {
            IsFacingLeft = true;
            IsFacingRight = false;
            IsFacingDown = false;
            IsFacingUp = false;
        }
        else if (movement.x > 0)
        {
            IsFacingLeft = false;
            IsFacingRight = true;
            IsFacingDown = false;
            IsFacingUp = false;
        }

        if (movement.y < 0)
        {
            IsFacingLeft = false;
            IsFacingRight = false;
            IsFacingDown = true;
            IsFacingUp = false;
        }
        else if (movement.y > 0)
        {
            IsFacingLeft = false;
            IsFacingRight = false;
            IsFacingDown = false;
            IsFacingUp = true;
        }

        // Adds animations
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);

        animator.SetBool("Facing Down", IsFacingDown);
        animator.SetBool("Facing Up", IsFacingUp);
        animator.SetBool("Facing Left", IsFacingLeft);
        animator.SetBool("Facing Right", IsFacingRight);

    }

    void FixedUpdate()
    {
        // Movement
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}


