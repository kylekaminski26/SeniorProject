using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//State variables for AI thinking
public enum AIState
{
    idle,
    chase,
    attack,
    start
}

//TO-Dos: Need to reconsider states in Battler, this construction utilizes two sets of states: Battler and AI
//and it is confusing to make sense of

public class Slime : Battler
{
    private float chaseRadius; //how far the slime can see
    public Transform target; //the target of this Battler
    private CircleCollider2D Hitbox; //should be generalized in parent (all Battlers have a list of hitboxes)
    private BoxCollider2D targetHurtbox;
    private Random random;

    private Pathfinding.AIPath Path; //used to toggle movement and search restraints
    private Pathfinding.AIDestinationSetter DestinationSetter; //has variable target (a transform of the goal)

    public AIState currentAIState;
    public AIState previousAIState;

    public float attackerTargetDistance = 0;



    void Awake()
    {
        base.Awake();
        //initialize child specific variables
        chaseRadius = 5f;

        //Hitbox = GameObject.Find("Hitbox").GetComponent<CircleCollider2D>();
        Hitbox = GetComponentInChildren<CircleCollider2D>();
        Hitbox.enabled = false;


        Path = GetComponent<Pathfinding.AIPath>();
        DestinationSetter = GetComponent<Pathfinding.AIDestinationSetter>();

        currentAIState = AIState.idle;
        previousAIState = AIState.start;

        if (GameObject.Find("Player"))
        {
            target = GameObject.FindWithTag("Player").transform;
        }
        
        targetHurtbox = target.Find("Hurtbox").GetComponent<BoxCollider2D>();


    }

    void FixedUpdate()
    {
        if (target != null)
        {
            Think();
        }
    }

   
    void Think()
    {
        //////////////////////////////////////////////////
        //Derive variables necessary for state transitions
        //////////////////////////////////////////////////


        //center of my hitbox
        Vector3 attackerHitboxCenter = (Vector3)Hitbox.transform.position + (Vector3)Hitbox.offset;

        //center of target's hurtbox
        Vector3 targetHurtboxCenter = (Vector3)targetHurtbox.transform.position + (Vector3)targetHurtbox.offset;//(Vector3)targetHurtbox.bounds.center;

        //maximal dimension of target's hurtbox(needs to be boxcollider2D)
        float minDim = (targetHurtbox.size.x < targetHurtbox.size.y) ? targetHurtbox.size.x : targetHurtbox.size.y;

        //radius of my hitbox
        float ra = Hitbox.radius;

        //note that the if the the distance between the center points is the radius + the mininal dimension
        //it will guarentee you are inside the hitbox


        //distance between centers
        //remove from public when done
        attackerTargetDistance = Vector3.Distance(attackerHitboxCenter, targetHurtboxCenter);

        //the unit vector pointing to target
        Vector3 TargetDirection = Vector3.Normalize(targetHurtboxCenter - attackerHitboxCenter);

        //How far inside the targets hurtbox do we want to be?
        //aim to have atleast half of your hitbox in them
        float penetration = ra/2; 

        //the vector that points from the attacker's hitbox to the targets hurtbox
        Vector3 TargetVector = TargetDirection * (attackerTargetDistance + penetration) + transform.position;

        float touchDistance = ra + minDim/2;



        /////////////////////////////////////////////
        ///Clean Up State Exits
        /////////////////////////////////////////////

        if (currentAIState != AIState.chase && previousAIState == AIState.chase)
        {
            DestinationSetter.enabled = false;
            Path.enabled = false;
        }







        //////////////////////
        ///State Transitions
        //////////////////////

        



        switch (currentAIState)
        {


            case AIState.idle: //Idle state

                //if in HitStun lock into Idle
                if (currentState != BattlerState.hitStun)
                {

                    //Am I already Idling?
                    if (previousAIState != AIState.idle)
                    {
                        previousAIState = currentAIState;
                        currentState = BattlerState.idle;
                    }
                    //Is the target in visible range?
                    if (attackerTargetDistance <= chaseRadius)
                    {
                        previousAIState = currentAIState;
                        currentAIState = AIState.chase;
                    }

                }
                

                break;



            case AIState.chase:// Chase state

                //Is this a self transition to chase or from another node?
                if (previousAIState != AIState.chase)
                {
                    currentState = BattlerState.walk;
                    DestinationSetter.enabled = true;
                    DestinationSetter.target = target;
                    Path.enabled = true;
                    previousAIState = AIState.chase;
                    currentState = BattlerState.walk;
                }

                //Is the target no longer in range?
                if (attackerTargetDistance >= chaseRadius)
                {
                    previousAIState = currentAIState;
                    currentAIState = AIState.idle;
                }


                //should have a constraints in here for available stamina,
                //flat attack wait mininum
                //and attack rate delay based on dexterity
                //touch distance is the smallest possible distance without an intersection
                //of hit box and hurtbox
                if (attackerTargetDistance < touchDistance)
                {
                    previousAIState = currentAIState;
                    currentAIState = AIState.attack;
                }

                //was I hit?
                if (currentState == BattlerState.hitStun)
                {
                    previousAIState = AIState.chase;
                    currentAIState = AIState.idle;
                }


                break;

            case AIState.attack: //Attack state

                StartCoroutine(AttackCo());
                previousAIState = currentAIState;
                currentAIState = AIState.idle;
                break;




        }

        
 }
        
    //Co-routine for handling attack (toggling of hitbox and Battler States)
    public IEnumerator AttackCo()
    {
        
        Hitbox.enabled = true;
        currentState = BattlerState.attack;
        yield return new WaitForSeconds(0.2f);
        Hitbox.enabled = false;
        currentState = BattlerState.idle;
        
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

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.CompareTag("player_attack") && currentState != BattlerState.hitStun)
        {
            //This will be changed to where it gets the damage from the
            // hitbox rather than from the baseAttack of the enemy.
            if (health - other.gameObject.GetComponentInParent<Battler>().baseAttack <= 0)
            {
                TakeDamage(other.gameObject.GetComponentInParent<Battler>().baseAttack);
                Die();
            }
            TakeDamage(other.gameObject.GetComponentInParent<Battler>().baseAttack);

            //* Need to use component in parent to utilize this script. Otherwise
            //* we need to add a knockback/damage script to the hitboxes of the enemy entity as well
            //* rather than having it all on the single enemy object.

            if (currentState != BattlerState.dead)
            {
                //GameObject combatEffect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
                // Destroy(combatEffect, 0.35f);
                Knockback(other.transform);
                StartCoroutine(KnockCo());
            }
        }
    }
    }
