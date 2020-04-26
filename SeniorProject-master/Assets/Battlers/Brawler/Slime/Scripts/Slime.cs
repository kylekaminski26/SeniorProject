using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Brawler
{
    private int count = 0;
    [SerializeField] private GameObject babySlime;

    private void OnDisable()
    {
        while (count < 2)
        {
            Instantiate(babySlime,
                    new Vector3(    gameObject.transform.position.x + Random.Range(-1.0f, 1.0f),
                                    gameObject.transform.position.y + Random.Range(-1.0f, 1.0f),
                                    0.0f),
                       gameObject.transform.rotation);
            count++;
        }

    }
}