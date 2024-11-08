using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBullet : Bullet
{
    public override void Start()
    {
        base.Start();
    }

    // For the Bullet script
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        if (other.CompareTag("Enemy"))
        {
            Destroy(gameObject);  // Destroy the bullet after collision
        }
    }

}
