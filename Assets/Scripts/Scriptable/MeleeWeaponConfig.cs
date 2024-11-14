using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Melee Weapon Config")]
public class MeleeWeaponConfig : ScriptableObject
{
    [Header("Weapon Setting")]
    public float damage = 10f;
    public float swingDelay = 0.3f;
    public float reflectCooldown = 5f;

    public float antiRecoil = 1f;
    public float antiRecoilTime = 0.1f;
    public float knockback = 1f;
    public float knockbackTime = 0.1f;
}
