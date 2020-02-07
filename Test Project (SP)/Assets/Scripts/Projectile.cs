using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    void OnCollision2D(Collision2D other)
    {
        Destroy(gameObject);
    }
}
