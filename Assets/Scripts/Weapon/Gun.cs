using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("General Setting. Necessary to setup.")]
    public RangeWeaponConfig weaponConfig;
    public Transform gunPoint;

    [Header("Itself Setting. Not necessary to setup.")]
    public GameObject player;
    public float nextFireTime = 0f;
    public string weaponOwnerTag = "";

    private void FireMethod(Vector3 targetPosition)
    {
        // Calculate direction to the mouse
        Vector3 direction = (targetPosition - gunPoint.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Instantiate bullet and set its rotation
        GameObject bullet = Instantiate(weaponConfig.bulletPrefab, gunPoint.position, Quaternion.Euler(0, 0, angle));
        bullet.GetComponent<Bullet>().weaponConfig = weaponConfig;
        bullet.GetComponent<Bullet>().weaponOwnerTag = weaponOwnerTag;
    }

    public virtual void Fire(Vector3 targetPosition)
    {
        if (weaponOwnerTag == "Player")
        {
            if (Time.time > nextFireTime)
            {
                FireMethod(targetPosition);
                nextFireTime = Time.time + weaponConfig.fireRate;

                // Add force to player
                Vector3 direction = (targetPosition - gunPoint.position).normalized;
                Vector3 recoilDirection = -direction;
                player.GetComponent<PlayerMovement>().TakeKnockback(recoilDirection * weaponConfig.recoil, weaponConfig.recoilTime);
            }
        }
        else if (weaponOwnerTag == "Enemy")
        {
            FireMethod(targetPosition);
        }
    }
}
