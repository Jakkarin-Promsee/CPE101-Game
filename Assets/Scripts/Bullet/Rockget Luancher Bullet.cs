using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockgetLuancherBullet : Bullet
{
    public float burnDamageMultiple = 0.5f;
    public float burntime = 0.75f;
    public float burnDuration = 3f;
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);

        if (other.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<EnemyController>().ApplyBurnEffect(damage * burnDamageMultiple, burnDuration, burntime);
            Destroy(gameObject);  // Destroy the bullet after collision
        }
    }
}
