using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class EnemyActionController : MonoBehaviour
{
    // Referent setting
    public Transform player;
    ///////
    private enum State { Idle, Chase, Attack, Circle, Random, Dodge, retreat }
    private Rigidbody2D rb;
    private State _currentState;


    // Attack status
    [Header("General Setting. \n[eyeRange > attackRange > chaseRange]")]
    public float attackCD = 8f;
    public float eyeRange = 12f;
    public float attackRange = 10f;
    public float chaseRange = 8f;
    ///////
    private bool _nextAttackState = true;


    // Movemet status
    [Header("Movement Setting")]
    public float moveSpeed = 2.5f;
    public float moveFrictionCoefficient = 1f;
    public float moveLeastStoppingDistance = 0.1f;
    public float _wallCheckTime = 0.5f;
    ///////
    private bool _isCheckingWall = false;
    private Vector3 _lastThisPosition;
    private bool _nextMovementState = true;
    private bool _forceEnd = false;


    // Patrol Status
    [Header("PatrolSetting Setting")]
    public float patrolCD = 5f;
    public float patrolSpeed = 2f;
    public float patrolLength = 3f;
    ///////
    private bool _nextPatrolState = true;
    private bool _patrolGotoTaget = true;
    private bool _isPatrolFinish = true;
    private Vector3 _patrolSponPosition;
    private Vector3 _patrolTargetPosition;


    // Circle status
    [Header("Circle movement Setting. \n[chaseRange > circleRadius]")]
    public float circleCD = 9f;
    public float circleDuration = 1f;
    public float circleSpeed = 2f;
    public float circleAngularSpeed = 0.3f;
    public float circleRadius = 6f;
    ///////
    private bool _isCircleMove = false;
    private bool _nextCircleState = true;
    private bool _nextCircleDurationState = true;
    private float _circleFaceAngle = 0f;
    private float _circleOffsetRadius = 0f;
    private bool _isCircleCW = true;
    bool _isCircleReversePhase = false;
    private Vector3 _circleLastPlayerPosition;


    // Random move status
    [Header("Random movement Setting")]
    public float randomMoveCD = 3f;
    public float randomMoveSpeed = 2f;
    public float randomMoveLength = 2f;
    ///////
    private bool _isRandomMove = false;
    private bool _nextRandomMoveState = true;
    private bool _randomMoveGotoTaget = true;
    private bool _isRandomMoveFinish = true;
    private Vector3 _randomMoveTargetPosition;
    private Vector3 _randomMoveBasePosition;


    // Dodge status
    [Header("Dodge movement Setting")]
    public float dodgeCD = 10f;
    public float dodgeDuration = 0.4f;
    public float dodgeSpeed = 5f;
    public int dodgeAngle = 90;
    public float dodgeRange = 2f;
    ///////
    private bool _nextDodgeState = true;
    private bool _nextDodgeDurationState = true;
    private float _dodgeFaceAngle = 0f;
    private Vector3 _dodgeDirection;


    void Start()
    {
        _currentState = State.Idle;
        rb = GetComponent<Rigidbody2D>();
        _patrolSponPosition = transform.position;

        // Set CD count when first load enemy (Make each enemy has different action)
        StartCoroutine(CountMoveState(0f));
        StartCoroutine(CountAttackCD((float)Random.Range(0, 11) / 2));
        StartCoroutine(CountPatrolCD((float)Random.Range(0, 11) / 2));
        StartCoroutine(CountCircleCD((float)Random.Range(0, 11) / 2));
        StartCoroutine(CountRandomMoveCD((float)Random.Range(0, 5) / 5));
        StartCoroutine(CountDodgeCD((float)Random.Range(0, 11) / 2));
    }

    void Update()
    {
        ApplyFriction();

        switch (_currentState)
        {
            case State.Idle: Patrol(); CheckForPlayer(); break;
            case State.Chase: CheckAttack(); CheckForPlayer(); ChasePlayer(); break;
            case State.Attack: Attack(); break;
            case State.Circle: CheckAttack(); CircleMove(); CheckWall(); break;
            case State.Random: CheckAttack(); RandomMove(); CheckWall(); break;
            case State.Dodge: Dodge(); break;
        }
    }

    private void CheckWall()
    {
        if (!_isCheckingWall)
        {
            _isCheckingWall = true;
            StartCoroutine(WaitCheckWall());
        }
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

    public void TakeKnockback(Vector3 force, float knockbackTime)
    {
        StartCoroutine(CountMoveState(knockbackTime));
        float mass = gameObject.GetComponent<Rigidbody2D>().mass;
        gameObject.GetComponent<Rigidbody2D>().AddForce(force * mass, ForceMode2D.Impulse);
    }

    private IEnumerator CountMoveState(float _waitTime)
    {
        _nextMovementState = false;
        yield return new WaitForSeconds(_waitTime);
        _nextMovementState = true;
    }

    private IEnumerator WaitCheckWall()
    {
        _forceEnd = false;
        _lastThisPosition = transform.position;

        yield return new WaitForSeconds(_wallCheckTime);

        _isCheckingWall = false;
        if (_lastThisPosition == transform.position)
        {
            Debug.Log("Found Wall");
            _forceEnd = true;
        }
        else
            _forceEnd = false;
    }

    private IEnumerator CountAttackCD(float _cd)
    {
        _nextAttackState = false;
        yield return new WaitForSeconds(_cd);
        _nextAttackState = true;
    }

    private IEnumerator CountPatrolCD(float _cd)
    {
        _nextPatrolState = false;
        yield return new WaitForSeconds(_cd);
        _nextPatrolState = true;
    }

    private IEnumerator CountCircleCD(float _cd)
    {
        _nextCircleState = false;
        yield return new WaitForSeconds(_cd);
        _nextCircleState = true;
    }

    private IEnumerator CountCircleDurationCD(float _cd)
    {
        _nextCircleDurationState = false;
        yield return new WaitForSeconds(_cd);
        _nextCircleDurationState = true;
    }

    private IEnumerator CountRandomMoveCD(float _cd)
    {
        _nextRandomMoveState = false;
        yield return new WaitForSeconds(_cd);
        _nextRandomMoveState = true;
    }

    private IEnumerator CountDodgeCD(float _cd)
    {
        _nextDodgeState = false;
        yield return new WaitForSeconds(_cd);
        _nextDodgeState = true;
    }

    private IEnumerator CountDodgeDurationCD(float _cd)
    {
        _nextDodgeDurationState = false;
        yield return new WaitForSeconds(_cd);
        _nextDodgeDurationState = true;
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
            if (_nextMovementState)
                rb.velocity = (_patrolTargetPosition - transform.position).normalized * patrolSpeed;

            if (Vector3.Distance(transform.position, _patrolTargetPosition) < moveLeastStoppingDistance && _patrolGotoTaget)
            {
                _patrolGotoTaget = false;
                _patrolTargetPosition = _patrolSponPosition;
            }

            if ((Vector3.Distance(transform.position, _patrolSponPosition) < moveLeastStoppingDistance && !_patrolGotoTaget) || _forceEnd)
            {
                StartCoroutine(CountPatrolCD(patrolCD * ((float)Random.Range(5, 16) / 10)));
                _isPatrolFinish = true;
                _patrolGotoTaget = true;
            }
            CheckWall();
        }
    }

    private void Attack()
    {
        Debug.Log("pwefffff");

        _currentState = State.Chase;
    }

    private void CheckAttack()
    {
        if (_nextAttackState && Vector3.Distance(transform.position, player.position) < attackRange)
        {
            StartCoroutine(CountAttackCD(attackCD * ((float)Random.Range(5, 16) / 10)));
            Attack();
        }
    }

    private void ChasePlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Can't be interrupt by anything, make this function solitude
        if (_nextDodgeState && distanceToPlayer < dodgeRange)
        {
            DodgeInitialize();
            _currentState = State.Dodge;
            return;
        }

        // If other move funcitn doesn't work, check function move condition
        // Can be interrupt by attack phase or when player is out of range
        // Must balance between go to fixed circle radius direction and go to random direction
        // From the all, it make this function have complex complex condition
        if (distanceToPlayer < chaseRange)
        {
            if (!_isRandomMove)
            {
                if (_nextCircleState)
                {
                    CircleMoveInitialize();
                    _currentState = State.Circle;
                    return;
                }
                else if (_isCircleMove)
                {
                    CircleMove();
                    _currentState = State.Circle;
                    return;
                }
            }

            if (!_isCircleMove)
            {
                if (_nextRandomMoveState)
                {
                    RandomMoveInitialize();
                    _currentState = State.Random;
                    return;
                }
                else if (_isRandomMove)
                {
                    RandomMove();
                    _currentState = State.Random;
                    return;
                }
            }
        }

        // Keep moving towards to player untill chaseRange >= distance
        if (distanceToPlayer > chaseRange)
        {
            if (_nextMovementState)
                rb.velocity = (player.position - transform.position).normalized * moveSpeed;
        }
    }

    private void CircleMoveInitialize()
    {
        _nextCircleState = false;
        _isCircleMove = true;
        Debug.Log("Circle move begin");

        // Initial time manager control params
        StartCoroutine(CountCircleDurationCD(circleDuration * ((float)Random.Range(5, 16) / 10)));

        // Complex random
        _circleOffsetRadius = Random.Range(-15, 5) / 10 * circleRadius;
        _isCircleCW = (Random.Range(0, 2) == 0) ? true : false;
        if (chaseRange + _circleOffsetRadius > chaseRange) _circleOffsetRadius = chaseRange;

        // Set angle direction [angle that player aim to enemy]
        Vector2 distanceVector = transform.position - player.position;
        _circleFaceAngle = Mathf.Atan2(distanceVector.y, distanceVector.x);

        _circleLastPlayerPosition = player.position;
    }

    private void CircleMove()
    {
        // Calculate the next position on the orbit path
        if (_isCircleCW)
            _circleFaceAngle -= Time.fixedDeltaTime * circleAngularSpeed; // Update the angle over time
        else
            _circleFaceAngle += Time.fixedDeltaTime * circleAngularSpeed; // Update the angle over time

        float x = Mathf.Cos(_circleFaceAngle) * circleRadius;
        float y = Mathf.Sin(_circleFaceAngle) * circleRadius;

        // Determine the target position based on the player's position and angle
        Vector3 circlePosition = new Vector3(_circleLastPlayerPosition.x + x, _circleLastPlayerPosition.y + y, 0);

        // Calculate the velocity to move towards the orbit position
        Vector3 direction = (circlePosition - transform.position).normalized;

        if (_nextMovementState)
            rb.velocity = direction * circleSpeed;

        if (_nextCircleDurationState || _forceEnd)
        {
            if (!_isCircleReversePhase && !_forceEnd)
            {
                _isCircleReversePhase = true;
                _isCircleCW = !_isCircleCW;
                StartCoroutine(CountCircleDurationCD(circleDuration * ((float)Random.Range(5, 16) / 10)));
            }
            else
            {
                StartCoroutine(CountCircleCD(circleCD * ((float)Random.Range(5, 16) / 10)));
                _isCircleMove = false;
                _isCircleReversePhase = false;
                _currentState = State.Chase;
            }

        }
    }

    private void RandomMoveInitialize()
    {
        Debug.Log("Random move begin");

        _nextRandomMoveState = false;
        _isRandomMove = true;
        _isRandomMoveFinish = true;
    }

    private void RandomMove()
    {
        if (_isRandomMoveFinish)
        {
            _randomMoveBasePosition = transform.position;

            int moveDir = Random.Range(0, 4); // 0-3  -- 0=down 1=up 2=left 3=right
            float vectorx = Random.Range(6, 15) * randomMoveLength / 10;
            float vectory = Random.Range(-5, 5) * vectorx / 10;
            if (moveDir == 0) _randomMoveTargetPosition = _randomMoveBasePosition + new Vector3(vectory, -vectorx, 0);
            else if (moveDir == 1) _randomMoveTargetPosition = _randomMoveBasePosition + new Vector3(vectory, vectorx, 0);
            else if (moveDir == 2) _randomMoveTargetPosition = _randomMoveBasePosition + new Vector3(-vectorx, vectory, 0);
            else if (moveDir == 3) _randomMoveTargetPosition = _randomMoveBasePosition + new Vector3(vectorx, vectory, 0);

            _isRandomMoveFinish = false;
            _randomMoveGotoTaget = true;
        }
        else
        {
            if (_nextMovementState)
                rb.velocity = (_randomMoveTargetPosition - transform.position).normalized * randomMoveSpeed;

            if (Vector3.Distance(transform.position, _randomMoveTargetPosition) < moveLeastStoppingDistance && _randomMoveGotoTaget)
            {
                bool isback = Random.Range(0, 4) % 2 == 1;
                _randomMoveGotoTaget = false;
                if (isback) _randomMoveTargetPosition = (_randomMoveBasePosition + 3 * transform.position) / 4;
            }

            if ((Vector3.Distance(transform.position, _randomMoveTargetPosition) < moveLeastStoppingDistance && !_randomMoveGotoTaget) || _forceEnd)
            {
                StartCoroutine(CountRandomMoveCD(randomMoveCD * ((float)Random.Range(5, 16) / 10)));
                _isRandomMove = false;

                _currentState = State.Chase;
            }
        }
    }

    private void DodgeInitialize()
    {
        Debug.Log("dodge begin");
        StartCoroutine(CountDodgeDurationCD(dodgeDuration * ((float)Random.Range(5, 16) / 10)));

        // Set angle direction [angle that enemy aim to player]
        Vector2 distanceVector = player.position - transform.position;
        _dodgeFaceAngle = Mathf.Atan2(distanceVector.y, distanceVector.x);

        if (Random.Range(0, 2) == 0) _dodgeFaceAngle += dodgeAngle * Mathf.Deg2Rad;
        else _dodgeFaceAngle -= dodgeAngle * Mathf.Deg2Rad;

        float x = Mathf.Cos(_dodgeFaceAngle);
        float y = Mathf.Sin(_dodgeFaceAngle);

        // Determine the target position based on the player's position and angle
        _dodgeDirection = new Vector3(x, y, 0).normalized;
    }

    private void Dodge()
    {
        if (!_nextDodgeDurationState)
        {
            if (_nextMovementState)
                rb.velocity = _dodgeDirection * dodgeSpeed;
        }
        else
        {
            StartCoroutine(CountDodgeCD(dodgeCD * ((float)Random.Range(5, 16) / 10)));
            _nextCircleState = true;
            _currentState = State.Chase;
        }
    }
}