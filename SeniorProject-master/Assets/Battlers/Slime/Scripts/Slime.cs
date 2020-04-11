using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//TO-Dos: Need to ensure all states are being accounted for
// Need to fix code so it is more compact and portable
// Need to move some elements to the Enemy Class if they can be used elsewhere
public class Slime : Battler
{
    private float chaseRadius; //how far the slime can see
    public Transform target; //the target of this Battler
    private Collider2D Hitbox;
    private Random random;


  
    void Awake()
    {
        base.Awake();
        //initialize child specific variables
        chaseRadius = 5f;
        Hitbox = GetComponentInChildren<CircleCollider2D>();
        Hitbox.enabled = false;
    }

    void FixedUpdate()
    {
        //fail safe to allow game manager to assign targets
        if (target != null)
        {
            //Slime thinking
            if (currentState != BattlerState.hitStun)
            {
                Think();
            }

            //toggle hitbox collider based on state
            if (currentState == BattlerState.attack)
            {
                //enable hitbox
                Hitbox.enabled = true;
                StartCoroutine(AttackCo());
            }
            
            
        }
    }

    //determines if the target is in view, if so makes that slime moves towards it target
    //having the slimes hitbox sit at the edge of the target's hurtbox
    void Think()
    {
        /*

        //get attackers hitbox center
        Vector3 attackerHitboxCenter = transform.position + (Vector3) GetComponentInChildren<CircleCollider2D>().offset;
        //attackers Hitbox radius
        float ra = GetComponentInChildren<CircleCollider2D>().radius;
        //get targets hurtbox center
        Vector3 targetHurtboxCenter = target.position + (Vector3) GetComponent<CircleCollider2D>().offset;
        //get targets hurtbox radius
        float rt = GetComponent<CircleCollider2D>().radius;

        //get the distance between the the center of the targets hurt box and the attackers hitbox
        float attackerTargetDistance = Vector3.Distance(attackerHitboxCenter,targetHurtboxCenter) - ra - rt;
        //the unit vector pointing to target
        Vector3 TargetDirection = Vector3.Normalize(targetHurtboxCenter - attackerHitboxCenter);
        //How far inside the targets hurtbox do we want to be?
        float penetration = ra; //penetrate a 1/4 of the hitbox radius into the target

        //the vector that points from the attacker's hitbox to the targets hurtbox
        Vector3 TargetVector = TargetDirection * (attackerTargetDistance+penetration) + transform.position;


        //Is the attackers hitbox inside the targets hurtbox?
        //Is the attacker not in hitstun?
        if (attackerTargetDistance < 0
            && Time.time > (lastAttackTime + 1/maxStamina) && currentState != BattlerState.hitStun)
        {
            lastAttackTime = Time.time;
            currentState = BattlerState.attack;
        }

        //if not move the slime towards the target
        else if (attackerTargetDistance <= chaseRadius)
        {
            if ((currentState == BattlerState.idle || currentState == BattlerState.walk)
                && (currentState != BattlerState.hitStun || currentState != BattlerState.dead))
            {
                //Vector3 temp = Vector3.MoveTowards(transform.position, TargetVector, movementSpeed * Time.deltaTime);
                Vector3 temp = Vector3.MoveTowards(transform.position, TargetVector, movementSpeed * Time.deltaTime);
                changeAnim(temp - transform.position);
                rb.MovePosition(temp);
                currentState = BattlerState.walk;
          
            }
        }

        */

    }

    //Co-routine for handling the termination of Attack State
    public IEnumerator AttackCo()
    {
        if (rb != null)
        {
            yield return new WaitForSeconds(0.2f);
            Hitbox.enabled = false;
            currentState = BattlerState.idle;
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

  
    //Accessors and Mutators
    public void SetTarget(Transform tr)
    {
        //change the transform
        target = tr;
    }
   

    public List<float> GetStats()
    {
        List<float> statList = new List<float>();
        statList.Add(maxHealth);
        statList.Add(maxStamina);
        statList.Add(baseAttack);
        statList.Add(movementSpeed);
        statList.Add(dexterity);
        statList.Add(chaseRadius);

        return statList;
    }
}
