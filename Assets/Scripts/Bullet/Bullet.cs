using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public WeaponConfig weaponConfig;
    public string weaponOwnerTag = "";
    private Vector2 direction;

    public virtual void Start()
    {
        direction = new Vector2(1, 0); // Set bullet direct to move forward
        Destroy(gameObject, weaponConfig.lifespan);  // Destroy after a certain time
    }

    public void Update()
    {
        MoveBullet();
    }

    protected virtual void MoveBullet()
    {
        transform.Translate(direction * weaponConfig.speed * Time.deltaTime);
    }

    // ! Test
    public void ChangeDirection(Vector2 newDirection)
    {
        direction = newDirection; // Set the direction to the new direction (normalized to avoid speed changes)
    }
    // ! Test

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && weaponOwnerTag != "Enemy")
        {
            // Take enemy damage
            other.gameObject.GetComponent<EnemyController>().TakeDamage(weaponConfig.damage);
            other.gameObject.GetComponent<EnemyActionController>().IsAttacked();

            // Calculate knockback vector
            float angleInRadians = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
            Vector2 knockbackDirection = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
            other.gameObject.GetComponent<EnemyActionController>().TakeKnockback(knockbackDirection * weaponConfig.knockback, weaponConfig.knockbackTime);

            // Spon explosion prefab at bullet direction
            if (weaponConfig.explosionPrefab)
            {
                Instantiate(weaponConfig.explosionPrefab, (transform.position + other.transform.position) / 2, Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z));
            }

            // If bullet don't move through enemy, destroy it
            if (!weaponConfig.isMoveThroughObject)
                Destroy(gameObject);  // Destroy the bullet after collision 
        }

        if (other.CompareTag("Player") && weaponOwnerTag != "Player")
        {
            other.gameObject.GetComponent<PlayerController>().TakeDamage(weaponConfig.damage);
            Destroy(gameObject);
        }
    }
}
