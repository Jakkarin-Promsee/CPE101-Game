using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Status Config")]
public class EnemyStatusConfig : ScriptableObject
{
    // Customize referent setting
    [Header("General Setting. \n[eyeRange > attackRange > chaseRange]")]

    [Tooltip("Important!")]
    public GameObject weaponPrefab;
    [Tooltip("Not important.")]
    public GameObject bulletPrefab;
    [Tooltip("Not important.")]
    public RangeWeaponConfig rangeWeaponConfig;
    [Tooltip("Not important.")]
    public MeleeWeaponConfig meleeWeaponConfig;

    // Attack Status
    [Header("General Setting. \n[eyeRange > attackRange > chaseRange]")]
    public float attackCD = 8f;
    public float eyeRange = 12f;
    public float attackRange = 10f;
    public float chaseRange = 8f;

    // Movemet status
    [Header("Movement Setting")]
    public float moveSpeed = 2.5f;
    public float moveFrictionCoefficient = 1f;
    public float moveLeastStoppingDistance = 0.1f;
    public float wallCheckTime = 0.5f;

    // Patrol Status
    [Header("PatrolSetting Setting")]
    public float patrolCD = 5f;
    public float patrolSpeed = 1.5f;
    public float patrolLength = 2.5f;

    // Circle status
    [Header("Circle movement Setting. \n[chaseRange > circleRadius]")]
    public float circleCD = 9f;
    public float circleDuration = 1f;
    public float circleSpeed = 2f;
    public float circleAngularSpeed = 0.3f;
    public float circleRadius = 6f;

    // Random move status
    [Header("Random movement Setting")]
    public float randomMoveCD = 3f;
    public float randomMoveSpeed = 2.5f;
    public float randomMoveLength = 1f;

    // Dodge status
    [Header("Dodge movement Setting")]
    public float dodgeCD = 10f;
    public float dodgeDuration = 0.4f;
    public float dodgeSpeed = 5f;
    public int dodgeAngle = 90;
    public float dodgeRange = 2f;
}
