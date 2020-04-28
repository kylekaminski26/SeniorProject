using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Brawler
{
    private int count = 0;
    [SerializeField] private GameObject babySlime;

    private void Update()
    {
        base.Update();

        if (health < (.30) * maxHealth) {
            if (Random.RandomRange(0, 100) == 1) {
                while (count < Random.RandomRange(1,6))
                {
                    Enemy e = (Instantiate(    babySlime,
                                    new Vector3(gameObject.transform.position.x + Random.Range(-1.0f, 1.0f),
                                            gameObject.transform.position.y + Random.Range(-1.0f, 1.0f),
                                            0.0f),
                                    gameObject.transform.rotation,
                                    GameObject.Find("enemyListContainer").transform)).GetComponent<Enemy>();
                    e.SetStats(.5f*maxHealth,maxStamina,baseAttack*.3f,3*movementSpeed,dexterity*1.5f,vitality);
                    count++;
                }
            }
            Die();
        }
    }
}