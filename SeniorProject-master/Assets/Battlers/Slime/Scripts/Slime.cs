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
    private Collider2D Hitbox; //should be generalized in parent (all Battlers have a list of hitboxes)
    private Random random;

    private Pathfinding.AIPath Path; //used to toggle movement and search restraints
    private Pathfinding.AIDestinationSetter DestinationSetter; //has variable target (a transform of the goal)

    public AIState currentAIState;
    public AIState previousAIState;



    void Awake()
    {
        base.Awake();
        //initialize child specific variables
        chaseRadius = 5f;
        //Hitbox = GetComponentInChildren<CircleCollider2D>(); This manner of reference is problematic, and the name is misleading
        //this does a breath first search from the parent down to children returning the first occurence of the specified type
        Hitbox = transform.GetChild(0).gameObject.GetComponent<Collider2D>(); //get the 1st and only child object to reference hitbox
        Hitbox.enabled = false;

        Path = GetComponent<Pathfinding.AIPath>();
        DestinationSetter = GetComponent<Pathfinding.AIDestinationSetter>();

        currentAIState = AIState.idle;
        previousAIState = AIState.start;
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            //Depending on Battler State, AI can not transition its State on its own
            if (currentState != BattlerState.attack && currentState != BattlerState.hitStun)
            {
                Think();
            }
           
        }
    }

   
    void Think()
    {
        //////////////////////////////////////////////////
        //Derive variables necessary for state transitions
        //////////////////////////////////////////////////

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
        
        
        switch (currentAIState) {


            case AIState.idle: //Idle state
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
                break;



            case AIState.chase:// Chase state

                //Is this a self transition to chase or from another node?
                if (previousAIState != AIState.chase) {
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
                if (attackerTargetDistance < 0)
                {
                    previousAIState = currentAIState;
                    currentAIState = AIState.attack;
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
}
