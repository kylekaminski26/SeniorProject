using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
    This hurtbox script should be attatched to the hurtbox of every battler.
    It provides a convenient way to determine whether an attack has occured,
    and transmitting who the attack is from
*/

public class Hurtbox : MonoBehaviour
{
    Battler parent;

    //when this is instantiated get a reference to the parent
    private void Awake()
    {
        parent = transform.parent.gameObject.GetComponent<Battler>();
    }

    //when the hurtbox is entered we alert the parent Battler object by calling
    //a public function which takes in the collided object as a parameter
    private void OnTriggerEnter2D(Collider2D collision)
    {
        parent.OnHurtboxCollision(collision);
    }
}
