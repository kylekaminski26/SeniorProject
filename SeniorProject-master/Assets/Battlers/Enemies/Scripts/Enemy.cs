using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Battler
{
    //Awake and Update help define specific interactions between
    //enemies and players, such as order in layer

    //State variables for AI thinking
    public enum AIState
    {
        idle,
        patrol,
        chase,
        sleuth,
        flee,
        approach,
        attack,
        dead,
        start
    }

    public Transform target; //Target of the Battler
    public BoxCollider2D targetHurtbox;


    public void Awake() {

        base.Awake();
        if (GameObject.Find("Player"))
        {
            target = GameObject.FindWithTag("Player").transform;
            targetHurtbox = target.Find("Hurtbox").GetComponent<BoxCollider2D>();
        }
    }

    public void Update()
    {
        base.Update();


        //check if the player is above me, if the player is above me than swap
        //my order in layer
        if (GameObject.Find("Player") && (target != null && currentState != BattlerState.dead)) {
            GameObject p = GameObject.Find("Player");
            if (p.transform.position.y > transform.position.y)
            {
                sr.sortingOrder = 1;
            }
            else {
                sr.sortingOrder = 0;
            }
        }

    }
}
