using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//TO-Dos: Need to reconsider states in Battler, this construction utilizes two sets of states: Battler and AI
//and it is confusing to make sense of

public class Brawler : Enemy
{

    private float visionRadius; //Vision Range of Battler
    public Transform target; //Target of the Battler

    //concerning the implementation of generic hitboxes
    private BoxCollider2D effectiveHitbox;
    private List<BoxCollider2D> Hitboxes;

    private BoxCollider2D targetHurtbox;
    private Random random;

    //TG
    private GameObject chaseTarget;
    [SerializeField] private float chaseDistance; //The distance in which the enemy will try to maintain from the player during chase
    [SerializeField] private float fleeTime;
    [SerializeField] private float fleeRange;
    [SerializeField] private GameObject waypoint;

    private GameObject[] patrolPointArray = new GameObject[4];//Must be the same size declared in the Patrol script attached to enemy
    private GameObject[] searchPointArray = new GameObject[4];
    [SerializeField] private float searchTime;//How long this enemy will search for a target
    [SerializeField] private float searchRange;//The range in which the enemy will search for the player

    private bool bIsSearching;
    private bool bIsFleeing;
    private bool bIsCounting;

    private Pathfinding.AIPath Path; //used to toggle movement and search restraints
    private Pathfinding.AIDestinationSetter DestinationSetter; //has variable target (a transform of the goal)
    private Pathfinding.Patrol AIPatrol; //used to define a patrol path

    public AIState currentAIState;
    public AIState previousAIState;

    public float attackerTargetDistance = 0;

    void Awake()
    {
        base.Awake();

        dexterity = .05f;
        vitality = .01f;
        movementSpeed = 5f;

        //initialize child specific variables
        visionRadius = 5f;

        //get references to all attatched hitboxes and set the default
        Hitboxes = new List<BoxCollider2D>();
        Transform[] ts = GetComponentsInChildren<Transform>();
        foreach (Transform t in ts)
        {
            if (t.CompareTag("Hitbox"))
            {
                Hitboxes.Add(t.GetComponent<BoxCollider2D>());

            }
        }

        effectiveHitbox = Hitboxes[0];

        Path = GetComponent<Pathfinding.AIPath>();
        DestinationSetter = GetComponent<Pathfinding.AIDestinationSetter>();
        AIPatrol = GetComponent<Pathfinding.Patrol>();

        Path.maxSpeed = movementSpeed;

        currentAIState = AIState.idle;
        previousAIState = AIState.start;

        if (GameObject.Find("Player"))
        {
            target = GameObject.FindWithTag("Player").transform;
        }

        targetHurtbox = target.Find("Hurtbox").GetComponent<BoxCollider2D>();

        //TG
        //I am not searching on instantiation
        bIsSearching = false;
        bIsFleeing = false;
        bIsCounting = false;

        //Create chase waypoint
        chaseTarget = Instantiate(waypoint, gameObject.transform.position, new Quaternion(), GameObject.Find("waypointListContainer").transform);

        //Create patrol waypoints
        patrolPointArray[0] = Instantiate(waypoint, new Vector3(gameObject.transform.position.x + 5f, gameObject.transform.position.y + 5f, 0), new Quaternion(), GameObject.Find("waypointListContainer").transform);
        patrolPointArray[1] = Instantiate(waypoint, new Vector3(gameObject.transform.position.x + 5f, gameObject.transform.position.y - 5f, 0), new Quaternion(), GameObject.Find("waypointListContainer").transform);
        patrolPointArray[2] = Instantiate(waypoint, new Vector3(gameObject.transform.position.x - 5f, gameObject.transform.position.y + 5f, 0), new Quaternion(), GameObject.Find("waypointListContainer").transform);
        patrolPointArray[3] = Instantiate(waypoint, new Vector3(gameObject.transform.position.x - 5f, gameObject.transform.position.y - 5f, 0), new Quaternion(), GameObject.Find("waypointListContainer").transform);

    }

