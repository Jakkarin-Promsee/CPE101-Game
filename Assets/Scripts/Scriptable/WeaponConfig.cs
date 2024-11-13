using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon Config")]
public class WeaponConfig : ScriptableObject
{
    [Header("Weapon Setting")]
    public GameObject bulletPrefab;
    public float damage = 10f;
    public float fireRate = 0.2f;
    public float recoil = 1f;
    public float recoilTime = 0.1f;
    public float knockback = 1f;
    public float knockbackTime = 0.1f;

    [Header("Bullet Setting")]
    public bool isMoveThroughObject = false;
    public float speed = 10f;
    public float lifespan = 20f;
    public GameObject explosionPrefab;

}
