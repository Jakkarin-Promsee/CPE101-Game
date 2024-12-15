using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class FireArea : Explosion
{
    [Header("General Setting. Necessary to setup.")]
    public float areaDuration = 5f;
    public float burnDamage = 3f;
    public float burnRate = 0.3f;

    private float nextBurn = 0;
    protected override void Start()
    {
        Destroy(gameObject, areaDuration);
    }

    // Update is called once per frame
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && Time.time > nextBurn)
        {
            nextBurn = Time.time + burnRate;
            other.gameObject.GetComponent<EnemyController>().TakeDamage(burnDamage);
        }

        if (other.CompareTag("Boss") && Time.time > nextBurn)
        {
            nextBurn = Time.time + burnRate;
            other.gameObject.GetComponent<SpiritKing>().TakeDamage(burnDamage);
        }
    }
}
