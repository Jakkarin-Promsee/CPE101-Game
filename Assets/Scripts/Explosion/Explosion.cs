using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    protected virtual void Start()
    {
        Destroy(gameObject, 0.2f);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
    }
}