    protected void Update()
    {
        base.Update();

        if (target != null)
        {   //cannot think if in hitstun or currently attacking
            if (currentState != BattlerState.attack && currentState != BattlerState.hitStun)
                Think();
        }
        setEffectiveHitbox();


    }



    void Think()
    {
        //////////////////////////////////////////////////
        //Derive variables necessary for state transitions
        //////////////////////////////////////////////////


        //center of my hitbox
        Vector3 attackerHitboxCenter = (Vector3)effectiveHitbox.transform.position + (Vector3)effectiveHitbox.offset;

        //center of target's hurtbox
        Vector3 targetHurtboxCenter = (Vector3)targetHurtbox.transform.position + (Vector3)targetHurtbox.offset;//(Vector3)targetHurtbox.bounds.center;

        //minimal dimension of target's hurtbox(needs to be boxcollider2D)
        float minDim = (targetHurtbox.size.x < targetHurtbox.size.y) ? targetHurtbox.size.x : targetHurtbox.size.y;

        //minimal dimension of my effectivehitbox
        float attackerMinDim = (effectiveHitbox.size.x < effectiveHitbox.size.y) ? effectiveHitbox.size.x : effectiveHitbox.size.y;

        //note that the if the the distance between the center points is the radius + the mininal dimension
        //it will guarentee you are inside the effectivehitbox


        //distance between centers
        //remove from public when done
        attackerTargetDistance = Vector3.Distance(attackerHitboxCenter, targetHurtboxCenter);

        //the unit vector pointing to target
        Vector3 TargetDirection = Vector3.Normalize(targetHurtboxCenter - attackerHitboxCenter);

        //How far inside the targets hurtbox do we want to be?
        //aim to have atleast half of your effectivehitbox in them
        float penetration = attackerMinDim / 2;

        //the vector that points from the attacker's effectivehitbox to the targets hurtbox
        Vector3 TargetVector = TargetDirection * (attackerTargetDistance + penetration) + transform.position;

        float touchDistance = attackerMinDim / 2 + minDim / 2; //the smallest distance the colliders can
                                                               //be without touching each other



        /////////////////////////////////////////////
        ///Clean Up State Exits
        /////////////////////////////////////////////

        if (currentAIState != AIState.chase && previousAIState == AIState.chase)
        {
            DestinationSetter.enabled = false;
            Path.enabled = false;
        }

        //TG
        if (currentAIState != AIState.patrol && previousAIState == AIState.patrol)
        {
            AIPatrol.enabled = false;
            Path.enabled = false;
        }

        if (currentAIState != AIState.approach && previousAIState == AIState.approach)
        {
            DestinationSetter.enabled = false;
            Path.enabled = false;
        }

        if (currentAIState != AIState.flee && previousAIState == AIState.flee)
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
                    if (attackerTargetDistance <= visionRadius)
                    {
                        previousAIState = currentAIState;
                        currentAIState = AIState.chase;
                    }

                    //If the target is not in visual range and I am NOT currently looking for them | TG
                    //Then Patrol
                    if ((attackerTargetDistance > visionRadius) && currentAIState != AIState.sleuth)
                    {
                        previousAIState = currentAIState;
                        currentAIState = AIState.patrol;
                    }

                }

                break;


            case AIState.patrol: //Patrol state TG
                //  Enable patrol script with the created waypoints
                //  Can Switch to chase

                //Am I already patrolling?
                if (previousAIState != AIState.patrol)
                {
                    AIPatrol.targets[0] = patrolPointArray[0].transform;
                    AIPatrol.targets[1] = patrolPointArray[1].transform;
                    AIPatrol.targets[2] = patrolPointArray[2].transform;
                    AIPatrol.targets[3] = patrolPointArray[3].transform;

                    AIPatrol.delay = 0f;    //Sets a delay(seconds) before the enemy will begin towards next search point
                    Path.enabled = true;
                    AIPatrol.enabled = true;
                    previousAIState = AIState.patrol;
                    currentState = BattlerState.walk;

                }

                //Is the target in visible range?
                if (attackerTargetDistance <= visionRadius)
                {
                    previousAIState = currentAIState;
                    currentAIState = AIState.chase;
                }

