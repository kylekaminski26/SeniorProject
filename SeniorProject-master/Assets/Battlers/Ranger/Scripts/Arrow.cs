using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private BoxCollider2D hitBox;
    // Start is called before the first frame update
    void Awake()
    {
        hitBox = GetComponent<BoxCollider2D>();
        Invoke("enableCollider", 0.5f);
        Destroy(gameObject, 3.0f);
    }

    void enableCollider()
    {
        hitBox.enabled = true;
    }
}
