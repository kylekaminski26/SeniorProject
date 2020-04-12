using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform target; // Gets the target object (aka. the player)
    public float dampSpeed;  // Dampening speed for repositioning camera
    public Vector2 maxPos;
    public Vector2 minPos;

    void LateUpdate()
    {
        
        if(transform.position != target.position)
        {
            Vector3 newPos = new Vector3(target.position.x, target.position.y, transform.position.z); // Sets X and Y to player's position and Z to -10
            newPos.x = Mathf.Clamp(newPos.x, minPos.x, maxPos.x);
            newPos.y = Mathf.Clamp(newPos.y, minPos.y, maxPos.y);


            transform.position = Vector3.Lerp(transform.position, newPos, dampSpeed); // Adjusts speed of camera repositioning
        }
    }
}
