using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour
{
    private Collider2D meleeCollider;
    Animator animator;
    public float swingDelay = 0.3f;
    private bool preventSwing = false;
    internal GameObject weaponPivot;
    public float damage = 10f; // Temp damage

    private void Start()
    {
        animator = GetComponent<Animator>();
        meleeCollider = GetComponent<Collider2D>();
    }

    // ! Test
    private void Update() {
        // Normal Attack
        if (Input.GetMouseButton(0)){
            Swing();
        }
    }
    // ! Test

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
        // If player's shield still exist, allow
        // Set blocking state
        // Play blocking animation
        // Change direction of incoming bullets to mouse position, and minus shield by 1
        // Start skill duration (3 secs)
        // Set skill cooldown(10 secs)
    }
    // ! Test

    private IEnumerator DelaySwing()
    {
        yield return new WaitForSeconds(swingDelay);
        // Enable attack again after Swing Delay
        meleeCollider.enabled = false;
        preventSwing = false;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            other.gameObject.GetComponent<EnemyController>().TakeDamage(damage);
        }
    }
}
