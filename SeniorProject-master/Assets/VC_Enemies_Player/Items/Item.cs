using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public float hpRestore;
    public float weaponAttack;
    public float projectileAttack;

    //These values will be set in the Inspector as each item has different values.

    public float GetHpRestore()
    {
        return hpRestore;
    }
    public float GetNewBaseAttack()
    {
        return weaponAttack;
    }
    public float GetNewProjectileAttack()
    {
        return projectileAttack;
    }
}
