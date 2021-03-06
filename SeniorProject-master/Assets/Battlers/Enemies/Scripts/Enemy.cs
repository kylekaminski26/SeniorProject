﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
    The Enemy class servers as the base class for all enemy instances.
    It contains fucntionality that relates enemy battlers to the player.
*/
public class Enemy : Battler
{
    
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

    //variables concerning evolution heuristic
    public float damageDealtToPlayer = 0; //adjusted in battler onHurtboxCollision 
    public float healthRegenerated = 0; //internal


    //This Awake() is overwritten so that enemies can locate the player
    public void Awake() {

        base.Awake();

        //enemy defaults
        maxHealth = Battler.MIN_MAXHEALTH;
        maxStamina = Battler.MIN_MAXSTAMINA;
        baseAttack = Battler.MIN_BASEATTACK;
        dexterity = Battler.MIN_DEXTERITY;
        vitality = Battler.MIN_VITALITY;
        movementSpeed = Battler.MIN_MAXMOVEMENTSPEED;

        if (GameObject.Find("Player"))
        {
            target = GameObject.FindWithTag("Player").transform;
            targetHurtbox = target.Find("Hurtbox").GetComponent<BoxCollider2D>();
        }
    }

    public void Update()
    {
        float healthDiff = (-1 * health);
        base.Update();
        healthRegenerated += (healthDiff + health);

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

    //called in players onHurtBoxCollision
    public void incrementDamageDealt() {
        damageDealtToPlayer += baseAttack;
    }

    //@precondition the enemy has died
    //a heuristic of the enemies fitness
    public float performanceHeuristic()
    {
        return (damageDealtToPlayer*2) + healthRegenerated;
    }

}
