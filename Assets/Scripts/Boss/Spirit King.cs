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

    public float circleRadius = 3;
    public bool _forceFixedAtCircle = false;

    ///////////////////////////////////////
    private bool _forceEnd = false;
    private bool _nextMovementState = true;
    private float _circleFaceAngle = 0f;
    private float _circleOffsetRadius = 0f;
    private bool _isCircleCW = true;

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
            case State.Idle: Idle(); break;
            case State.ChaseCircle: ChaseCircle(); break;
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
        if (_nextNormalAttack)
        {
            _currentState = State.ChaseCircle;
            ChaseCircleInitial();
            StartCoroutine(CountNormalAttackCd());
        }

    }

    private void ChaseCircleInitial()
    {
        Debug.Log("Start target");

        // Complex random
        _circleOffsetRadius = (float)Random.Range(8, 15) / 10 * circleRadius;
        _isCircleCW = (Random.Range(0, 2) == 0) ? true : false;
        StartCoroutine(CountCircleCWCd());

        // Set angle direction [angle that player aim to enemy]
        Vector2 distanceVector = transform.position - player.position;
        _circleFaceAngle = Mathf.Atan2(distanceVector.y, distanceVector.x);
    }

    private float _chaseCircleCWCd = 2f;
    private bool _nextCircleCWChange = false;

    private IEnumerator CountCircleCWCd()
    {
        _nextCircleCWChange = false;
        yield return new WaitForSeconds(_chaseCircleCWCd);
        _nextCircleCWChange = true;
    }

    private float _normalAttackCd = 3f;
    private bool _nextNormalAttack = true;
    private IEnumerator CountNormalAttackCd()
    {
        _nextNormalAttack = false;
        yield return new WaitForSeconds(_normalAttackCd);
        _nextNormalAttack = true;
    }

    private void ChaseCircle()
    {
        // Calculate the next position on the orbit path
        if (_isCircleCW)
            _circleFaceAngle -= Time.fixedDeltaTime * circleAngularSpeed;
        else
            _circleFaceAngle += Time.fixedDeltaTime * circleAngularSpeed;
        Debug.Log(_circleFaceAngle);

        float x = Mathf.Cos(_circleFaceAngle) * _circleOffsetRadius;
        float y = Mathf.Sin(_circleFaceAngle) * _circleOffsetRadius;

        // Determine the target position based on the player's position and angle
        Vector3 circlePosition = new Vector3(player.position.x + x, player.position.y + y, 0);
        Vector3 direction = (circlePosition - transform.position).normalized;

        if (_nextMovementState)
            rb.velocity = direction * circleSpeed;

        if (_nextCircleCWChange)
        {
            StartCoroutine(CountCircleCWCd());
            _isCircleCW = !_isCircleCW;
        }

        if (Vector3.Distance(player.position, transform.position) < _circleOffsetRadius * 1.5 || _forceEnd)
        {
            Debug.Log("Arrived target");
            _currentState = State.Idle;
        }
    }

}
