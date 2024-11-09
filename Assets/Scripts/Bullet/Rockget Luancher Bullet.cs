using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockgetLuancherBullet : Bullet
{
    public float burnDamageMultiple = 0.5f;
    public float burntime = 0.75f;
    public float burnDuration = 3f;

    public override void Start()
    {
        base.Start();
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);

        if (other.CompareTag("Enemy"))
        {
            // Take burn effet to enemy and destroy this prefab
            other.gameObject.GetComponent<EnemyController>().ApplyBurnEffect(damage * burnDamageMultiple, burnDuration, burntime);
        }
    }
}
