using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public WeaponConfig weapon;
    public Transform gunPoint;
    public GameObject player;
    public float nextFireTime = 0f; // To handle fire rate

    public virtual void Fire()
    {
        if (Time.time > nextFireTime)
        {
            // Get mouse position in world space
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Calculate direction to the mouse
            Vector3 direction = (mousePosition - gunPoint.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Instantiate bullet and set its rotation
            GameObject bullet = Instantiate(weapon.bulletPrefab, gunPoint.position, Quaternion.Euler(0, 0, angle));
            bullet.GetComponent<Bullet>().damage = weapon.damage;
            bullet.GetComponent<Bullet>().knockback = weapon.knockback;

            // Add force to player
            player.GetComponent<PlayerMovement>().isforce = true;
            Vector3 recoilDirection = -direction;
            player.GetComponent<Rigidbody2D>().AddForce(recoilDirection * weapon.recoil, ForceMode2D.Impulse);

            nextFireTime = Time.time + weapon.fireRate;
        }
    }
}
