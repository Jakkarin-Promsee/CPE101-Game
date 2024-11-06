using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject bulletPrefab;  // The bullet prefab for this gun
    public Transform gunPoint;       // The gunpoint where bullets will be fired
    public float fireRate = 0.2f;    // Time between shots
    public float nextFireTime = 0f; // To handle fire rate

    public virtual void Fire()
    {
        if (Time.time > nextFireTime)
        {
            // Calculate direction towards the mouse position
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f; // Ensure the z-axis is zero to work in 2D space

            Vector3 direction = (mousePosition - gunPoint.position).normalized;

            // Calculate rotation to face the mouse
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Instantiate bullet and set its rotation
            Instantiate(bulletPrefab, gunPoint.position, Quaternion.Euler(0, 0, angle));

            nextFireTime = Time.time + fireRate;
        }
    }
}
