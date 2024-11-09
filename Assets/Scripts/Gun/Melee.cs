using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour
{
    Collider2D meleeCollider;
    Animator animator;
    public float delay = 0.3f;
    private bool attackBlocked = false;
    internal GameObject weaponPivot;    
    public float damage = 10f; // Temp damage

    private void Start() {
        animator = GetComponent<Animator>();
        meleeCollider = GetComponent<Collider2D>();
    }
    public void Swing(){
        if(attackBlocked) return;
        // Prevent another attack for some period of time
        animator.SetTrigger("swordSwing");
        meleeCollider.enabled = true;
        attackBlocked = true;
        StartCoroutine(DelaySwing());
    }

    public IEnumerator DelaySwing(){
        yield return new WaitForSeconds(delay);
        // Enable attack again after delay
        meleeCollider.enabled = false;
        attackBlocked = false;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other){
        if(other.tag == "Enemy"){
            print("Im dealing damage");
            other.gameObject.GetComponent<EnemyController>().TakeDamage(damage);
        }
    }
}
