using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    private Battler enemy;

    //Curse you Unity for not allowing use to have pivot points for sprites!

    private Transform healthBarFillPivot; //Position where we scale the fill in the right direction
    private Transform healthBarScalePivot; // Scales the entire health bar without moving it out of place.

    // Start is called before the first frame update
    void Awake()
    {
        enemy = GetComponentInParent<Battler>();
        healthBarFillPivot = transform.GetChild(0).GetChild(0).GetChild(0);
        healthBarScalePivot = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        float globalMaxHealth = Battler.MAX_MAXHEALTH;
        float fillScaleFactor = enemy.health / enemy.maxHealth;
        float healthBarScaleFactor = enemy.maxHealth / globalMaxHealth;

        //Heard you liked GetChild calls, so I put a GetChild call within a GetChild call and did it again 
        //so you can get a sprite renderer that is a child of another sprite renderer so you can enable/disable

        // Yes, I should've just made an array of sprite renderers from all components. Yes, I didn't do it. Why? IDK.
        if (fillScaleFactor == 1)
        {
            GetComponent<SpriteRenderer>().enabled = false; //Disables icon
            transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().enabled = false; //Disables background;
            transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().enabled = false; //Disables fill;

        }
        else
        {
            GetComponent<SpriteRenderer>().enabled = true; //Enables icon
            transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().enabled = true; //Enables background;
            transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().enabled = true; //Enables fill;
        }

        healthBarFillPivot.transform.localScale = new Vector3(fillScaleFactor, 1, 1);
        healthBarScalePivot.transform.localScale = new Vector3(healthBarScaleFactor, 1, 1);
    }
}
