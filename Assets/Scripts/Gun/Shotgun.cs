using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Gun
{
    public override void Fire(Vector3 targetPosition)
    {
        if (Time.time > base.nextFireTime)
        {
            ShotgunBullet shotgunBullet = weapon.bulletPrefab.GetComponent<ShotgunBullet>();
            shotgunBullet.pos = gunPoint.position;
            shotgunBullet.FirePellets(0);  // Fire multiple pellets
        }

        base.Fire(targetPosition);
    }
}
