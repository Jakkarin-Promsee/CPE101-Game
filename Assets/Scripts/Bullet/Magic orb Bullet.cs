using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicorbBullet : Bullet
{
    public override void Start()
    {
        base.Start();
    }

    // For the Bullet script
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
    }
}
