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
    [Header("General Setting. \n[eyeRange > attackRange > chaseRange]")]
    public float eyeRange = 10f;
    public float attackRange = 8f;
    public float chaseRange = 5f;
    public float attackCD = 4f;
    ///////
    private float nextAttackTime = 0f;

    // Movemet status
    [Header("Movement Setting")]
    public float moveSpeed = 2.5f;
    public float frictionCoefficient = 1f;
    public float stoppingDistance = 0.1f;
    ///////
    private float nextMovement = 0f;

    // Patrol Status
    [Header("PatrolSetting Setting")]
    public float patrolSpeed = 1.5f;
    public float patrolLength = 2f;
    public float patrolCD = 3f;
    ///////
    private bool gotoTaget = true;
    private bool patrolFinish = true;
    private float nextPatrolTime = 0f;
    private Vector3 sponPosition;
    private Vector3 patrolTarget;

    // Circle status
    [Header("Circle movement Setting. \n[chaseRange > circleRadius]")]
    public float circleMoveSpeed = 2f;
    public float circleRadius = 3f;
    public float circleDuration = 2f;
    public float circleCD = 4f;
    ///////
    private bool isCircleMove = false;
    private float angleCircle = 0f;
    private float nextCircleTime = 0f;
    private float durationCircleTime = 0f;
    private float offsetCircleRadius = 0f;
    private bool randomCircleCW = true;

    // Random move status
    [Header("Random movement Setting")]
    public float randomMoveCD = 3f;
    public float randomMoveLength = 2f;
    public float randomMoveSpeed = 2f;
    ///////
    private bool isRandomMove = false;
    private float nextRandomMoveTime = 0f;
    private bool randomMoveGotoTaget = true;
    private bool randomMoveFinish = true;
    private Vector3 randomMoveTarget;
    private Vector3 randomMoveBase;
    private Vector3 lastPos;
    private float wallCheckTime = 0.5f;
    private float nextWallCheckTime = 0f;
    private bool isFirstRandomMove = true;

    // Dodge status
    [Header("Dodge movement Setting")]
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

        randomMoveTarget = transform.position;

        // Random activate time
        nextPatrolTime = Random.Range(0, 11) / 2;
        nextAttackTime = Random.Range(0, 11) / 2;
        nextCircleTime = Random.Range(0, 11) / 2;
        nextRandomMoveTime = Random.Range(0, 11) / 2;
        nextDodgeTime = Random.Range(0, 11) / 2;
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
            case State.Chase: CheckAttack(); CheckForPlayer(); ChasePlayer(); break;
            case State.Attack: Attack(); break;
            case State.Circle: CirclePlayer(); break;
            case State.Random: RandomMove(); break;
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
        rb.AddForce(rb.mass * frictionForce);
    }

    private void Patrol()
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
        Debug.Log("pwefffff");

        currentState = State.Chase;
    }

    private void CheckAttack()
    {
        if (Time.time > nextAttackTime && Vector3.Distance(transform.position, player.position) < attackRange)
        {
            nextAttackTime = Time.time + attackCD;
            Attack();
        }

    }

    private void ChasePlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (!isRandomMove && distanceToPlayer < chaseRange)
        {
            // If first active state
            if (Time.time > nextCircleTime)
            {
                Debug.Log("Circle move begin");

                // Initial time manager control params
                durationCircleTime = Time.time + circleDuration * Random.Range(7, 16) / 10;
                nextCircleTime = durationCircleTime + circleCD * Random.Range(7, 16) / 10;

                isCircleMove = true;

                // Complex random
                offsetCircleRadius = Random.Range(-15, 5) / 10 * circleRadius;
                randomCircleCW = (Random.Range(0, 2) == 0) ? true : false;
                if (chaseRange + offsetCircleRadius > chaseRange) offsetCircleRadius = chaseRange;

                // Set angle direction
                Vector2 distanceVector = transform.position - player.position;
                angleCircle = Mathf.Atan2(distanceVector.y, distanceVector.x);

                currentState = State.Circle;
                return;
            }
            // If in the circle movement duration
            else if (isCircleMove)
            {
                currentState = State.Circle;
                return;
            }
        }

        if (!isCircleMove && distanceToPlayer < chaseRange)
        {
            // If first active state
            if (Time.time > nextRandomMoveTime)
            {
                Debug.Log("Random move begin");

                nextRandomMoveTime = Time.time + randomMoveCD * Random.Range(7, 16) / 10;

                isRandomMove = true;
                randomMoveFinish = true;

                currentState = State.Random;
                return;

            }
            // If in the random movement duration
            else if (isRandomMove)
            {
                currentState = State.Random;
                return;
            }
        }

        if (Time.time > nextDodgeTime && distanceToPlayer < dodgeRange)
        {

        }

        if (distanceToPlayer > chaseRange)
        {
            // Keep moving towards player in chase state
            if (Time.time > nextMovement)
                rb.velocity = (player.position - transform.position).normalized * moveSpeed;
        }
    }


    bool isReverseCirclePhase = false;
    void CirclePlayer()
    {
        // Calculate the next position on the orbit path
        if (randomCircleCW)
            angleCircle -= Time.fixedDeltaTime / 3; // Update the angle over time
        else
            angleCircle += Time.fixedDeltaTime / 3; // Update the angle over time

        float x = Mathf.Cos(angleCircle) * circleRadius;
        float y = Mathf.Sin(angleCircle) * circleRadius;

        // Determine the target position based on the player's position and angle
        Vector3 circlePosition = new Vector3(player.position.x + x, player.position.y + y, 0);

        // Calculate the velocity to move towards the orbit position
        Vector3 direction = (circlePosition - transform.position).normalized;

        if (Time.time > nextMovement)
            rb.velocity = direction * circleMoveSpeed;

        if (Time.time > durationCircleTime)
        {
            if (!isReverseCirclePhase)
            {
                isReverseCirclePhase = true;
                randomCircleCW = !randomCircleCW;
                durationCircleTime = Time.time + circleDuration;
                nextCircleTime = Time.time + circleCD * Random.Range(7, 16) / 10;
            }
            else
            {
                nextCircleTime = Time.time + circleCD * Random.Range(7, 16) / 10;
                isCircleMove = false;
                isReverseCirclePhase = false;
                currentState = State.Chase;
            }

        }
    }
    private void RandomMove()
    {
        if (randomMoveFinish)
        {
            randomMoveBase = transform.position;

            int moveDir = Random.Range(0, 4); // 0-3  -- 0=down 1=up 2=left 3=right
            float vector = Random.Range(6, 15) * randomMoveLength / 10;
            if (moveDir == 0) randomMoveTarget = randomMoveBase + new Vector3(0, -vector, 0);
            else if (moveDir == 1) randomMoveTarget = randomMoveBase + new Vector3(0, vector, 0);
            else if (moveDir == 2) randomMoveTarget = randomMoveBase + new Vector3(-vector, 0, 0);
            else if (moveDir == 3) randomMoveTarget = randomMoveBase + new Vector3(vector, 0, 0);

            isFirstRandomMove = true;
            randomMoveFinish = false;
            randomMoveGotoTaget = true;
        }
        else
        {
            bool forceEnd = false;

            if (isFirstRandomMove)
            {
                isFirstRandomMove = false;
                nextWallCheckTime = Time.time + wallCheckTime;
                lastPos = transform.position;
            }
            else if (Time.time > nextWallCheckTime)
            {
                nextWallCheckTime = Time.time + wallCheckTime;

                if (lastPos == transform.position)
                {
                    Debug.Log("Found Wall");
                    forceEnd = true;
                }

                lastPos = transform.position;
            }

            if (Time.time > nextMovement)
                rb.velocity = (randomMoveTarget - transform.position).normalized * randomMoveSpeed;

            if (Vector3.Distance(transform.position, randomMoveTarget) < stoppingDistance && randomMoveGotoTaget)
            {
                randomMoveGotoTaget = false;
                randomMoveTarget = randomMoveBase;
            }

            if ((Vector3.Distance(transform.position, randomMoveTarget) < stoppingDistance && !randomMoveGotoTaget) || forceEnd)
            {
                nextRandomMoveTime = Time.time + randomMoveCD * Random.Range(5, 16) / 10;
                isRandomMove = false;

                currentState = State.Chase;
            }
        }
    }


}
