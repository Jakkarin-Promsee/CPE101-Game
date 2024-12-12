using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TestTools;
using UnityEngine.Tilemaps;

public class SpiritKing : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Attack testSkill;
    [SerializeField] private Transform player;
    public GameObject playerCamera;
    private NavMeshAgent agent;
    public Tilemap groundTilemap;
    public Tilemap wallTilemap;
    public Tilemap obstacleTilemap;


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
    public float normalAttackWait = 1f;
    public float normalAttackDelayDuration = 0.75f;
    public float normalAttackDamage = 30f;
    public float normalAttackKnockback = 13f;
    public float normalAttackWarningDuration = 2f;
    public float normalAttackWarningBlinkInterval = 0.3f;
    public float normalAttacklengthOffset = 0f;
    public float normalAttackShakeDuration = 0.5f;
    public float normalAttackShakeMagnitude = 1f;
    private float normalAttackMoveSpeed;
    private bool nextNomalAttack = true;

    // Normal Attack 
    [Header("Skill 1 Setting")]
    public float skill1CD = 5f;
    public float skill1Wait = 1f;
    public float skill1WarningDuration = 2f;
    public float skill1WarningBlinkInterval = 0.3f;
    public float skill1DelayDuration = 1.5f;
    public float skill1DashDuration = 1f;
    public float skill1Damage = 40f;
    public float skill1DashSpeed = 30f;
    public float skill1Knockback = 20f;
    public float skill1lengthOffset = 0f;
    public float skill1ShakeDuration = 1.5f;
    public float skill1ShakeMagnitude = 2f;
    public GameObject skill1WarningAreaPrefab;
    public GameObject skill1AreaAttackPrefab;
    private bool nextSkill1 = true;
    private bool skill1isDashing = false;
    private bool skill1isHitWall = false;

    // Skill 2
    [Header("Skill 2 Settings")]
    public float skill2CD = 5f;
    public float skill2Wait = 1f;
    public float skill2WarningDuration = 2f;
    public float skill2WarningBlinkInterval = 0.3f;
    public float skill2DelayDuration = 1.5f;
    public float skill2Damage = 40f;
    public float skill2Knockback = 20f;
    public GameObject skill2WarningAreaLine;
    public GameObject skill2AttackLine;
    public Color skill2BlinkColor = Color.white;
    public float skill2WarningAreaTransparent = 0.5f;
    public float skill2ShakeDuration = 2f;
    public float skill2ShakeMagnitude = 1f;
    private bool nextSkill2 = true;


    // Skill 3
    [Header("Skill3 Settings")]
    public GameObject skill3WarningAreaPrefab;
    public float skill3CD = 2f;
    public float skill3Wait = 1f;
    public float skill3DelayDuration = 0.75f;
    public float skill3Damage = 30f;
    public float skill3Knockback = 13f;
    public float skill3WarningDuration = 2f;
    public float skill3WarningBlinkInterval = 0.3f;
    public float skill3lengthOffset = 0f;
    private float skill3MoveSpeed;
    public float skill3HitKnockBack = 15f;
    public float skill3ShakeDuration = 2f;
    public float skill3ShakeMagnitude = 2f;
    private bool nextSkill3 = true;

    // Skill 4
    [Header("Skill4 Setting")]
    public GameObject skill4MeteorPrefab;
    public GameObject skill4WarningAreaPrefab;
    public GameObject skill4AreaAttackPrefab;
    public int skill2spawnAmount = 5;
    public float skill4CD = 2f;
    public float skill4Wait = 1f;
    public float skill4WarningDuration = 2f;
    public float skill4WarningBlinkInterval = 0.3f;
    public float skill4AreaAttackDuration = 15f;
    public float skill4ShakeDuration = 4f;
    public float skill4ShakeMagnitude = 2f;
    private bool nextSkill4 = true;

    // Skill 5
    [Header("Skill5 Setting")]
    public float skill5CD = 5f;
    public float skill5Wait = 1f;
    public float skill5WarningDuration = 2f;
    public float skill5WarningBlinkInterval = 0.3f;
    public float skill5DelayDuration = 1.5f;
    public float skill5Damage = 40f;
    public float skill5Knockback = 20f;
    public GameObject skill5WarningAreaLine;
    public GameObject skill5AttackLine;
    public Color skill5BlinkColor = Color.white;
    public float skill5WarningAreaTransparent = 0.5f;
    public float skill5ShakeDuration = 2f;
    public float skill5ShakeMagnitude = 1f;
    private bool nextSkill5 = true;
    private bool skill5isDashing = false;
    private bool skill5isHitWall = false;



    // Handle Warning Area
    [Header("Warning Area Settings")]
    public Color blinkColor = Color.white;
    private GameObject currentWarningArea;
    private List<GameObject> playerInZone;


    // Raycast Setting
    [Header("Raycast Setting")]
    public LayerMask wallLayer;
    public LayerMask playerLayer;
    private const float DefaultRaycastDistance = 50f;


    // Other
    private const float IEUpdateInterval = 0.05f;
    private Rigidbody2D rb;


    // State Setting
    private enum State { Idle, Chase, Attack };
    private enum Attack { NormalAttack, Skill1, Skill2, Skill3, Skill4, Skill5, Skill6, Null }
    /* Attack Idea
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
    [SerializeField] private NormalAttackState normalAttackState = NormalAttackState.Initial;
    private bool isAttacking = false;

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

    void Update()
    {
        ApplyFriction();

        switch (currentState)
        {
            case State.Idle:
                Idle();
                break;

            case State.Attack:
                if (!isAttacking)
                {
                    switch (currentAttack)
                    {
                        case Attack.NormalAttack:
                            StartCoroutine(NormalAttackControllerIE());
                            break;

                        case Attack.Skill1:
                            StartCoroutine(Skill1ControllerIE());
                            break;

                        case Attack.Skill2:
                            StartCoroutine(Skill2ControllerIE());
                            break;

                        case Attack.Skill3:
                            StartCoroutine(Skill3ControllerIE());
                            break;

                        case Attack.Skill4:
                            StartCoroutine(Skill4ControllerIE());
                            break;

                        default:
                            break;
                    }
                }

                break;
        }
    }

    private void Idle()
    {
        if (testSkill != Attack.Null)
        {
            currentState = State.Attack;
            currentAttack = testSkill;
            return;
        }

        if (nextSkill4)
        {
            currentState = State.Attack;
            currentAttack = Attack.Skill4;
            return;
        }

        if (nextSkill3)
        {
            currentState = State.Attack;
            currentAttack = Attack.Skill3;
            return;
        }

        if (nextSkill1)
        {
            currentState = State.Attack;
            currentAttack = Attack.Skill1;
            return;
        }

        if (nextSkill2)
        {
            currentState = State.Attack;
            currentAttack = Attack.Skill2;
            return;
        }

        if (nextNomalAttack)
        {
            currentState = State.Attack;
            currentAttack = Attack.NormalAttack;
            return;
        }
    }

    public IEnumerator Skill5ControllerIE()
    {
        // Initialize state
        isAttacking = true;

        // Create warning area
        float elapsedTime = 0;
        Vector2 skillDirection = new Vector2(0, 0);
        Vector2 target = new Vector2(0, 0);

        GameObject spawnedLine = Instantiate(skill2WarningAreaLine, new Vector2(0, 0), Quaternion.identity);
        LineRenderer warningAreaLineRenderer = spawnedLine.GetComponent<LineRenderer>();
        bool activeBlink = false;

        while (elapsedTime < skill2WarningDuration - (skill2WarningDuration - skill2DelayDuration) * 0.6f)
        {
            // Setup warning area
            if (elapsedTime < skill2DelayDuration)
                skillDirection = (player.transform.position - transform.position).normalized;

            // Attack player in this raycast
            RaycastHit2D hit = Physics2D.Raycast(transform.position, skillDirection, Mathf.Infinity, wallLayer);
            if (hit.collider != null)
                target = hit.point;
            else
                target = (Vector2)transform.position + skillDirection * DefaultRaycastDistance;

            if (!activeBlink)
            {
                activeBlink = true;
                StartCoroutine(BlinkLine(warningAreaLineRenderer, skill2BlinkColor, skill2WarningAreaTransparent));
            }

            yield return DrawLine(warningAreaLineRenderer, target, IEUpdateInterval);

            elapsedTime += IEUpdateInterval;
        }
        Destroy(spawnedLine);

        yield return new WaitForSeconds((skill2WarningDuration - skill2DelayDuration) * 0.4f);

        RaycastHit2D playerHit = Physics2D.Raycast(transform.position, skillDirection, Vector2.Distance(transform.position, target), playerLayer);

        GameObject attackLine = Instantiate(skill2AttackLine, new Vector2(0, 0), Quaternion.identity);
        LineRenderer attackLineRenderer = attackLine.GetComponent<LineRenderer>();
        yield return DrawLine(attackLineRenderer, target, 0.15f);
        Destroy(attackLine);

        if (playerHit.collider != null)
        {
            // Damage the player
            if (playerHit.collider != null)
            {
                playerHit.collider.GetComponent<PlayerController>().TakeDamage(skill2Damage);
                playerHit.collider.GetComponent<PlayerMovement>().TakeKnockback((target - (Vector2)transform.position).normalized * skill2Knockback, 0.3f);
            }
        }
        StartCoroutine(ShackCamera(skill2ShakeDuration, skill2ShakeMagnitude));

        // Wait cooldown
        yield return new WaitForSeconds(skill2Wait);
        StartCoroutine(CountSkill2CD());

        // Set state to defualt
        isAttacking = false;
        currentState = State.Idle;
    }


    public IEnumerator Skill4ControllerIE()
    {
        // Initialize
        isAttacking = true;

        // Shank camera
        yield return ShackCamera(skill4ShakeDuration, skill4ShakeMagnitude);

        // Setup warning area
        BoundsInt bounds = groundTilemap.cellBounds;
        List<Vector3> validPositions = new List<Vector3>();
        List<GameObject> warningAreas = new List<GameObject>();

        for (int x = bounds.xMin; x < bounds.xMax; x += 3)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y += 3)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);

                // Check if the tile is on the ground layer and not obstructed
                if (groundTilemap.HasTile(cellPosition) &&
                    !wallTilemap.HasTile(cellPosition) &&
                    !obstacleTilemap.HasTile(cellPosition))
                {
                    // Convert cell position to world position and add to the valid list
                    Vector3 worldPosition = groundTilemap.CellToWorld(cellPosition) + groundTilemap.tileAnchor;
                    validPositions.Add(worldPosition);
                }
            }
        }

        // Randomly spawn objects at valid positions
        for (int i = 0; i < 5; i++)
        {
            if (validPositions.Count == 0) break;

            // Select a random position
            int randomIndex = Random.Range(0, validPositions.Count);
            Vector3 spawnPosition = validPositions[randomIndex];

            // Spawn the object
            GameObject warningArea = Instantiate(skill4WarningAreaPrefab, spawnPosition, Quaternion.identity);

            warningAreas.Add(warningArea);

            // Remove the position to avoid duplicate spawns
            validPositions.RemoveAt(randomIndex);
        }

        // Add Blink componet
        foreach (GameObject warningArea in warningAreas)
        {
            StartCoroutine(BlinkWarningArea(warningArea, skill4WarningDuration, skill4WarningBlinkInterval, blinkColor));
        }

        yield return new WaitForSeconds(skill4WarningDuration + 0.5f);

        StartCoroutine(ShackCamera(skill4ShakeDuration, skill4ShakeMagnitude));
        for (int i = warningAreas.Count - 1; i >= 0; i--)
        {
            StartCoroutine(SpawnMeteor(skill4MeteorPrefab, skill4AreaAttackPrefab, warningAreas[i].transform.position));
            Destroy(warningAreas[i]);
        }

        // Wait cooldown
        yield return new WaitForSeconds(skill4Wait * 3);
        StartCoroutine(CountSkill4CD());

        // Set state to defualt
        isAttacking = false;
        currentState = State.Idle;
    }



    public IEnumerator Skill3ControllerIE()
    {
        // Initialize
        skill3MoveSpeed = moveSpeed * 100f;
        float oldAcceleration = agent.acceleration;
        agent.acceleration = 200f;
        isAttacking = true;

        // Chase Player
        while (!ChasePlayer(0.5f, skill3MoveSpeed))
        {
            skill3MoveSpeed += Time.deltaTime;
            yield return new WaitForSeconds(IEUpdateInterval);
        }

        // Hit player before skill
        player.GetComponent<PlayerMovement>().TakeKnockback((player.transform.position - transform.position).normalized * skill3HitKnockBack, 0.3f);
        StartCoroutine(ShackCamera(skill3ShakeDuration, skill3ShakeMagnitude));

        // Setup warning area
        Vector3 direction = (player.transform.position - transform.position).normalized;
        Vector3 offset = direction * ((skill3WarningAreaPrefab.GetComponent<Transform>().localScale.x / 2) + skill3lengthOffset);
        Vector3 spawnPosition = transform.position + offset;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Instantiate the warning area
        currentWarningArea = Instantiate(skill3WarningAreaPrefab, spawnPosition, Quaternion.Euler(0, 0, angle), transform);
        currentWarningArea.AddComponent<AreaAttack>();

        // Call another warning area coroutine script
        StartCoroutine(DelayMoveAngle(currentWarningArea, skill3DelayDuration, skill3lengthOffset));
        yield return StartCoroutine(BlinkWarningArea(currentWarningArea, skill3WarningDuration, skill3WarningBlinkInterval, blinkColor));

        // Save waring area data and destroy this gameobject
        playerInZone = new List<GameObject>(currentWarningArea.GetComponent<AreaAttack>().playersInZone);
        Destroy(currentWarningArea);

        // Attack player in area
        foreach (GameObject player in playerInZone)
        {
            player.GetComponent<PlayerMovement>().TakeKnockback((player.transform.position - transform.position).normalized * skill3Knockback, 0.3f);
            player.GetComponent<PlayerController>().TakeDamage(skill3Damage);
        }
        StartCoroutine(ShackCamera(skill3ShakeDuration, skill3ShakeMagnitude));

        // Wait cooldown
        yield return new WaitForSeconds(skill3Wait);
        StartCoroutine(CountSkill3CD());

        // Set state to defualt
        skill3MoveSpeed = moveSpeed;
        agent.acceleration = oldAcceleration;
        isAttacking = false;
        currentState = State.Idle;
    }

    public IEnumerator Skill2ControllerIE()
    {
        // Initialize state
        isAttacking = true;

        // Create warning area
        float elapsedTime = 0;
        Vector2 skillDirection = new Vector2(0, 0);
        Vector2 target = new Vector2(0, 0);

        GameObject spawnedLine = Instantiate(skill2WarningAreaLine, new Vector2(0, 0), Quaternion.identity);
        LineRenderer warningAreaLineRenderer = spawnedLine.GetComponent<LineRenderer>();
        bool activeBlink = false;

        while (elapsedTime < skill2WarningDuration - (skill2WarningDuration - skill2DelayDuration) * 0.6f)
        {
            // Setup warning area
            if (elapsedTime < skill2DelayDuration)
                skillDirection = (player.transform.position - transform.position).normalized;

            // Attack player in this raycast
            RaycastHit2D hit = Physics2D.Raycast(transform.position, skillDirection, Mathf.Infinity, wallLayer);
            if (hit.collider != null)
                target = hit.point;
            else
                target = (Vector2)transform.position + skillDirection * DefaultRaycastDistance;

            if (!activeBlink)
            {
                activeBlink = true;
                StartCoroutine(BlinkLine(warningAreaLineRenderer, skill2BlinkColor, skill2WarningAreaTransparent));
            }

            yield return DrawLine(warningAreaLineRenderer, target, IEUpdateInterval);

            elapsedTime += IEUpdateInterval;
        }
        Destroy(spawnedLine);

        yield return new WaitForSeconds((skill2WarningDuration - skill2DelayDuration) * 0.4f);

        RaycastHit2D playerHit = Physics2D.Raycast(transform.position, skillDirection, Vector2.Distance(transform.position, target), playerLayer);

        GameObject attackLine = Instantiate(skill2AttackLine, new Vector2(0, 0), Quaternion.identity);
        LineRenderer attackLineRenderer = attackLine.GetComponent<LineRenderer>();
        yield return DrawLine(attackLineRenderer, target, 0.15f);
        Destroy(attackLine);

        if (playerHit.collider != null)
        {
            // Damage the player
            if (playerHit.collider != null)
            {
                playerHit.collider.GetComponent<PlayerController>().TakeDamage(skill2Damage);
                playerHit.collider.GetComponent<PlayerMovement>().TakeKnockback((target - (Vector2)transform.position).normalized * skill2Knockback, 0.3f);
            }
        }
        StartCoroutine(ShackCamera(skill2ShakeDuration, skill2ShakeMagnitude));

        // Wait cooldown
        yield return new WaitForSeconds(skill2Wait);
        StartCoroutine(CountSkill2CD());

        // Set state to defualt
        isAttacking = false;
        currentState = State.Idle;
    }

    public IEnumerator Skill1ControllerIE()
    {
        // Initialize state
        isAttacking = true;


        // Setup warning area
        Vector3 direction = (player.transform.position - transform.position).normalized;
        Vector3 offset = direction * ((skill1WarningAreaPrefab.GetComponent<Transform>().localScale.x / 2) + skill1lengthOffset);
        Vector3 spawnPosition = transform.position + offset;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;


        // Instantiate the warning area
        currentWarningArea = Instantiate(skill1WarningAreaPrefab, spawnPosition, Quaternion.Euler(0, 0, angle), transform);
        currentWarningArea.AddComponent<AreaAttack>();


        // Call another warning area coroutine script
        StartCoroutine(DelayMoveAngle(currentWarningArea, skill1DelayDuration, skill1lengthOffset));
        yield return BlinkWarningArea(currentWarningArea, skill1WarningDuration, skill1WarningBlinkInterval, blinkColor);


        // Save waring area data and destroy this gameobject
        float faceAngle = currentWarningArea.GetComponent<Transform>().eulerAngles.z;
        playerInZone = new List<GameObject>(currentWarningArea.GetComponent<AreaAttack>().playersInZone);
        Destroy(currentWarningArea);


        // Draw the raycast to find target position (wall)
        float radians = faceAngle * Mathf.Deg2Rad;
        Vector2 dashDirection = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)).normalized;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dashDirection, Mathf.Infinity, wallLayer);
        Vector2 dashTarget;
        if (hit.collider != null)
            dashTarget = hit.point;
        else
            dashTarget = (Vector2)transform.position + dashDirection * DefaultRaycastDistance;


        // Initialize and manage to moving target state
        StartCoroutine(ShackCamera(skill1ShakeDuration, skill1ShakeMagnitude));
        bool isHitPlayer = false;
        bool isHitWall = false;
        for (int i = 0; i < 2; i++)
        {
            float elapsedTime = 0;
            float durationCal;
            bool isback;

            if (i == 0) // First dash
            {
                isback = false;
                durationCal = skill1DashDuration;
            }
            else if (i == 1 && !isHitPlayer) // Sencond dash
            {
                if (isHitWall)
                {
                    Skill1AreaAttackIE();
                    StartCoroutine(ShackCamera(skill1ShakeDuration, skill1ShakeMagnitude));
                    isback = true;
                    durationCal = 0.15f;
                }
                else
                    continue;
            }
            else // Complete dash
            {
                if (isHitPlayer)
                {
                    Skill1AreaAttackIE();
                    StartCoroutine(ShackCamera(skill1ShakeDuration, skill1ShakeMagnitude));
                }

                rb.velocity = new Vector2(0, 0);
                break;
            }

            skill1isDashing = true;
            isHitWall = false;

            // Dash manager
            while (elapsedTime < durationCal)
            {
                isHitPlayer = false;
                if (Vector3.Distance(transform.position, player.position) < 2f)
                {
                    rb.velocity = new Vector2(0, 0);
                    player.GetComponent<PlayerController>().TakeDamage(skill1Damage);
                    player.GetComponent<PlayerMovement>().TakeKnockback((player.position - transform.position) * skill1Knockback, 0.3f);

                    isHitPlayer = true;
                    break;
                }
                else
                {
                    Vector3 _direction;
                    if (!isback)
                        _direction = ((Vector3)dashTarget - transform.position).normalized;
                    else
                        _direction = (transform.position - (Vector3)dashTarget).normalized;

                    rb.velocity = _direction * skill1DashSpeed;

                    if (!isback)
                    {
                        if (skill1isHitWall)
                        {
                            skill1isHitWall = false; // Set param to default for next wall hit
                            isHitWall = true;
                            break;
                        }
                    }
                }

                yield return new WaitForSeconds(IEUpdateInterval);
                elapsedTime += IEUpdateInterval;
            }
            skill1isDashing = false;
        }
        rb.velocity = new Vector2(0, 0);

        // Wait cooldown
        yield return new WaitForSeconds(skill2Wait);
        StartCoroutine(CountSkill1CD());

        // Set state to defualt
        isAttacking = false;
        currentState = State.Idle;
    }

    private void Skill1AreaAttackIE()
    {
        GameObject currentAreaAttact = Instantiate(skill1AreaAttackPrefab, transform.position, Quaternion.identity);
    }

    public IEnumerator SpawnMeteor(GameObject meteorPrefab, GameObject meterorArea, Vector3 targetPosition, float spawnHeightOffset = 15.8f, float meteorSpeed = 25, float angle = 30)
    {
        // Calculate spawn position above the camera
        Vector3 spawnPosition = new Vector3(
                targetPosition.x - spawnHeightOffset / Mathf.Tan(Mathf.Deg2Rad * (angle + Random.Range(-20, 20))),
                playerCamera.transform.position.y + spawnHeightOffset,
                targetPosition.z
        );

        // Instantiate the meteor
        GameObject meteor = Instantiate(meteorPrefab, spawnPosition, Quaternion.identity);
        Transform meteorTranform = meteor.GetComponent<Transform>();

        // Rotate the meteor to face the target
        Vector3 direction = (targetPosition - spawnPosition).normalized;
        float angleFace = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        meteor.transform.rotation = Quaternion.Euler(0, 0, angleFace);

        Rigidbody2D meteorRb = meteor.GetComponent<Rigidbody2D>();

        while (meteorTranform.position.y > targetPosition.y)
        {
            meteorRb.velocity = direction * meteorSpeed;
            yield return new WaitForSeconds(IEUpdateInterval);
        }

        StartCoroutine(ShackCamera(0.1f, 1f));
        GameObject meteorAttack = Instantiate(meterorArea, targetPosition, Quaternion.identity);
        Destroy(meteor);
    }

    private IEnumerator ShackCamera(float duration, float magnitude)
    {
        Transform camTransform = playerCamera.GetComponent<Transform>();
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Vector3 originalPos = camTransform.position;

            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            camTransform.localPosition = new Vector3(originalPos.x + offsetX, originalPos.y + offsetY, originalPos.z);

            yield return new WaitForSeconds(IEUpdateInterval);
            elapsed += IEUpdateInterval;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (skill1isDashing)
        {
            if (collision.gameObject.CompareTag("Wall"))
            {
                skill1isHitWall = true;
            }
        }
    }

    private IEnumerator DrawLine(LineRenderer lineRenderer, Vector2 dashTarget, float time)
    {
        lineRenderer.positionCount = 2; // Start and end points
        lineRenderer.SetPosition(0, transform.position); // Starting point
        lineRenderer.SetPosition(1, dashTarget); // Endpoint

        yield return DisableLineRenderer(lineRenderer, time);
    }

    private IEnumerator BlinkLine(LineRenderer lineRenderer, Color newColor, float transparent)
    {
        Color oldStartColor = lineRenderer.startColor;
        Color oldEndColor = lineRenderer.endColor;
        bool isOnOldColor = true;

        oldStartColor.a = transparent;
        oldEndColor.a = transparent;

        newColor.a = transparent;

        while (lineRenderer != null)
        {
            if (isOnOldColor)
            {
                lineRenderer.startColor = newColor;
                lineRenderer.endColor = newColor;
                isOnOldColor = false;
            }
            else
            {
                lineRenderer.startColor = oldStartColor;
                lineRenderer.endColor = oldEndColor;
                isOnOldColor = true;
            }

            yield return new WaitForSeconds(skill2WarningBlinkInterval);
        }
    }

    private IEnumerator DisableLineRenderer(LineRenderer lineRenderer, float time)
    {
        lineRenderer.enabled = true;
        yield return new WaitForSeconds(time);
        lineRenderer.enabled = false;
    }

    private IEnumerator BlinkWarningArea(GameObject _warningArea, float warningDuration, float warningBlinkInterval, Color blinkColor)
    {
        if (_warningArea != null)
        {
            float elapsedTime = 0f;
            SpriteRenderer warningSprite = _warningArea.GetComponent<SpriteRenderer>();
            Color oldColor = warningSprite.color;
            while (elapsedTime < warningDuration)
            {
                warningSprite.color = blinkColor;
                yield return new WaitForSeconds(warningBlinkInterval);
                warningSprite.color = oldColor;
                yield return new WaitForSeconds(warningBlinkInterval);
                elapsedTime += warningBlinkInterval * 2;
            }
        }
    }

    private IEnumerator DelayMoveAngle(GameObject _warningArea, float delayDuration, float lengthOffset)
    {
        float elapsedTime = 0f;

        while (elapsedTime < delayDuration)
        {
            Vector3 direction = (player.transform.position - transform.position).normalized;
            Vector3 offset = direction * ((_warningArea.GetComponent<Transform>().localScale.x / 2) + lengthOffset);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            _warningArea.transform.position = transform.position + offset;
            _warningArea.transform.rotation = Quaternion.Euler(0, 0, angle);

            yield return new WaitForSeconds(IEUpdateInterval);
            elapsedTime += IEUpdateInterval;
        }
    }

    private IEnumerator NormalAttackControllerIE()
    {
        // Initialize
        normalAttackMoveSpeed = moveSpeed * 2f;
        isAttacking = true;

        // Chase Player
        while (!ChasePlayer(0.5f, normalAttackMoveSpeed))
        {
            normalAttackMoveSpeed += Time.deltaTime;
            yield return new WaitForSeconds(IEUpdateInterval);
        }


        // Setup warning area
        Vector3 direction = (player.transform.position - transform.position).normalized;
        Vector3 offset = direction * ((normalAttackWarningAreaPrefab.GetComponent<Transform>().localScale.x / 2) + normalAttacklengthOffset);
        Vector3 spawnPosition = transform.position + offset;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Instantiate the warning area
        currentWarningArea = Instantiate(normalAttackWarningAreaPrefab, spawnPosition, Quaternion.Euler(0, 0, angle), transform);
        currentWarningArea.AddComponent<AreaAttack>();

        // Call another warning area coroutine script
        StartCoroutine(DelayMoveAngle(currentWarningArea, normalAttackDelayDuration, normalAttacklengthOffset));
        yield return StartCoroutine(BlinkWarningArea(currentWarningArea, normalAttackWarningDuration, normalAttackWarningBlinkInterval, blinkColor));

        // Save waring area data and destroy this gameobject
        playerInZone = new List<GameObject>(currentWarningArea.GetComponent<AreaAttack>().playersInZone);
        Destroy(currentWarningArea);

        // Attack player in area
        foreach (GameObject player in playerInZone)
        {
            player.GetComponent<PlayerMovement>().TakeKnockback((player.transform.position - transform.position).normalized * normalAttackKnockback, 0.3f);
            player.GetComponent<PlayerController>().TakeDamage(normalAttackDamage);
        }

        StartCoroutine(ShackCamera(normalAttackShakeDuration, normalAttackShakeMagnitude));

        // Wait cooldown
        yield return new WaitForSeconds(normalAttackWait);
        StartCoroutine(CountNormalAttackCD());

        // Set state to defualt
        normalAttackMoveSpeed = moveSpeed;
        isAttacking = false;
        currentState = State.Idle;
    }

    private void ApplyFriction()
    {
        // Calculate the friction force
        Vector2 frictionForce = -rb.velocity * moveFrictionCoefficient;
        rb.AddForce(rb.mass * frictionForce);
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

    IEnumerator CountSkill2CD()
    {
        nextSkill2 = false;
        yield return new WaitForSeconds(skill2CD);
        nextSkill2 = true;
    }

    IEnumerator CountSkill3CD()
    {
        nextSkill3 = false;
        yield return new WaitForSeconds(skill3CD);
        nextSkill3 = true;
    }

    IEnumerator CountSkill4CD()
    {
        nextSkill4 = false;
        yield return new WaitForSeconds(skill4CD);
        nextSkill4 = true;
    }

    private bool ChasePlayer(float chaseLength, float speed)
    {

        float distance = Vector3.Distance(transform.position, player.position);
        distance -= GetComponent<Transform>().localScale.y + player.GetComponent<Transform>().localScale.y;
        if (distance > chaseLength)
        {
            if (nextMovementState)
                agent.SetDestination(player.position);
            agent.speed = speed;

            return false;
        }
        agent.SetDestination(transform.position);
        agent.speed = 0;
        return true;
    }
}