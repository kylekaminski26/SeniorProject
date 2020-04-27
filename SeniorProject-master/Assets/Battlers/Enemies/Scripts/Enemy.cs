using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        start
    }
}
