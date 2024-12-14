using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public RangeWeaponConfig weaponConfig;
    public string weaponOwnerTag = "";
    private bool hasReflected = false;
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

    public void Reflect(float newAngle)
    {
        if (!hasReflected)
        {
            hasReflected = true;
            ChangeMoveAngle(newAngle);
        }
    }

    public void ChangeMoveAngle(float newAngle)
    {
        Vector3 currentRotation = transform.eulerAngles;
        transform.eulerAngles = new Vector3(currentRotation.x, currentRotation.y, newAngle);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Boss") && weaponOwnerTag != "Enemy")
        {
            other.gameObject.GetComponent<SpiritKing>().TakeDamage(weaponConfig.damage);

            // Spon explosion prefab at bullet direction
            if (weaponConfig.explosionPrefab)
            {
                Instantiate(weaponConfig.explosionPrefab, (transform.position + other.transform.position) / 2, Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z));
            }

            // If bullet don't move through enemy, destroy it
            if (!weaponConfig.isMoveThroughObject)
                Destroy(gameObject);  // Destroy the bullet after collision 
        }

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

            // Calculate knockback vector
            float angleInRadians = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
            Vector2 knockbackDirection = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
            other.gameObject.GetComponent<PlayerMovement>().TakeKnockback(knockbackDirection * weaponConfig.knockback, weaponConfig.knockbackTime);

            Destroy(gameObject);
        }
    }
}
