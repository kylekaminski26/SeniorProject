using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostBolt : MonoBehaviour
{
    private int layerMask;
    // Start is called before the first frame update
    void Awake()
    {
        layerMask = 1 << 10;
        Destroy(gameObject, 3.0f);
    }

    private void Update()
    {
        if (Physics2D.Raycast(transform.position, transform.position, 0.2f, layerMask))
        {
            Destroy(gameObject);
        }
    }

}
