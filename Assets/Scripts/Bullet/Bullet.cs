using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public BulletConfig bullet;
    private Vector2 direction;
    public float damage;
    public float knockback;

    public virtual void Start()
    {
        direction = new Vector2(1, 0); // Set bullet direct to move forward
        Destroy(gameObject, bullet.lifespan);  // Destroy after a certain time
    }

    public void Update()
    {
        MoveBullet(); // Move the bullet in its direction
    }

    protected virtual void MoveBullet()
    {
        transform.Translate(direction * bullet.speed * Time.deltaTime);  // Move the bullet
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {


            other.gameObject.GetComponent<EnemyController>().TakeDamage(damage);

            float angle = transform.rotation.eulerAngles.z;
            float angleInRadians = angle * Mathf.Deg2Rad;
            Vector2 knockbackDirection = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
            other.gameObject.GetComponent<Rigidbody2D>().AddForce(other.gameObject.GetComponent<Rigidbody2D>().mass / 10 * knockbackDirection * knockback, ForceMode2D.Impulse);

            if (bullet.explosionPrefab)
            {
                Instantiate(bullet.explosionPrefab, (transform.position + other.transform.position) / 2, Quaternion.Euler(0, 0, angle));
            }
        }
    }
}
