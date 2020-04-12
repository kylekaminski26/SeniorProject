using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetVisible : MonoBehaviour
{
    Enemy parentScript;
    void Start()
    {
        parentScript = this.GetComponentInParent<Enemy>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        parentScript.targetVisible = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
            parentScript.targetVisible = false;
    }
}
