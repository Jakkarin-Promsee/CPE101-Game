using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f; // The bullet speed
    public float lifespan = 5f; // The bullet life time before automaticallu destroy
    public GameObject explosionPrefab; // The explosion prefab for this bullet


    private Vector2 direction;
    public float damage;
    public float knockback;

    public virtual void Start()
    {
        direction = new Vector2(1, 0); // Set bullet direct to move forward
        Destroy(gameObject, lifespan);  // Destroy after a certain time
    }

    public void Update()
    {
        MoveBullet(); // Move the bullet in its direction
    }

    protected virtual void MoveBullet()
    {
        transform.Translate(direction * speed * Time.deltaTime);  // Move the bullet
    }

    // For the Bullet script
    private void OnTriggerEnter2D(Collider2D other)
    {

        // if (other.CompareTag("Player")) Debug.Log("Touch player");
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Bullet hit the target!");
            other.gameObject.GetComponent<EnemyController>().hp -= damage;
            float angle = transform.rotation.eulerAngles.z;
            float angleInRadians = angle * Mathf.Deg2Rad;
            Vector2 knockbackDirection = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
            Debug.Log(angle);
            other.gameObject.GetComponent<Rigidbody2D>().AddForce(other.gameObject.GetComponent<Rigidbody2D>().mass / 10 * knockbackDirection * knockback, ForceMode2D.Impulse);

            // Handle damage or other logic here
            Destroy(gameObject);  // Destroy the bullet after collision
        }
    }
}
