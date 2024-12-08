using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : Gun
{
    public override void Fire(Vector3 targetPosition)
    {
        base.Fire(targetPosition);  // Pistol fire logic
    }
}
