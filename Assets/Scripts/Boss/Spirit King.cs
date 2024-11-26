using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpiritKing : MonoBehaviour
{
    public Transform player;
    public float moveFrictionCoefficient = 1;


    // Circle status
    public float circleCD = 6;
    public float circleDuration = 2;
    public float circleSpeed = 3;
    public float circleAngularSpeed = 0.3f;

    public float circleRadius = 4;
    public bool _forceFixedAtCircle = false;

    ///////////////////////////////////////
    private bool _forceEnd = false;
    private bool _nextMovementState = true;
    private bool _isCircleMove = false;
    private bool _nextCircleState = true;
    public bool _nextCircleDurationState = true;
    private float _circleFaceAngle = 0f;
    private float _circleOffsetRadius = 0f;
    private bool _isCircleCW = true;
    bool _isCircleReversePhase = false;

    private enum State { Idle, ChaseCircle, ChaseStraight, Random };
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

        switch (_currentState)
        {
            case State.Idle: break;
            case State.ChaseCircle: CircleMove(); break;
        }
    }

    private void ApplyFriction()
    {
        // Calculate the friction force
        Vector2 frictionForce = -rb.velocity * moveFrictionCoefficient;
        rb.AddForce(rb.mass * frictionForce);
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

    private void Idle()
    {
        if (_nextCircleState)
        {
            CircleMoveInitialize();
            _currentState = State.ChaseCircle;
            return;
        }
        else if (_isCircleMove)
        {
            CircleMove();
            _currentState = State.ChaseCircle;
            return;
        }
    }

    private void CircleMoveInitialize()
    {
        Debug.Log("Circle Move Start");
        _nextCircleState = false;
        _isCircleMove = true;

        // Initial time manager control params
        StartCoroutine(CountCircleDurationCD(circleDuration * ((float)Random.Range(5, 16) / 10)));

        // Complex random
        _circleOffsetRadius = (float)Random.Range(8, 15) / 10 * circleRadius;
        Debug.Log(_circleOffsetRadius);
        _isCircleCW = (Random.Range(0, 2) == 0) ? true : false;

        // Set angle direction [angle that player aim to enemy]
        Vector2 distanceVector = transform.position - player.position;
        _circleFaceAngle = Mathf.Atan2(distanceVector.y, distanceVector.x);
    }

    private void CircleMove()
    {
        // Calculate the next position on the orbit path
        if (_isCircleCW)
            _circleFaceAngle -= Time.fixedDeltaTime * circleAngularSpeed; // Update the angle over time
        else
            _circleFaceAngle += Time.fixedDeltaTime * circleAngularSpeed; // Update the angle over time

        float x = Mathf.Cos(_circleFaceAngle) * _circleOffsetRadius;
        float y = Mathf.Sin(_circleFaceAngle) * _circleOffsetRadius;

        // Determine the target position based on the player's position and angle
        Vector3 circlePosition = new Vector3(player.position.x + x, player.position.y + y, 0);

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
                _currentState = State.Idle;
            }

        }
    }

}