                break;


            case AIState.chase:// Chase state

                //Is this a self transition to chase or from another node?
                if (previousAIState != AIState.chase)
                {
                    currentState = BattlerState.walk;
                    DestinationSetter.enabled = true;
                    Path.enabled = true;
                    previousAIState = AIState.chase;
                    currentState = BattlerState.walk;
                }

                //If the target is within visual range (That's why we're here), then
                //chase them from (chaseDistance) units away
                chaseTarget.transform.position = gameObject.transform.position + TargetDirection * (attackerTargetDistance - chaseDistance);
                DestinationSetter.target = chaseTarget.transform;

                //If I am able to attack, then approach
                //Changed the cost of stamina to the calculated amount
                if (stamina >= (.05f * 50) + (.10f * baseAttack))
                {
                    previousAIState = currentAIState;
                    currentAIState = AIState.approach;
                }

                //If I am too damaged, I will flee
                if (health <= maxHealth / 5)
                {
                    previousAIState = currentAIState;
                    currentAIState = AIState.flee;
                }

                //The target is no longer in visual range. Search for them | TG
                if (attackerTargetDistance > visionRadius)
                {
                    previousAIState = currentAIState;
                    currentAIState = AIState.sleuth;
                }

                //was I hit?
                if (currentState == BattlerState.hitStun)
                {
                    previousAIState = AIState.chase;
                    currentAIState = AIState.idle;
                }

                break;


            case AIState.sleuth: //Sleuth state TG
                //Can switch to patrol or chase

                //Am I already searching?
                if (previousAIState != AIState.sleuth)
                {
                    bIsSearching = true;
                    bIsCounting = false;

                    // Instantiate waypoints based on the target's position
                    searchPointArray[0] = Instantiate(waypoint, new Vector3(target.transform.position.x, target.transform.position.y + searchRange, 0), new Quaternion());
                    searchPointArray[1] = Instantiate(waypoint, new Vector3(target.transform.position.x, target.transform.position.y - searchRange, 0), new Quaternion());
                    searchPointArray[2] = Instantiate(waypoint, new Vector3(target.transform.position.x + searchRange, target.transform.position.y, 0), new Quaternion());
                    searchPointArray[3] = Instantiate(waypoint, new Vector3(target.transform.position.x - searchRange, target.transform.position.y, 0), new Quaternion());

                    //Set targets to the waypoints created for searching
                    AIPatrol.targets[0] = searchPointArray[0].transform;
                    AIPatrol.targets[1] = searchPointArray[1].transform;
                    AIPatrol.targets[2] = searchPointArray[2].transform;
                    AIPatrol.targets[3] = searchPointArray[3].transform;

                    Path.enabled = true;
                    AIPatrol.delay = 1f; //Sets a delay(seconds) before the enemy will begin towards next search point
                    AIPatrol.enabled = true;
                    previousAIState = AIState.sleuth;
                    currentState = BattlerState.walk;

                }

                //After searching for X amount of time | See function searchForTime
                //If I am no longer searching
                //Return to normal patrol
                if (bIsSearching == false)
                {
                    previousAIState = AIState.sleuth;
                    currentAIState = AIState.patrol;

                    AIPatrol.enabled = false;

                    Destroy(searchPointArray[0]);
                    Destroy(searchPointArray[1]);
                    Destroy(searchPointArray[2]);
                    Destroy(searchPointArray[3]);
                }

                //Is the target in visible range?
                //If yes, chase
                if (attackerTargetDistance <= visionRadius)
                {
                    previousAIState = AIState.sleuth;
                    currentAIState = AIState.chase;

                    AIPatrol.enabled = false;

                    Destroy(searchPointArray[0]);
                    Destroy(searchPointArray[1]);
                    Destroy(searchPointArray[2]);
                    Destroy(searchPointArray[3]);
                }

                //Count down until I give up the search
                if (bIsCounting == false)
                {
                    StartCoroutine(doForTime(searchTime));
                }

                break;


