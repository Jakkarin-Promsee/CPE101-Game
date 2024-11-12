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
    public string weaponOwnerTag = "";

    private void FireMethod(Vector3 targetPosition)
    {
        // Calculate direction to the mouse
        Vector3 direction = (targetPosition - gunPoint.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Instantiate bullet and set its rotation
        GameObject bullet = Instantiate(weapon.bulletPrefab, gunPoint.position, Quaternion.Euler(0, 0, angle));
        bullet.GetComponent<Bullet>().damage = weapon.damage;
        bullet.GetComponent<Bullet>().knockback = weapon.knockback;
        bullet.GetComponent<Bullet>().knockbackTime = weapon.knockbackTime;
        bullet.GetComponent<Bullet>().weaponOwnerTag = weaponOwnerTag;
    }

    public virtual void Fire(Vector3 targetPosition)
    {
        if (weaponOwnerTag == "Player")
        {
            if (Time.time > nextFireTime)
            {
                FireMethod(targetPosition);
                nextFireTime = Time.time + weapon.fireRate;

                // Add force to player
                Vector3 direction = (targetPosition - gunPoint.position).normalized;
                Vector3 recoilDirection = -direction;
                player.GetComponent<PlayerMovement>().Recoil(recoilDirection * weapon.recoil, weapon.recoilTime);
            }
        }
        else if (weaponOwnerTag == "Enemy")
        {
            FireMethod(targetPosition);
        }
    }
}
