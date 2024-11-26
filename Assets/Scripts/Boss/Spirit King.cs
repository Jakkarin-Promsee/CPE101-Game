using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpiritKing : MonoBehaviour
{
    public Transform player;
    public float moveFrictionCoefficient = 1;


    private enum State { Idle, Chase, Random };
    private enum Attack { NormalAttack, Skill1, Skill2, Skill3, Skill4, Skill5, Skill6 }
    /*
        Idle => Rest Stand
        Chase => run to player
        NormalAttack => Fencing straight down with medium red area
        Skill1 => Quick Dash to player direction (less caution time)
        Skill2 => Flip Bullet Sword Technique => walk to player
        Skill3 => Mega Slice => large red area with continuous explosions on the ground
        Skill4 => Frequency Slice Sword Wave to player
        Skill5 => Sword Laser 540 anagle
        Skill6 => Summon black hole area attack
    */

    private State _currentState;
    private Attack _currentAttack;
    private Rigidbody2D rb;


    void Start()
    {
        this.player = GameObject.FindWithTag("Player").transform;

        _currentState = State.Idle;
        _currentAttack = Attack.NormalAttack;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        ApplyFriction();

        if (_isAttack)
        {
            switch (_currentAttack)
            {
                case Attack.NormalAttack: NormalAttack(); break;
            }
        }
        else
        {
            switch (_currentState)
            {
                case State.Idle: Idle(); break;
                case State.Chase: break;
            }
        }



    }

    private void ApplyFriction()
    {
        // Calculate the friction force
        Vector2 frictionForce = -rb.velocity * moveFrictionCoefficient;
        rb.AddForce(rb.mass * frictionForce);
    }

    private void Idle()
    {
        if (_nextNomalAttack)
        {
            normalAttackMoveSpeed = moveSpeed * 1.5f;
            NormalAttack();
        }
    }

    private void NormalAttack()
    {
        _currentAttack = Attack.NormalAttack;
        _isAttack = true;
        _nextNomalAttack = false;

        normalAttackMoveSpeed += Time.deltaTime / 3;

        if (!ChasePlayer(4f, normalAttackMoveSpeed))
            return;

        Debug.Log("I got you");
        _isAttack = false;
        StartCoroutine(CountNormalAttackCD());
    }

    IEnumerator CountNormalAttackCD()
    {
        _nextNomalAttack = false;
        yield return new WaitForSeconds(normalAttackCD);
        _nextNomalAttack = true;
    }


    public float moveSpeed = 3.5f;
    public float normalAttackCD = 2f;
    private float normalAttackMoveSpeed;

    private bool _isAttack = false;
    private bool _nextMovementState = true;
    private bool _nextNomalAttack = true;

    private bool ChasePlayer(float chaseLength, float speed)
    {

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > chaseLength)
        {
            if (_nextMovementState)
                rb.velocity = (player.position - transform.position).normalized * speed;
            return false;
        }
        return true;
    }
}