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
    public float nextFireTime = 0f;

    // ! Test
    private void Update() {
        // Normal attack
        if (Input.GetMouseButton(0)){
            Fire();
        }
    }
    // ! Test

    public virtual void Fire()
    {
        if (Time.time > nextFireTime)
        {
            nextFireTime = Time.time + weapon.fireRate;

            // Calculate direction to the mouse
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = (mousePosition - gunPoint.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Instantiate bullet and set its rotation
            GameObject bullet = Instantiate(weapon.bulletPrefab, gunPoint.position, Quaternion.Euler(0, 0, angle));
            bullet.GetComponent<Bullet>().damage = weapon.damage;
            bullet.GetComponent<Bullet>().knockback = weapon.knockback;
            bullet.GetComponent<Bullet>().knockbackTime = weapon.knockbackTime;

            // Add force to player
            Vector3 recoilDirection = -direction;
            player.GetComponent<PlayerMovement>().Recoil(recoilDirection * weapon.recoil, weapon.recoilTime);
        }
    }
}
