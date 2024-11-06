using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AkBullet : MonoBehaviour
{
    public GameObject bulletPrefab;   // Bullet prefab to spawn
    public Transform gunPoint;        // Point from where bullets are spawned
    public float fireRate = 2f;     // Time between shots

    private float nextFireTime = 0f;

    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Fire()
    {
        Debug.Log("fire");
        // Calculate direction towards the mouse position
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - (Vector2)gunPoint.position).normalized;

        // Instantiate bullet and set its direction
        GameObject bullet = Instantiate(bulletPrefab, gunPoint.position, Quaternion.identity);
        bullet.AddComponent<Bullet>();
        bullet.GetComponent<Bullet>().SetDirection(direction);
    }
}
