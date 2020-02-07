using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform target; // Gets the target object (aka. the player)
    public float dampSpeed;  // Dampening speed for repositioning camera

    // Update is called once per frame
    void Update()
    {

        Vector3 newPos = target.position + new Vector3(0, 0, -10); // Sets X and Y to player's position and Z to -10
        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * dampSpeed); // Adjusts speed of camera repositioning
    }
}