            //Create approach state TG
            case AIState.approach:
                //Is this a self transition to chase or from another node?
                if (previousAIState != AIState.approach)
                {
                    currentState = BattlerState.walk;
                    DestinationSetter.enabled = true;
                    DestinationSetter.target = target;
                    Path.enabled = true;
                    previousAIState = AIState.approach;
                    currentState = BattlerState.walk;
                }

                //The target is no longer in visual range. Search for them | TG
                if (attackerTargetDistance > visionRadius)
                {
                    previousAIState = currentAIState;
                    currentAIState = AIState.sleuth;
                }

                //If I am too damaged, then flee
                if (health <= maxHealth / 5)
                {
                    previousAIState = currentAIState;
                    currentAIState = AIState.flee;
                }

                //should have a constraints in here for available stamina,
                //flat attack wait mininum
                //and attack rate delay based on dexterity
                //touch distance is the smallest possible distance without an intersection
                //of hit box and hurtbox
                if (attackerTargetDistance < touchDistance && currentState != BattlerState.attack) //prevent from attacking to often
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

            //TG
            case AIState.flee:
                //When health is low, Flee to a patrol point
                //and resume a patrol after X amount of time
                //Is this a self transition to chase or from another node?
                if (previousAIState != AIState.flee)
                {
                    bIsFleeing = true;
                    bIsCounting = false;

                    currentState = BattlerState.walk;
                    DestinationSetter.enabled = true;
                    Path.enabled = true;
                    previousAIState = AIState.flee;
                    currentState = BattlerState.walk;
                }

                //If the target is within visual range (That's why we're here), then
                //chase them from (chaseDistance) units away
                chaseTarget.transform.position = gameObject.transform.position - TargetDirection * (fleeRange);
                DestinationSetter.target = chaseTarget.transform;

                if (bIsFleeing == false)
                {
                    previousAIState = AIState.flee;
                    currentAIState = AIState.idle;

                    DestinationSetter.enabled = false;

                }

                //Count down until I give up the search
                if (bIsCounting == false)
                {
                    StartCoroutine(doForTime(fleeTime));
                }

                break;

        }

    }

    //checks the movement direction of the brawler, and set the
    //the effective hitbox to the hitbox that is closest to the direction
    //of movement
    public void setEffectiveHitbox()
    {
        if (Hitboxes.Count > 1)
        {
            //do stuff
            int minNdx = 0;
            float minAngle = 2 * Mathf.PI;
            Vector3 hitboxPosition;
            for (int i = 0; i < Hitboxes.Count; i++)
            {
                hitboxPosition = Hitboxes[i].offset;
                float angle = Mathf.Acos(Vector3.Dot(Vector3.Normalize(hitboxPosition), Vector3.Normalize(movementDirection)));
                if (angle < minAngle)
                {
                    minNdx = i;
                    minAngle = angle;
                }
            }
            effectiveHitbox = Hitboxes[minNdx];
        }

        //otherwise default hitbox is tautological
    }


    //Co-routine for handling attack (toggling of effectivehitbox and Battler States)
    public IEnumerator AttackCo()
    {
        //buffer frames
        currentState = BattlerState.attack;
        yield return new WaitForSeconds(.2f); 

        //Attack Frames
        BoxCollider2D activeHitbox = effectiveHitbox;
        activeHitbox.enabled = true;
        stamina -= (.05f * 50) + (.10f * baseAttack); //3; where 50 = globalMaxBaseAttack 
        yield return new WaitForSeconds(0.3f);

        //Ending Frames
        //disable all hitboxes after an attack ends
        //since the effective hitbox may have been changed
        //during the yield time of this coroutine
        activeHitbox.enabled = false;
        currentState = BattlerState.idle;

    }

    //TG
    public IEnumerator doForTime(float time)
    {
        bIsCounting = true;

        yield return new WaitForSeconds(time);
        //After X seconds I am no longer searching for the target
        bIsFleeing = false;
        bIsSearching = false;
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
        statList.Add(visionRadius);

        return statList;
    }

}
