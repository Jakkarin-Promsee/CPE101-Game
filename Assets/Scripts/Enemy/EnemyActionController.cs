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
    private State _currentState;

    // Attack status
    [Header("General Setting. \n[eyeRange > attackRange > chaseRange]")]
    public float attackCD = 4f;
    public float eyeRange = 10f;
    public float attackRange = 8f;
    public float chaseRange = 5f;
    ///////
    private float _nextAttackTime = 0f;

    // Movemet status
    [Header("Movement Setting")]
    public float moveSpeed = 2.5f;
    public float moveFrictionCoefficient = 1f;
    public float moveLeastStoppingDistance = 0.1f;
    ///////
    private float _nextMovementTime = 0f;

    // Patrol Status
    [Header("PatrolSetting Setting")]
    public float patrolCD = 3f;
    public float patrolSpeed = 1.5f;
    public float patrolLength = 2f;
    ///////
    private bool _nextPatrolState = false;
    private float _nextPatrolTime = 0f;
    private bool _patrolGotoTaget = true;
    private bool _isPatrolFinish = true;
    private Vector3 _patrolSponPosition;
    private Vector3 _patrolTargetPosition;

    // Circle status
    [Header("Circle movement Setting. \n[chaseRange > circleRadius]")]
    public float circleCD = 4f;
    public float circleDuration = 2f;
    public float circleSpeed = 2f;
    public float circleAngularSpeed = 0.3f;
    public float circleRadius = 3f;
    ///////
    private bool _isCircleMove = false;
    private float _nextCircleTime = 0f;
    private float _circleFaceAngle = 0f;
    private float _nextCircleDurationTime = 0f;
    private float _circleOffsetRadius = 0f;
    private bool _isCircleCW = true;
    bool _isCircleReversePhase = false;

    // Random move status
    [Header("Random movement Setting")]
    public float randomMoveCD = 3f;
    public float randomMoveSpeed = 2f;
    public float randomMoveLength = 2f;
    ///////
    private bool _isRandomMove = false;
    private float _nextRandomMoveTime = 0f;
    private bool _randomMoveGotoTaget = true;
    private bool _isRandomMoveFinish = true;
    private Vector3 _randomMoveTargetPosition;
    private Vector3 _randomMoveBasePosition;
    private Vector3 _lastThisPosition;
    private float _wallCheckTime = 0.5f;
    private float _nextWallCheckTime = 0f;
    private bool _isFirstRandomMove = true;

    // Dodge status
    [Header("Dodge movement Setting")]
    public float dodgeCD = 5f;
    public float dodgeRange = 2f;

    ///////
    private float _nextDodgeTime = 0f;

    // The others
    private Rigidbody2D rb;


    void Start()
    {
        _currentState = State.Idle;
        rb = GetComponent<Rigidbody2D>();
        _patrolSponPosition = transform.position;

        _randomMoveTargetPosition = transform.position;

        // Random activate time
        // _nextPatrolTime = Random.Range(0, 11) / 2;
        _nextAttackTime = Random.Range(0, 11) / 2;
        _nextCircleTime = Random.Range(0, 11) / 2;
        _nextRandomMoveTime = Random.Range(0, 11) / 2;
        _nextDodgeTime = Random.Range(0, 11) / 2;

        StartCoroutine(CountPatrolCD((float)Random.Range(0, 11) / 2));
    }

    public void TakeKnockback(Vector3 force, float knockbackTime)
    {
        _nextMovementTime = Time.time + knockbackTime;
        float mass = gameObject.GetComponent<Rigidbody2D>().mass;
        gameObject.GetComponent<Rigidbody2D>().AddForce(force * mass, ForceMode2D.Impulse);
    }

    void Update()
    {
        ApplyFriction();

        switch (_currentState)
        {
            case State.Idle: Patrol(); CheckForPlayer(); break;
            case State.Chase: CheckAttack(); CheckForPlayer(); ChasePlayer(); break;
            case State.Attack: Attack(); break;
            case State.Circle: CirclePlayer(); break;
            case State.Random: RandomMove(); break;
                // case State.Dodge: Dodge(); break;
        }
    }

    private IEnumerator CountPatrolCD(float _cd)
    {
        _nextPatrolState = false;
        yield return new WaitForSeconds(_cd);
        _nextPatrolState = true;
    }

    private void CheckForPlayer()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < eyeRange)
        {
            _currentState = State.Chase;
        }
        else
        {
            _currentState = State.Idle;
        }
    }

    private void ApplyFriction()
    {
        // Calculate the friction force
        Vector2 frictionForce = -rb.velocity * moveFrictionCoefficient;
        rb.AddForce(rb.mass * frictionForce);
    }

    private void Patrol()
    {
        if (_isPatrolFinish)
        {
            if (_nextPatrolState)
            {
                Debug.Log("Patrol begin");
                _nextPatrolState = false;

                int moveDir = Random.Range(0, 4); // 0-3  -- 0=down 1=up 2=left 3=right
                float vector = Random.Range(6, 10) * patrolLength / 10;
                if (moveDir == 0) _patrolTargetPosition = _patrolSponPosition + new Vector3(0, -vector, 0);
                else if (moveDir == 1) _patrolTargetPosition = _patrolSponPosition + new Vector3(0, vector, 0);
                else if (moveDir == 2) _patrolTargetPosition = _patrolSponPosition + new Vector3(-vector, 0, 0);
                else if (moveDir == 3) _patrolTargetPosition = _patrolSponPosition + new Vector3(vector, 0, 0);

                _isPatrolFinish = false;
                _patrolGotoTaget = true;
            }
        }
        else
        {
            if (Time.time > _nextMovementTime)
                rb.velocity = (_patrolTargetPosition - transform.position).normalized * patrolSpeed;

            if (Vector3.Distance(transform.position, _patrolTargetPosition) < moveLeastStoppingDistance && _patrolGotoTaget)
            {
                _patrolGotoTaget = false;
                _patrolTargetPosition = _patrolSponPosition;
            }

            if (Vector3.Distance(transform.position, _patrolSponPosition) < moveLeastStoppingDistance && !_patrolGotoTaget)
            {
                StartCoroutine(CountPatrolCD(patrolCD));
                _isPatrolFinish = true;
                _patrolGotoTaget = true;
            }
        }
    }

    private void Attack()
    {
        Debug.Log("pwefffff");

        _currentState = State.Chase;
    }

    private void CheckAttack()
    {
        if (Time.time > _nextAttackTime && Vector3.Distance(transform.position, player.position) < attackRange)
        {
            _nextAttackTime = Time.time + attackCD;
            Attack();
        }

    }

    private void ChasePlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (!_isRandomMove && distanceToPlayer < chaseRange)
        {
            // If first active state
            if (Time.time > _nextCircleTime)
            {
                Debug.Log("Circle move begin");

                // Initial time manager control params
                _nextCircleDurationTime = Time.time + circleDuration * Random.Range(7, 16) / 10;
                _nextCircleTime = _nextCircleDurationTime + circleCD * Random.Range(7, 16) / 10;

                _isCircleMove = true;

                // Complex random
                _circleOffsetRadius = Random.Range(-15, 5) / 10 * circleRadius;
                _isCircleCW = (Random.Range(0, 2) == 0) ? true : false;
                if (chaseRange + _circleOffsetRadius > chaseRange) _circleOffsetRadius = chaseRange;

                // Set angle direction
                Vector2 distanceVector = transform.position - player.position;
                _circleFaceAngle = Mathf.Atan2(distanceVector.y, distanceVector.x);

                _currentState = State.Circle;
                return;
            }
            // If in the circle movement duration
            else if (_isCircleMove)
            {
                _currentState = State.Circle;
                return;
            }
        }

        if (!_isCircleMove && distanceToPlayer < chaseRange)
        {
            // If first active state
            if (Time.time > _nextRandomMoveTime)
            {
                Debug.Log("Random move begin");

                _nextRandomMoveTime = Time.time + randomMoveCD * Random.Range(7, 16) / 10;

                _isRandomMove = true;
                _isRandomMoveFinish = true;

                _currentState = State.Random;
                return;

            }
            // If in the random movement duration
            else if (_isRandomMove)
            {
                _currentState = State.Random;
                return;
            }
        }

        if (Time.time > _nextDodgeTime && distanceToPlayer < dodgeRange)
        {

        }

        if (distanceToPlayer > chaseRange)
        {
            // Keep moving towards player in chase state
            if (Time.time > _nextMovementTime)
                rb.velocity = (player.position - transform.position).normalized * moveSpeed;
        }
    }

    void CirclePlayer()
    {
        // Calculate the next position on the orbit path
        if (_isCircleCW)
            _circleFaceAngle -= Time.fixedDeltaTime * circleAngularSpeed; // Update the angle over time
        else
            _circleFaceAngle += Time.fixedDeltaTime / 3; // Update the angle over time

        float x = Mathf.Cos(_circleFaceAngle) * circleRadius;
        float y = Mathf.Sin(_circleFaceAngle) * circleRadius;

        // Determine the target position based on the player's position and angle
        Vector3 circlePosition = new Vector3(player.position.x + x, player.position.y + y, 0);

        // Calculate the velocity to move towards the orbit position
        Vector3 direction = (circlePosition - transform.position).normalized;

        if (Time.time > _nextMovementTime)
            rb.velocity = direction * circleSpeed;

        if (Time.time > _nextCircleDurationTime)
        {
            if (!_isCircleReversePhase)
            {
                _isCircleReversePhase = true;
                _isCircleCW = !_isCircleCW;
                _nextCircleDurationTime = Time.time + circleDuration;
                _nextCircleTime = Time.time + circleCD * Random.Range(7, 16) / 10;
            }
            else
            {
                _nextCircleTime = Time.time + circleCD * Random.Range(7, 16) / 10;
                _isCircleMove = false;
                _isCircleReversePhase = false;
                _currentState = State.Chase;
            }

        }
    }
    private void RandomMove()
    {
        if (_isRandomMoveFinish)
        {
            _randomMoveBasePosition = transform.position;

            int moveDir = Random.Range(0, 4); // 0-3  -- 0=down 1=up 2=left 3=right
            float vector = Random.Range(6, 15) * randomMoveLength / 10;
            if (moveDir == 0) _randomMoveTargetPosition = _randomMoveBasePosition + new Vector3(0, -vector, 0);
            else if (moveDir == 1) _randomMoveTargetPosition = _randomMoveBasePosition + new Vector3(0, vector, 0);
            else if (moveDir == 2) _randomMoveTargetPosition = _randomMoveBasePosition + new Vector3(-vector, 0, 0);
            else if (moveDir == 3) _randomMoveTargetPosition = _randomMoveBasePosition + new Vector3(vector, 0, 0);

            _isFirstRandomMove = true;
            _isRandomMoveFinish = false;
            _randomMoveGotoTaget = true;
        }
        else
        {
            bool forceEnd = false;

            if (_isFirstRandomMove)
            {
                _isFirstRandomMove = false;
                _nextWallCheckTime = Time.time + _wallCheckTime;
                _lastThisPosition = transform.position;
            }
            else if (Time.time > _nextWallCheckTime)
            {
                _nextWallCheckTime = Time.time + _wallCheckTime;

                if (_lastThisPosition == transform.position)
                {
                    Debug.Log("Found Wall");
                    forceEnd = true;
                }

                _lastThisPosition = transform.position;
            }

            if (Time.time > _nextMovementTime)
                rb.velocity = (_randomMoveTargetPosition - transform.position).normalized * randomMoveSpeed;

            if (Vector3.Distance(transform.position, _randomMoveTargetPosition) < moveLeastStoppingDistance && _randomMoveGotoTaget)
            {
                _randomMoveGotoTaget = false;
                _randomMoveTargetPosition = _randomMoveBasePosition;
            }

            if ((Vector3.Distance(transform.position, _randomMoveTargetPosition) < moveLeastStoppingDistance && !_randomMoveGotoTaget) || forceEnd)
            {
                _nextRandomMoveTime = Time.time + randomMoveCD * Random.Range(5, 16) / 10;
                _isRandomMove = false;

                _currentState = State.Chase;
            }
        }
    }


}
