using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

public class Melee : MonoBehaviour
{
    Collider2D meleeCollider;
    Animator animator;
    public float swingDelay = 0.3f;
    public float reflectCooldown = 5f;
    internal GameObject weaponPivot;
    public float damage = 10f;
    public string weaponOwnerTag = "";
    private bool preventSwing = false;
    public bool isReflectingBullet = false;

    // ! Test
    private SpriteRenderer spriteRenderer;
    private Vector2 reflectDirection;
    // ! Test

    private void Start()
    {
        animator = GetComponent<Animator>();
        meleeCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void Swing()
    {
        if (preventSwing) return;
        // Prevent another attack for some period of time
        animator.SetTrigger("swordSwing");
        meleeCollider.enabled = true;
        preventSwing = true;
        StartCoroutine(DelaySwing());
    }

    // ! Test
    public void Reflect(){
        if(isReflectingBullet) {
            print("Effect's happening right now!");
            return;
        }
        isReflectingBullet = true;
        spriteRenderer.color = Color.red;
        StartCoroutine(CooldownReflect());
    }
    // ! Test

    public IEnumerator DelaySwing()
    {
        yield return new WaitForSeconds(swingDelay);
        // Enable attack again after delay
        meleeCollider.enabled = false;
        preventSwing = false;
    }

    public IEnumerator CooldownReflect(){
        yield return new WaitForSeconds(reflectCooldown);
        spriteRenderer.color = Color.black;
        isReflectingBullet = false;
        print("Effect stops");
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet") && weaponOwnerTag != "Bullet")
        {
            if (isReflectingBullet)
            {
                // ! Test
                // Change OwnerTag to Player
                other.GetComponent<DefaultBullet>().weaponOwnerTag = "Player";

                // Calculate the direction from the object to the mouse position
                GameObject incomingBullet = other.gameObject;
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                reflectDirection = (mousePosition - (Vector2)incomingBullet.transform.position).normalized;

                Bullet bulletMovement = incomingBullet.GetComponent<Bullet>();
                bulletMovement.ChangeDirection(reflectDirection); // Assuming you have a ChangeDirection method
                // ! Test
            }
            else if (!other.CompareTag(weaponOwnerTag))
            {
                Destroy(other.gameObject);
            }
        }
        else if (other.CompareTag("Enemy") && weaponOwnerTag != "Enemy")
        {
            other.gameObject.GetComponent<EnemyController>().TakeDamage(damage);
        }
        else if (other.CompareTag("Player") && weaponOwnerTag != "Player")
        {
            Debug.Log("I hit player lol");
            other.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
        }
    }
}
