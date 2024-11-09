using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public BulletConfig bullet;

    public float damage;
    public float knockback;
    public float knockbackTime;
    private Vector2 direction;

    public virtual void Start()
    {
        direction = new Vector2(1, 0); // Set bullet direct to move forward
        Destroy(gameObject, bullet.lifespan);  // Destroy after a certain time
    }

    public void Update()
    {
        MoveBullet();
    }

    protected virtual void MoveBullet()
    {
        transform.Translate(direction * bullet.speed * Time.deltaTime);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Take enemy damage
            other.gameObject.GetComponent<EnemyController>().TakeDamage(damage);

            // Calculate knockback vector
            float angleInRadians = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
            Vector2 knockbackDirection = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
            other.gameObject.GetComponent<EnemyActionController>().TakeKnockback(knockbackDirection * knockback, knockbackTime);

            // Spon explosion prefab at bullet direction
            if (bullet.explosionPrefab)
            {
                Instantiate(bullet.explosionPrefab, (transform.position + other.transform.position) / 2, Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z));
            }

            // If bullet don't move through enemy, destroy it
            if (!bullet.isMoveThroughObject)
                Destroy(gameObject);  // Destroy the bullet after collision 
        }
    }
}
