using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

public class Melee : MonoBehaviour
{
    [Header("General Setting. Necessary to setup.")]
    public MeleeWeaponConfig weaponConfig;

    [Header("Itself Setting. Not necessary to setup.")]
    public GameObject weaponPivot;
    public GameObject player;

    private Collider2D meleeCollider;
    private Animator animator;
    public string weaponOwnerTag = "";
    public bool preventSwing = false;
    public bool isReflectingBullet = false;
    public PlayerController playerController;
    private SpriteRenderer spriteRenderer;


    private void Start()
    {
        animator = GetComponent<Animator>();
        meleeCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        playerController = player.GetComponent<PlayerController>();
    }

    public void Swing()
    {
        if (!preventSwing){
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

    public void Reflect(){
        // Prevent player from using reflect if shield is <= 0
        if(playerController.shield <= 0) return;

        animator.SetBool("swordReflect", true);
    }

    public IEnumerator DelaySwing()
    {
        preventSwing = true;

        yield return new WaitForSeconds(weaponConfig.swingDelay);

        meleeCollider.enabled = false;
        preventSwing = false;
    }

    // This function will be call at the start of animation
    public void StartReflecting(){
        preventSwing = true;
        isReflectingBullet = true;
        meleeCollider.enabled = true;
        spriteRenderer.color = Color.red;
    }

    // This function will be call at the end of animation
    public void StopReflecting(){
        preventSwing = false;
        isReflectingBullet = false;
        meleeCollider.enabled = false;
        animator.SetBool("swordReflect", false);
        spriteRenderer.color = Color.black;
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
                other.GetComponent<Bullet>().Reflect(newMoveAngle);

                // Reduce player shield by damage amount
                playerController.shield -= weaponConfig.damage;

                // Stop reflecting if shield after collision is 0
                if(playerController.shield <= 0)
                    StopReflecting();
            }
            else if (!other.CompareTag(weaponOwnerTag))
            {
                Destroy(other.gameObject);
            }
        }
        else if (other.CompareTag("Enemy") && weaponOwnerTag != "Enemy" && !isReflectingBullet)
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
