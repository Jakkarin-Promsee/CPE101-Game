using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class SpiritKing : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    private NavMeshAgent agent;

    [Header("General Setting")]
    public float moveFrictionCoefficient = 1;
    public float moveSpeed = 3.5f;

    // Action State Controller Variables
    [SerializeField] private State currentState;
    [SerializeField] private Attack currentAttack;
    private bool nextMovementState = true;


    // Normal Attack 
    [Header("Normal Attack Setting")]
    public GameObject normalAttackWarningAreaPrefab;
    public float normalAttackCD = 2f;
    public float normalAttackDelay = 0.75f;
    public float normalAttackDamage = 30f;
    public float normalAttackKnockback = 13f;
    public float normalAttackWarningDuration = 2f;
    public float normalAttackWarningBlinkInterval = 0.3f;
    public float normalAttacklengthOffset = 0f;
    private float normalAttackMoveSpeed;
    private bool nextNomalAttack = true;

    // Normal Attack 
    [Header("Skill 1 Setting")]
    public GameObject skill1WarningAreaPrefab;
    public float skill1CD = 5f;
    public float skill1Delay = 1.5f;
    public float skill1Damage = 40f;
    public float skill1Knockback = 13f;
    public float skill1WarningDuration = 2f;
    public float skill1WarningBlinkInterval = 0.3f;
    public float skill1DashSpeed = 4f;
    public float skill1lengthOffset = 0f;

    // Handle Warning Area
    [Header("Warning Area Settings")]
    public Color initialColor = Color.white;
    public Color blinkColor = Color.red;
    private GameObject currentWarningArea;
    private List<GameObject> playerInZone;
    private bool isCreateWarningArea = false;
    private bool isWarningAreaDone = false;

    [Header("Raycast Setting")]
    public LayerMask wallLayer;
    public LayerMask playerLayer;

    // Other
    private Rigidbody2D rb;

    // State Set
    private enum State { Idle, Chase, Attack };
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
    private enum NormalAttackState { Initial, Chase, Warning, Attack, CoolDown };
    private enum Skill1State { Initial, Warning, Drawline, Attack, CoolDown };

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        currentState = State.Idle;
        currentAttack = Attack.NormalAttack;
        rb = GetComponent<Rigidbody2D>();
    }

    [SerializeField] private NormalAttackState normalAttackState = NormalAttackState.Initial;

    void Update()
    {
        ApplyFriction();

        switch (currentState)
        {
            case State.Idle:
                Idle();
                break;

            case State.Attack:
                switch (currentAttack)
                {
                    case Attack.NormalAttack:
                        NormalAttackController();
                        break;

                    case Attack.Skill1:
                        Skill1Controller();
                        break;

                    default:
                        break;
                }
                break;
        }
    }

    private float skill1Angle;
    private bool isSkill1Dashing = false;

    private Skill1State skill1State = Skill1State.Initial;

    public LineRenderer lineRenderer;
    Vector2 skill1DashTarget;
    float skill1ElapsedTime;
    private void Skill1DrawLine(Vector2 dashTarget)
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2; // Start and end points
        lineRenderer.SetPosition(0, transform.position); // Starting point
        lineRenderer.SetPosition(1, dashTarget); // Endpoint
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        // lineRenderer.startWidth = 0.1f;
        // lineRenderer.endWidth = 0.1f;

        // Disable the LineRenderer after 0.5 seconds
        StartCoroutine(DisableLineRenderer(lineRenderer));
    }

    private IEnumerator DisableLineRenderer(LineRenderer lineRenderer)
    {
        lineRenderer.enabled = true;
        yield return new WaitForSeconds(0.5f);
        lineRenderer.enabled = false;
    }

    public void Skill1Controller()
    {
        switch (skill1State)
        {
            case Skill1State.Initial:
                currentAttack = Attack.Skill1;
                skill1State = Skill1State.Warning;
                nextSkill1 = false;
                break;

            case Skill1State.Warning:
                if (!isCreateWarningArea)
                {
                    isCreateWarningArea = true;

                    Vector3 direction = (player.transform.position - transform.position).normalized;
                    Vector3 offset = direction * ((skill1WarningAreaPrefab.GetComponent<Transform>().localScale.x / 2) + skill1lengthOffset);
                    Vector3 spawnPosition = transform.position + offset;
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                    // Instantiate the warning area
                    currentWarningArea = Instantiate(skill1WarningAreaPrefab, spawnPosition, Quaternion.Euler(0, 0, angle), transform);
                    currentWarningArea.AddComponent<AreaAttack>();

                    StartCoroutine(DelayMoveAngle(currentWarningArea, skill1Delay, skill1lengthOffset));
                    StartCoroutine(BlinkWarningArea(currentWarningArea, skill1WarningDuration, skill1WarningBlinkInterval));
                }
                else if (isWarningAreaDone)
                {
                    skill1Angle = currentWarningArea.GetComponent<Transform>().eulerAngles.z;
                    playerInZone = new List<GameObject>(currentWarningArea.GetComponent<AreaAttack>().playersInZone);

                    Destroy(currentWarningArea);
                    isCreateWarningArea = false;

                    skill1State = Skill1State.Attack;
                }
                break;


            case Skill1State.Attack:


                if (!isSkill1Dashing)
                {
                    float radians = skill1Angle * Mathf.Deg2Rad;
                    Vector2 dashDirection = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)).normalized;


                    RaycastHit2D hit = Physics2D.Raycast(transform.position, dashDirection, Mathf.Infinity, wallLayer);
                    if (hit.collider != null)
                    {
                        skill1DashTarget = hit.point;
                    }
                    else
                    {
                        // Safety fallback in case no wall is hit (optional)
                        skill1DashTarget = (Vector2)transform.position + dashDirection * 50f;
                    }

                    RaycastHit2D playerHit = Physics2D.Raycast(transform.position, dashDirection, Vector2.Distance(transform.position, skill1DashTarget), playerLayer);
                    if (playerHit.collider != null)
                    {
                        // Damage the player
                        PlayerController playerHealth = playerHit.collider.GetComponent<PlayerController>();
                        if (playerHealth != null)
                        {
                            playerHealth.TakeDamage(skill1Damage);
                        }
                    }

                    skill1ElapsedTime = 0;
                    isSkill1Dashing = true;
                }
                else
                {
                    if (skill1ElapsedTime < 1 || Vector3.Distance(transform.position, player.position) < 0.5)
                    {
                        rb.velocity = ((Vector3)skill1DashTarget - transform.position).normalized * 30;
                        skill1ElapsedTime += Time.deltaTime;
                    }

                    else
                    {
                        isSkill1Dashing = false;
                        skill1State = Skill1State.CoolDown;
                    }
                }
                break;

            case Skill1State.CoolDown:
                currentState = State.Idle;
                skill1State = Skill1State.Initial;
                StartCoroutine(CountSkill1CD());
                break;
        }
    }

    IEnumerator BlinkWarningArea(GameObject _warningArea, float warningDuration, float warningBlinkInterval)
    {
        isWarningAreaDone = false;

        float elapsedTime = 0f;
        SpriteRenderer warningSprite = _warningArea.GetComponent<SpriteRenderer>();
        while (elapsedTime < warningDuration)
        {
            warningSprite.color = initialColor;
            yield return new WaitForSeconds(warningBlinkInterval);
            warningSprite.color = blinkColor;
            yield return new WaitForSeconds(warningBlinkInterval);
            elapsedTime += warningBlinkInterval * 2;
        }

        isWarningAreaDone = true;
    }

    IEnumerator DelayMoveAngle(GameObject _warningArea, float delayAngle, float lengthOffset)
    {
        float elapsedTime = 0f;

        while (elapsedTime < delayAngle)
        {
            Vector3 direction = (player.transform.position - transform.position).normalized;
            Vector3 offset = direction * ((_warningArea.GetComponent<Transform>().localScale.x / 2) + lengthOffset);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            _warningArea.transform.position = transform.position + offset;
            _warningArea.transform.rotation = Quaternion.Euler(0, 0, angle);

            yield return new WaitForSeconds(0.05f);
            elapsedTime += 0.05f;
        }
    }

    private void NormalAttackController()
    {
        switch (normalAttackState)
        {
            case NormalAttackState.Initial:
                normalAttackMoveSpeed = moveSpeed * 1.5f;
                currentAttack = Attack.NormalAttack;
                normalAttackState = NormalAttackState.Chase;
                nextNomalAttack = false;
                break;

            case NormalAttackState.Chase:
                if (ChasePlayer(4f, normalAttackMoveSpeed))
                {
                    normalAttackMoveSpeed += Time.deltaTime / 2;
                    normalAttackState = NormalAttackState.Warning;
                }
                break;

            case NormalAttackState.Warning:
                if (!isCreateWarningArea)
                {
                    isCreateWarningArea = true;

                    Vector3 direction = (player.transform.position - transform.position).normalized;
                    Vector3 offset = direction * ((normalAttackWarningAreaPrefab.GetComponent<Transform>().localScale.x / 2) + normalAttacklengthOffset);
                    Vector3 spawnPosition = transform.position + offset;
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                    // Instantiate the warning area
                    currentWarningArea = Instantiate(normalAttackWarningAreaPrefab, spawnPosition, Quaternion.Euler(0, 0, angle), transform);
                    currentWarningArea.AddComponent<AreaAttack>();

                    StartCoroutine(DelayMoveAngle(currentWarningArea, normalAttackDelay, normalAttacklengthOffset));
                    StartCoroutine(BlinkWarningArea(currentWarningArea, normalAttackWarningDuration, normalAttackWarningBlinkInterval));
                }
                else if (isWarningAreaDone)
                {
                    playerInZone = new List<GameObject>(currentWarningArea.GetComponent<AreaAttack>().playersInZone);

                    Destroy(currentWarningArea);
                    isCreateWarningArea = false;

                    normalAttackState = NormalAttackState.Attack;
                }
                break;

            case NormalAttackState.Attack:
                foreach (GameObject player in playerInZone)
                {
                    player.GetComponent<PlayerMovement>().TakeKnockback((player.transform.position - transform.position).normalized * normalAttackKnockback, 0.3f);
                    player.GetComponent<PlayerController>().TakeDamage(normalAttackDamage);
                }
                normalAttackState = NormalAttackState.CoolDown;
                break;

            case NormalAttackState.CoolDown:
                currentState = State.Idle;
                normalAttackState = NormalAttackState.Initial;
                normalAttackMoveSpeed = moveSpeed;
                StartCoroutine(CountNormalAttackCD());
                break;
        }
    }

    private void ApplyFriction()
    {
        // Calculate the friction force
        Vector2 frictionForce = -rb.velocity * moveFrictionCoefficient;
        rb.AddForce(rb.mass * frictionForce);
    }

    private bool nextSkill1 = true;

    private void Idle()
    {
        if (nextSkill1)
        {
            currentState = State.Attack;
            currentAttack = Attack.Skill1;
            return;
        }


        if (nextNomalAttack)
        {
            currentState = State.Attack;
            currentAttack = Attack.NormalAttack;
            return;
        }

    }

    IEnumerator CountNormalAttackCD()
    {
        nextNomalAttack = false;
        yield return new WaitForSeconds(normalAttackCD);
        nextNomalAttack = true;
    }

    IEnumerator CountSkill1CD()
    {
        nextSkill1 = false;
        yield return new WaitForSeconds(skill1CD);
        nextSkill1 = true;
    }

    private bool ChasePlayer(float chaseLength, float speed)
    {

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > chaseLength)
        {
            if (nextMovementState)
                agent.SetDestination(player.position);
            agent.speed = speed;

            // rb.velocity = (player.position - transform.position).normalized * speed;
            return false;
        }
        agent.speed = 0;
        return true;
    }
}