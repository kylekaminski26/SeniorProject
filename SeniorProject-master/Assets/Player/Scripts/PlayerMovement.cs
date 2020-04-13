using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerStateJM
{
    walk,
    attack,
    interact
}

public class PlayerMovement : MonoBehaviour
{
    public PlayerStateJM currentState;
    public float speed;
    private Rigidbody2D myRigidbody;
    private Vector3 change;
    private Animator animator;

    private AnimatorStateInfo animationState;
    private int attackHash = Animator.StringToHash("Attack");


    // Start is called before the first frame update
    // Use this for initialization
    void Start()
    {
        currentState = PlayerStateJM.walk;
        animator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
        animator.SetFloat("moveX", 0);
        animator.SetFloat("moveY", -1);
    }

    //Update is called every frame. 
    private void Update()
    {
        if (Input.GetButtonDown("Attack") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            animator.SetTrigger(attackHash);
        }
    }

    //FixedUpdate is called at a fixed interval and is independent of frame rate. Put physics code here.
    void FixedUpdate()
    {
        change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal");
        change.y = Input.GetAxisRaw("Vertical");

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            UpdateAminationAndMove();
        }
    }

    void UpdateAminationAndMove()
    {
        if (change != Vector3.zero)
        {
            MoveCharacter();
            animator.SetFloat("moveX", change.x);
            animator.SetFloat("moveY", change.y);
            animator.SetBool("moving", true);
        }
        else
        {
            animator.SetBool("moving", false);
        }
    }

    void MoveCharacter()
    {
        change.Normalize();
        myRigidbody.MovePosition(
               transform.position + change * speed * Time.deltaTime
            );
    }

}
