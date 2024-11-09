using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Old : MonoBehaviour
{
    public enum State { Idle, Patrol, Chase, Attack, Circle, Dodge }
    public State currentState;

    public Transform player;
    public float eyeRange = 10f;
    public float chaseRange = 5f;
    public float attackRange = 3f;
    public float attackRate = 2f;
    public float moveSpeed = 2.5f;
    public float patrolSpeed = 1.5f;
    public float circleRadius = 5f;
    public float dodgeDistance = 1.5f;
    public float dashSpeed = 5f;  // Dash speed
    public float dashInterval = 3f; // Time interval between dashes

    private Vector3 patrolTarget;
    private float nextAttackTime = 0f;
    private float dodgeTimer;
    private bool isDodging = false;
    private bool isDashing = false; // Flag to check if enemy is dashing
    private float lastDashTime = 0f; // Last time a dash occurred
    private Rigidbody2D rb;

    void Start()
    {
        currentState = State.Idle;
        rb = GetComponent<Rigidbody2D>(); // Get Rigidbody2D reference
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Idle: CheckForPlayer(); Patrol(); break;
            case State.Patrol: Patrol(); CheckForPlayer(); break;
            case State.Chase: ChasePlayer(); break;
            case State.Attack: Attack(); break;
            case State.Circle: CirclePlayer(); break;
            case State.Dodge: Dodge(); break;
        }
    }

    void CheckForPlayer()
    {
        if (Vector3.Distance(transform.position, player.position) < eyeRange)
        {
            currentState = State.Chase;
        }
    }

    void Patrol()
    {
        if (Vector3.Distance(transform.position, patrolTarget) < 0.2f)
        {
            patrolTarget = new Vector3(
                transform.position.x + Random.Range(-5f, 5f),
                transform.position.y + Random.Range(-5f, 5f),
                transform.position.z);
        }
        transform.position = Vector3.MoveTowards(transform.position, patrolTarget, patrolSpeed * Time.deltaTime);
    }

    void ChasePlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < attackRange)
        {
            currentState = State.Attack;
        }
        else if (distanceToPlayer < chaseRange / 2 && Random.value < 0.5f)
        {
            currentState = State.Circle;
        }
        else if (distanceToPlayer < chaseRange / 3 && Random.value < 0.3f)
        {
            currentState = State.Dodge;
        }
        else
        {
            // Keep moving towards player in chase state
            rb.velocity = (player.position - transform.position).normalized * moveSpeed;
        }
    }

    void CirclePlayer()
    {
        float angle = Mathf.Sin(Time.time) * circleRadius;
        Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * circleRadius;
        Vector3 circlePosition = player.position + offset;

        transform.position = Vector3.MoveTowards(transform.position, circlePosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, player.position) < attackRange)
        {
            currentState = State.Attack;
        }
        else if (Vector3.Distance(transform.position, player.position) > chaseRange)
        {
            currentState = State.Chase;
        }
    }

    void Dodge()
    {
        if (!isDodging)
        {
            Vector3 dodgeDirection = (transform.position - player.position).normalized;
            dodgeDirection += new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0f);
            Vector3 dodgePosition = transform.position + dodgeDirection * dodgeDistance;

            transform.position = Vector3.MoveTowards(transform.position, dodgePosition, moveSpeed * Time.deltaTime);
            isDodging = true;
            dodgeTimer = Time.time + 0.5f;
        }

        if (Time.time > dodgeTimer)
        {
            isDodging = false;
            currentState = State.Chase;
        }
    }

    void Attack()
    {
        // Stop movement during attack state but allow some random or dash movement
        rb.velocity = Vector2.zero;

        // Handle random movement or dash
        if (Time.time - lastDashTime >= dashInterval && !isDashing)
        {
            isDashing = true;
            lastDashTime = Time.time;
            DashTowardsPlayer();
        }
        else if (isDashing)
        {
            // Random movement
            RandomMovement();
            isDashing = false;
        }

        if (Time.time >= nextAttackTime)
        {
            Debug.Log("Enemy performs an attack!");
            nextAttackTime = Time.time + attackRate;

            Vector3 direction = (player.position - transform.position).normalized;
            ShootProjectile(direction);
        }

        // Check if player moves out of attack range, return to chase
        if (Vector3.Distance(transform.position, player.position) > attackRange)
        {
            currentState = State.Chase;
        }
    }

    void DashTowardsPlayer()
    {
        Vector3 dashDirection = (player.position - transform.position).normalized;
        rb.velocity = dashDirection * dashSpeed;
        Debug.Log("Enemy dashes towards player!");

        // Dash will automatically stop after a very short time. This is handled by the next frame when the velocity is reset to zero.
    }

    void RandomMovement()
    {
        // Randomly move in a small area
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f).normalized;
        transform.position += randomDirection * moveSpeed * Time.deltaTime;
        Debug.Log("Enemy moves randomly");
    }

    void ShootProjectile(Vector3 direction)
    {
        Debug.Log("Shooting projectile at player");
        // Add projectile instantiation here
    }
}