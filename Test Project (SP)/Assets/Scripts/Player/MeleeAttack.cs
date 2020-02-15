using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    public float attackRate = 0.375f;

    private float nextAttack = 0.0f;
    private string directionOfAttack = "Down";


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1") && Time.time > nextAttack)
        {
            nextAttack = Time.time + attackRate;

            if (PlayerMovement.IsFacingUp)
                directionOfAttack = "Up";
            else if (PlayerMovement.IsFacingDown)
                directionOfAttack = "Down";
            else if (PlayerMovement.IsFacingLeft)
                directionOfAttack = "Left";
            else if (PlayerMovement.IsFacingRight)
                directionOfAttack = "Right";
            Invoke("Attack", 0.375f);
        }
    }
}
