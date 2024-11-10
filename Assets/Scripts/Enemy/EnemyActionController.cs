using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class EnemyActionController : MonoBehaviour
{
    public Transform player;

    // Pattern status
    public enum State { Idle, Chase, Attack, Circle, Random, Dodge, retreat }
    ///////
    public State currentState;

    // Attack status
    public float eyeRange = 10f;
    public float chaseRange = 5f;
    public float attackRange = 8f;
    public float attackRate = 2f;
    ///////
    private float nextAttackTime = 0f;

    // Movemet status
    public float moveSpeed = 2.5f;
    public float frictionCoefficient = 1f;
    ///////
    private float nextMovement = 0f;

    // Patrol Status
    public float patrolSpeed = 1.5f;
    public float patrolLength = 2f;
    public float patrolCD = 3f;
    public float stoppingDistance = 0.1f;
    ///////
    private bool gotoTaget = true;
    private bool patrolFinish = true;
    private float nextPatrolTime = 0f;
    private Vector3 sponPosition;
    private Vector3 patrolTarget;

    // Circle status
    public float circleDistance = 3f; // Distance from the player
    public float circleMoveSpeed = 2f; // Speed of orbiting
    public float circleDuration = 2f;
    public float circleCD = 4f;
    ///////
    private float nextCircleTime = 0f;
    private float angle = 0f;
    private float nextCircleTimeEnd = 0f;
    private float CircleBufferAngle = 0;

    // Random move status
    public float randomMoveCD = 3f;
    ///////
    private float nextRandomMoveTime = 0f;

    // Dodge status
    public float dodgeRange = 2f;
    public float dodgeCD = 5f;
    ///////
    private float nextDodgeTime = 0f;

    // The others
    private Rigidbody2D rb;


    void Start()
    {
        currentState = State.Idle;
        rb = GetComponent<Rigidbody2D>();
        sponPosition = transform.position;

        // Random activate time
        nextPatrolTime = Random.Range(0, 11) / 5;
        nextAttackTime = Random.Range(0, 11) / 5;
        nextCircleTime = Random.Range(0, 11) / 5;
        nextRandomMoveTime = Random.Range(0, 11) / 5;
        nextDodgeTime = Random.Range(0, 11) / 5;
    }

    public void TakeKnockback(Vector3 force, float knockbackTime)
    {
        nextMovement = Time.time + knockbackTime;
        float mass = gameObject.GetComponent<Rigidbody2D>().mass;
        gameObject.GetComponent<Rigidbody2D>().AddForce(force * mass, ForceMode2D.Impulse);
    }

    void Update()
    {
        ApplyFriction();

        switch (currentState)
        {
            case State.Idle: Patrol(); CheckForPlayer(); break;
            case State.Chase: ChasePlayer(); break;
            case State.Attack: Attack(); break;
            case State.Circle: CirclePlayer(); break;
                // case State.Dodge: Dodge(); break;
        }
    }

    private void CheckForPlayer()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < eyeRange)
        {
            currentState = State.Chase;
        }
        else
        {
            currentState = State.Idle;
        }
    }

    private void ApplyFriction()
    {
        // Calculate the friction force
        Vector2 frictionForce = -rb.velocity * frictionCoefficient;

        // Apply friction force to the Rigidbody
        rb.AddForce(rb.mass * frictionForce);
    }


    void Patrol()
    {
        if (patrolFinish)
        {
            if (Time.time > nextPatrolTime)
            {
                int moveDir = Random.Range(0, 4); // 0-3  -- 0=down 1=up 2=left 3=right
                float vector = Random.Range(6, 10) * patrolLength / 10;
                if (moveDir == 0) patrolTarget = sponPosition + new Vector3(0, -vector, 0);
                else if (moveDir == 1) patrolTarget = sponPosition + new Vector3(0, vector, 0);
                else if (moveDir == 2) patrolTarget = sponPosition + new Vector3(-vector, 0, 0);
                else if (moveDir == 3) patrolTarget = sponPosition + new Vector3(vector, 0, 0);

                patrolFinish = false;
                gotoTaget = true;
            }
        }
        else
        {
            if (Time.time > nextMovement)
                rb.velocity = (patrolTarget - transform.position).normalized * patrolSpeed;

            if (Vector3.Distance(transform.position, patrolTarget) < stoppingDistance && gotoTaget)
            {
                gotoTaget = false;
                patrolTarget = sponPosition;
            }

            if (Vector3.Distance(transform.position, sponPosition) < stoppingDistance && !gotoTaget)
            {
                patrolFinish = true;
                gotoTaget = true;
                nextPatrolTime = Time.time + patrolCD * Random.Range(5, 16) / 10;
            }
        }
    }

    private void Attack()
    {
        nextAttackTime = Time.time + attackRate * Random.Range(8, 11) / 10;
        Debug.Log("pwefffff");

        currentState = State.Chase;
    }

    private void ChasePlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (Time.time > nextAttackTime && distanceToPlayer < attackRange)
        {
            currentState = State.Attack;
        }
        else if (Time.time > nextCircleTime && distanceToPlayer < chaseRange)
        {
            CircleBufferAngle = Random.Range(0, 360);
            nextCircleTimeEnd = Time.time + circleDuration;
            currentState = State.Circle;
        }
        else if (Time.time > nextRandomMoveTime && distanceToPlayer < chaseRange)
        {
            // currentState = State.Dodge;
        }
        else if (Time.time > nextDodgeTime && distanceToPlayer < dodgeRange)
        {

        }
        else if (distanceToPlayer > chaseRange)
        {
            // Keep moving towards player in chase state
            if (Time.time > nextMovement)
                rb.velocity = (player.position - transform.position).normalized * moveSpeed;
        }
    }

    void CirclePlayer()
    {
        if (Vector3.Distance(transform.position, player.position) < attackRange)
        {
            currentState = State.Attack;
        }

        // Calculate the next position on the orbit path
        angle += circleMoveSpeed * Time.fixedDeltaTime + CircleBufferAngle; // Update the angle over time
        float x = Mathf.Cos(angle) * circleDistance * Random.Range(5, 16) / 10;
        float y = Mathf.Sin(angle) * circleDistance * Random.Range(5, 16) / 10;

        // Determine the target position based on the player's position and angle
        Vector3 circlePosition = new Vector3(player.position.x + x, player.position.y + y, 0);

        // Calculate the velocity to move towards the orbit position
        Vector3 direction = (circlePosition - transform.position).normalized;

        if (Time.time > nextMovement)
            rb.velocity = new Vector2(direction.y, direction.x) * circleMoveSpeed;

        if (Time.time > nextCircleTimeEnd)
        {
            nextCircleTime = Time.time + circleCD * Random.Range(5, 16) / 10;
            currentState = State.Chase;
        }
    }


}
