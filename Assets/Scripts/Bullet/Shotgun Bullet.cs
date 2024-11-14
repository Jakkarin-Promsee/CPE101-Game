using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShotgunBullet : Bullet
{
    public int pelletCount = 5;
    public float spreadAngle = 15f;
    public Vector3 pos;

    public override void Start()
    {
        base.Start();
    }

    public void FirePellets(RangeWeaponConfig _weaponConfig, string _weaponOwnerTag, Vector3 direction, float angle)
    {
        for (int i = 0; i < pelletCount; i++)
        {
            // Calculate rotation to face the mouse
            float buffer_angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float ran = Random.Range(-spreadAngle, spreadAngle);

            GameObject bullet = Instantiate(_weaponConfig.bulletPrefab, pos, Quaternion.Euler(0, 0, buffer_angle + angle + ran));
            bullet.GetComponent<Bullet>().weaponConfig = _weaponConfig;
            bullet.GetComponent<Bullet>().weaponOwnerTag = _weaponOwnerTag;
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
    }
}
