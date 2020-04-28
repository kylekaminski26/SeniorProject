using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    private Battler enemy;
    private Transform  healthBarFill; //Technically the pivot, but it works.

    // Start is called before the first frame update
    void Awake()
    {
        enemy = GetComponentInParent<Battler>();
        healthBarFill = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        float scaleFactor = enemy.health / enemy.maxHealth;
        GetComponent<SpriteRenderer>().enabled = true;
        transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().enabled = true;

        //We should probably have it to where we get all the sprite renderers, put them into
        // an array, and disable/enable all of the them. But do we really need an array of 2 objects.
        /*
        if (scaleFactor == 1)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        }
        else
        {
            GetComponent<SpriteRenderer>().enabled = true;
            transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
        }
        */
        healthBarFill.transform.localScale = new Vector3(scaleFactor, 1, 1);
    }
}
