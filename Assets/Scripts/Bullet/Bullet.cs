using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifespan = 5f;
    public GameObject explosionPrefab; // Reference to explosion prefab
    public Vector2 direction = new Vector2(1, 0);

    public virtual void Start()
    {
        Destroy(gameObject, lifespan);  // Destroy after a certain time
    }

    public void Update()
    {
        // Move the bullet in its direction
        MoveBullet();
    }

    protected virtual void MoveBullet()
    {
        transform.Translate(direction * speed * Time.deltaTime);  // Move the bullet
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);  // Destroy bullet on collision
    }
}
