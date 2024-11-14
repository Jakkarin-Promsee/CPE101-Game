using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

public class Melee : MonoBehaviour
{
    public MeleeWeaponConfig weaponConfig;
    public GameObject weaponPivot;
    public GameObject player;

    private Collider2D meleeCollider;
    private Animator animator;
    public string weaponOwnerTag = "";
    private bool preventSwing = false;
    public bool isReflectingBullet = false;
    private SpriteRenderer spriteRenderer;


    private void Start()
    {
        animator = GetComponent<Animator>();
        meleeCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void Swing()
    {
        if (!preventSwing)
        {
            animator.SetTrigger("swordSwing");
            meleeCollider.enabled = true;
            StartCoroutine(DelaySwing());

            if (weaponOwnerTag == "Player")
            {
                Vector3 direction = weaponPivot.GetComponent<WeaponAim>().GetDefaultDirection();
                player.gameObject.GetComponent<PlayerMovement>().TakeKnockback(direction * weaponConfig.antiRecoil, weaponConfig.antiRecoilTime);
            }
            else if (weaponOwnerTag == "Enemy")
            {
                Vector3 direction = weaponPivot.GetComponent<EnemyWeaponAim>().GetDefaultDirection();
                player.gameObject.GetComponent<EnemyActionController>().TakeKnockback(direction * weaponConfig.antiRecoil, weaponConfig.antiRecoilTime);
            }
        }
    }

    // Setup reflect param
    public void Reflect()
    {
        StartCoroutine(CooldownReflect());
    }

    public IEnumerator DelaySwing()
    {
        preventSwing = true;

        yield return new WaitForSeconds(weaponConfig.swingDelay);

        meleeCollider.enabled = false;
        preventSwing = false;
    }

    public IEnumerator CooldownReflect()
    {
        isReflectingBullet = true;
        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(weaponConfig.reflectCooldown);

        isReflectingBullet = false;
        spriteRenderer.color = Color.black;
        print("Effect stops");
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet") && weaponOwnerTag != "Bullet")
        {
            if (isReflectingBullet)
            {
                // Change OwnerTag to Player
                other.GetComponent<Bullet>().weaponOwnerTag = "Player";

                // Calculate the angle direction to make bullet move to mouse position
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3 newDirection = (mousePosition - other.transform.position).normalized;
                float newMoveAngle = Mathf.Atan2(newDirection.y, newDirection.x) * Mathf.Rad2Deg;

                // Set new bullet angle
                other.GetComponent<Bullet>().ChangeMoveAngle(newMoveAngle);
            }
            else if (!other.CompareTag(weaponOwnerTag))
            {
                Destroy(other.gameObject);
            }
        }
        else if (other.CompareTag("Enemy") && weaponOwnerTag != "Enemy")
        {
            other.gameObject.GetComponent<EnemyController>().TakeDamage(weaponConfig.damage);

            // Calculate knockback, player is player, other is enemy
            Vector3 direction = (other.transform.position - player.transform.position).normalized;
            other.gameObject.GetComponent<EnemyActionController>().TakeKnockback(direction * weaponConfig.knockback, weaponConfig.knockbackTime);
        }
        else if (other.CompareTag("Player") && weaponOwnerTag != "Player")
        {
            // Calculate knockback, player is enemy, other is player
            Vector3 direction = (other.transform.position - player.transform.position).normalized;
            other.gameObject.GetComponent<PlayerMovement>().TakeKnockback(direction * weaponConfig.knockback, weaponConfig.knockbackTime);

            other.gameObject.GetComponent<PlayerController>().TakeDamage(weaponConfig.damage);
        }
    }
}
