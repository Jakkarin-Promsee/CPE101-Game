using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon Config")]
public class WeaponConfig : ScriptableObject
{
    public GameObject bulletPrefab;
    public float damage = 10f;
    public float fireRate = 0.2f;
    public float recoil = 1f;
    public float knockback = 1f;

}
