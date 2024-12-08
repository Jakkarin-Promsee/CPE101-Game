using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultGun : Gun
{
    public override void Fire(Vector3 targetPosition)
    {
        base.Fire(targetPosition);  // Pistol fire logic
    }
}
