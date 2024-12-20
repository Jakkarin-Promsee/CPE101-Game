using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TestTools;
using UnityEngine.Tilemaps;

public class SpiritKing : MonoBehaviour
{
    public event Action<float> OnHpChanged;

    [Header("References")]
    [SerializeField] private Attack testSkill;
    [SerializeField] private Transform player;
    public GameObject playerCamera;
    private NavMeshAgent agent;
    public Tilemap groundTilemap;
    public Tilemap wallTilemap;
    public Tilemap obstacleTilemap;
    public GameObject[] enemyPrefab;
    public float GroundWarningCenterX = 75.82343f;
    public float GroundWarningCenterY = -53.70882f;


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
    public int skill4spawnAmount = 5;
    public float skill4CD = 2f;
    public float skill4Wait = 1f;
    public float skill4WarningDuration = 2f;
    public float skill4WarningBlinkInterval = 0.3f;
    public float skill4ShakeDuration = 4f;
    public float skill4ShakeMagnitude = 2f;
    private bool nextSkill4 = true;

    // Skill 5
    [Header("Skill5 Setting")]
    public float skill5CD = 5f;
    public float skill5Wait = 1f;
    public float skill5WarningDuration = 2f;
    public float skill5WarningBlinkInterval = 0.3f;
    public float skill5AttackAngle = 540;
    public float skill5Damage = 40f;
    public float skill5Knockback = 20f;
    public GameObject skill5WarningAreaPrefab;
    public GameObject skill5WarningAreaLine;
    public GameObject skill5AttackLine;
    public Color skill5BlinkColor = Color.white;
    public float skill5WarningAreaTransparent = 0.5f;
    public float skill5ShakeDuration = 2f;
    public float skill5ShakeMagnitude = 1f;
    public float skill5AttackLineSpeed = 5f;
    private bool nextSkill5 = true;

    // Skill 6
    [Header("Skill6 Setting")]
    public float skill6CD = 5f;
    public float skill6Wait = 1f;
    public float skill6WarningDuration = 2f;
    public float skill6WarningBlinkInterval = 0.3f;
    public float skill6SpawnBlinkDuration = 2f;
    public GameObject skill6WarningAreaPrefab;
    public GameObject skill6SpawnAreaPrefab;
    public float skill6ShakeDuration = 4f;
    public float skill6ShakeMagnitude = 2f;
    public int skill6spawnAmount = 4;
    private bool nextSkill6 = true;


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
    private Animator animator;


    // State Setting
    private enum State { Idle, Chase, Attack };
    public enum Attack { NormalAttack, Skill1, Skill2, Skill3, Skill4, Skill5, Skill6, Null };
    /* Attack Idea
        Idle => Just Idle, what is your expect?

        NormalAttack => medium warning area and straight fence sword

        Skill1 => direction warning area and dash to this direction
                  if hit something, spawn area attack

        Skill2 => direction warning area and shoot player with laser

        Skill3 => dash to player, large warning area, mega straight fence sword

        Skill4 => random large warning area, spawn meteor, make area attack

        Skill5 => all map warning area, shoot laser for 540 degree

        Skill6 => all map warning area, spawn random enemy
    */
    private bool isAttacking = false;


    // Boss phase setting
    [Header("Boss Phase Setting")]
    public bool isActive = true;
    public float maxHp = 1000f;
    public float currentHp;
    public Attack[] phase1 = {
        Attack.Skill6,
        Attack.NormalAttack,
        Attack.Skill1,
        Attack.NormalAttack,
        Attack.Skill2,
        Attack.Skill2,
        Attack.Skill1,
        };

    public Attack[] phase2 = {
        Attack.Skill6,
        Attack.Skill6,
        Attack.NormalAttack,
        Attack.Skill2,
        Attack.Skill2,
        Attack.Skill1,
        Attack.Skill1,
        Attack.Skill1,
        };

    public Attack[] phase3 =
    {
        Attack.Skill6,
        Attack.Skill4,
        Attack.Skill3,
        Attack.Skill3,
        Attack.Skill4,
        Attack.Skill5,
        Attack.Skill4,
    };

    public float[] hp2ActivePhase = { 100f, 80f, 40f };
    [SerializeField] private int currentPhase = 1;
    [SerializeField] private int phaseIdx = 1;
    private SpriteRenderer spriteRenderer;
    public Color flashColor = Color.white;  // Color to flash
    public float flashDuration = 0.1f;      // Duration of flash in seconds
    private Color originalColor;            // Original color to reset back to


    void Start()
    {
        currentHp = maxHp;

        spriteRenderer = GetComponent<SpriteRenderer>();

        player = GameObject.FindWithTag("Player").transform;

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        currentState = State.Idle;
        currentAttack = Attack.NormalAttack;
        rb = GetComponent<Rigidbody2D>();
        originalColor = spriteRenderer.color;
        animator = GetComponent<Animator>();
    }

    private bool isCall = true;

    public void TakeDamage(float damage)
    {
        float pt = currentHp / maxHp;
        OnHpChanged?.Invoke(currentHp / maxHp);

        if (currentHp <= 0)
        {
            foreach (GameObject enemy in enemies)
            {
                Destroy(enemy);
            }

            Destroy(gameObject);
        }

        else
            currentHp -= damage;
        StartCoroutine(FlashWhite());

        // public float[] hp2ActivePhase = { 100f, 80f, 40f };
        if (pt < 0.4f)
            ChangePhase(3);
        else if (pt < 0.8f)
            ChangePhase(2);
        else
            ChangePhase(1);
    }

    private IEnumerator FlashWhite()
    {
        // Set the sprite color to the flash color (white)
        spriteRenderer.color = flashColor;

        // Wait for the specified flash duration
        yield return new WaitForSeconds(flashDuration);

        // Reset the color back to the original color
        spriteRenderer.color = originalColor;
    }

    public void Active()
    {
        isActive = true;
    }

    public void UnActive()
    {
        isActive = false;
    }

    void Update()
    {
        Vector2 direction = player.position - transform.position;
        if (direction.x < 0)
            spriteRenderer.flipX = true;
        else if (direction.x > 0)
            spriteRenderer.flipX = false;

        if (isActive)
        {
            if (!isCall)
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

                                case Attack.Skill5:
                                    StartCoroutine(Skill5ControllerIE());
                                    break;

                                case Attack.Skill6:
                                    StartCoroutine(Skill6ControllerIE());
                                    break;

                                default:
                                    break;
                            }
                        }

                        break;
                }
            }
            else
                isCall = false;
        }
    }


    private void Idle()
    {
        // OldSkillController();
        NewSkillController();
    }

    private void ChangePhase(int newPhase)
    {
        currentPhase = newPhase;
        // phaseIdx = 0;
    }

    private void NewSkillController()
    {
        Attack[] localAttackPhase = null;

        switch (currentPhase)
        {
            case 1:
                localAttackPhase = phase1;
                break;
            case 2:
                localAttackPhase = phase2;
                break;
            case 3:
                localAttackPhase = phase3;
                break;
        }

        if (localAttackPhase != null)
        {
            if (phaseIdx >= localAttackPhase.Length)
                phaseIdx = 0;

            currentState = State.Attack;
            currentAttack = localAttackPhase[phaseIdx];

            phaseIdx++;
        }
    }

    private void OldSkillController()
    {
        if (testSkill != Attack.Null)
        {
            currentState = State.Attack;
            currentAttack = testSkill;
            return;
        }

        if (nextSkill6)
        {
            currentState = State.Attack;
            currentAttack = Attack.Skill6;
        }

        if (nextSkill5)
        {
            currentState = State.Attack;
            currentAttack = Attack.Skill5;
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

    List<GameObject> enemies = new List<GameObject>();

    public IEnumerator Skill6ControllerIE()
    {
        // Initialize
        isAttacking = true;

        // Shank camera
        StartCoroutine(ShackCamera(skill6ShakeDuration, skill6ShakeMagnitude));

        // Setup warning area
        BoundsInt bounds = groundTilemap.cellBounds;
        List<Vector3> validPositions = new List<Vector3>();
        List<GameObject> spawnAreas = new List<GameObject>();

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

        yield return StartCoroutine(CallAnimation("S4", 0.5f));
        StartCoroutine(CallAnimationRepeater("S4", 2f));

        // Setup warning area
        GameObject warningArea = Instantiate(skill5WarningAreaPrefab, new Vector2(GroundWarningCenterX, GroundWarningCenterY), Quaternion.identity);

        yield return BlinkWarningArea(warningArea, skill6WarningDuration, skill6WarningBlinkInterval, blinkColor);

        Destroy(warningArea);

        // Randomly spawn objects at valid positions
        for (int i = 0; i < skill6spawnAmount; i++)
        {
            if (validPositions.Count == 0) break;

            // Select a random position
            int randomIndex = UnityEngine.Random.Range(0, validPositions.Count);
            Vector3 spawnPosition = validPositions[randomIndex];

            // Spawn the object
            GameObject spawnArea = Instantiate(skill6SpawnAreaPrefab, spawnPosition, Quaternion.identity);

            spawnAreas.Add(spawnArea);

            // Remove the position to avoid duplicate spawns
            validPositions.RemoveAt(randomIndex);
        }

        // Add Blink componet
        foreach (GameObject spawnArea in spawnAreas)
        {
            StartCoroutine(BlinkWarningArea(spawnArea, skill6SpawnBlinkDuration, skill6WarningBlinkInterval, blinkColor));
        }

        yield return new WaitForSeconds(skill6SpawnBlinkDuration + 0.5f);

        StartCoroutine(ShackCamera(skill6ShakeDuration, skill6ShakeMagnitude));

        for (int i = spawnAreas.Count - 1; i >= 0; i--)
        {
            int j = UnityEngine.Random.Range(0, enemyPrefab.Length - 1);
            GameObject enemy = Instantiate(enemyPrefab[j], spawnAreas[i].transform.position, Quaternion.identity);

            if (enemy)
                enemies.Add(enemy);

            Destroy(spawnAreas[i]);
        }

        yield return AddIsAttacked2Enemies(enemies);

        // Wait cooldown
        yield return StartCoroutine(CallAnimationRepeater("T", skill6Wait));
        StartCoroutine(CountSkill4CD());

        // Set state to defualt
        isAttacking = false;
        currentState = State.Idle;
    }

    public IEnumerator AddIsAttacked2Enemies(List<GameObject> enemies)
    {
        yield return null;

        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<EnemyActionController>().IsAttacked();
        }
    }

    public IEnumerator Skill5ControllerIE()
    {
        // Initialize state
        isAttacking = true;
        Vector2 skillDirection = new Vector2(1, 0);
        Vector2 target;

        // Create warning area prefab at center of map
        GameObject warningArea = Instantiate(skill5WarningAreaPrefab, new Vector2(GroundWarningCenterX, GroundWarningCenterY), Quaternion.identity);

        // Create warning line prefab
        GameObject warningLine = Instantiate(skill5WarningAreaLine, new Vector2(0, 0), Quaternion.identity);
        LineRenderer warningLineRenderer = warningLine.GetComponent<LineRenderer>();

        // Calculate warning line length
        RaycastHit2D hit = Physics2D.Raycast(transform.position, skillDirection, Mathf.Infinity, wallLayer);
        if (hit.collider != null)
            target = hit.point;
        else
            target = (Vector2)transform.position + skillDirection * DefaultRaycastDistance;

        // Blink warning line
        StartCoroutine(DrawLine(warningLineRenderer, target, skill5WarningDuration));
        StartCoroutine(BlinkLine(warningLineRenderer, skill5BlinkColor, skill5WarningAreaTransparent));

        // Blink warning prefab
        yield return StartCoroutine(BlinkWarningArea(warningArea, skill5WarningDuration, skill5WarningBlinkInterval, blinkColor));

        // Delete warning element
        Destroy(warningArea);
        Destroy(warningLine);

        yield return new WaitForSeconds(skill5WarningDuration * 0.4f);

        StartCoroutine(CallAnimationRepeater("S5", 3f));

        // Create attack line
        GameObject attackLine = Instantiate(skill5AttackLine, new Vector2(0, 0), Quaternion.identity);
        float angle = 0;

        // Move attack line on skill5 angle
        while (angle < skill5AttackAngle)
        {
            float angle2Radius = angle * Mathf.Deg2Rad;
            skillDirection = new Vector2(Mathf.Cos(angle2Radius), Mathf.Sin(angle2Radius));

            hit = Physics2D.Raycast(transform.position, skillDirection, Mathf.Infinity, wallLayer);
            if (hit.collider != null)
                target = hit.point;
            else
                target = (Vector2)transform.position + skillDirection * DefaultRaycastDistance;

            RaycastHit2D playerHit = Physics2D.Raycast(transform.position, skillDirection, Vector2.Distance(transform.position, target), playerLayer);

            LineRenderer attackLineRenderer = attackLine.GetComponent<LineRenderer>();
            StartCoroutine(DrawLine(attackLineRenderer, target, IEUpdateInterval));

            if (playerHit.collider != null)
            {
                // Damage the player
                if (playerHit.collider != null)
                {
                    StartCoroutine(ShackCamera(skill5ShakeDuration, skill5ShakeMagnitude));
                    playerHit.collider.GetComponent<PlayerController>().TakeDamage(skill5Damage);
                    playerHit.collider.GetComponent<PlayerMovement>().TakeKnockback((target - (Vector2)transform.position).normalized * skill5Knockback, 0.3f);
                }
            }

            yield return new WaitForSeconds(IEUpdateInterval);
            angle += skill5AttackLineSpeed;
        }

        Destroy(attackLine);

        // Wait cooldown
        yield return StartCoroutine(CallAnimationRepeater("T", skill5Wait));
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

        yield return StartCoroutine(CallAnimation("S4", 0.5f));
        StartCoroutine(CallAnimationRepeater("S4", 2f));

        // Randomly spawn objects at valid positions
        for (int i = 0; i < skill4spawnAmount; i++)
        {
            if (validPositions.Count == 0) break;

            // Select a random position
            int randomIndex = UnityEngine.Random.Range(0, validPositions.Count);
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
        yield return StartCoroutine(CallAnimationRepeater("T", skill4Wait));
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
        while (!ChasePlayer(3f, skill3MoveSpeed))
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

        yield return StartCoroutine(CallAnimation("S3", 0.25f));

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
        yield return StartCoroutine(CallAnimationRepeater("T", skill3Wait));
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

        yield return StartCoroutine(CallAnimation("S2", 0.15f));

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
        yield return StartCoroutine(CallAnimationRepeater("T", skill2Wait));
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

            animator.SetTrigger("S1");

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
        yield return StartCoroutine(CallAnimationRepeater("T", skill1Wait));
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
                targetPosition.x - spawnHeightOffset / Mathf.Tan(Mathf.Deg2Rad * (angle + UnityEngine.Random.Range(-20, 20))),
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

            float offsetX = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            float offsetY = UnityEngine.Random.Range(-1f, 1f) * magnitude;

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
        while (!ChasePlayer(3f, normalAttackMoveSpeed))
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

        yield return StartCoroutine(CallAnimation("N", 0.3f));

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
        yield return StartCoroutine(CallAnimationRepeater("T", normalAttackWait));
        StartCoroutine(CountNormalAttackCD());

        // Set state to defualt
        normalAttackMoveSpeed = moveSpeed;
        isAttacking = false;
        currentState = State.Idle;
    }

    private IEnumerator CallAnimationRepeater(string triggerName, float time)
    {
        float lapseTime = 0f;
        while (lapseTime < time)
        {
            animator.SetTrigger(triggerName);
            yield return new WaitForSeconds(IEUpdateInterval);
            lapseTime += IEUpdateInterval;
        }
    }

    private IEnumerator CallAnimation(string triggerName, float time)
    {
        animator.SetTrigger(triggerName);
        yield return new WaitForSeconds(time);
    }


    private void ApplyFriction()
    {
        // Calculate the friction force
        Vector2 frictionForce = -rb.velocity * moveFrictionCoefficient;
        rb.AddForce(rb.mass * frictionForce);
    }

    private IEnumerator CountNormalAttackCD()
    {
        nextNomalAttack = false;
        yield return new WaitForSeconds(normalAttackCD);
        nextNomalAttack = true;
    }

    private IEnumerator CountSkill1CD()
    {
        nextSkill1 = false;
        yield return new WaitForSeconds(skill1CD);
        nextSkill1 = true;
    }

    private IEnumerator CountSkill2CD()
    {
        nextSkill2 = false;
        yield return new WaitForSeconds(skill2CD);
        nextSkill2 = true;
    }

    private IEnumerator CountSkill3CD()
    {
        nextSkill3 = false;
        yield return new WaitForSeconds(skill3CD);
        nextSkill3 = true;
    }

    private IEnumerator CountSkill4CD()
    {
        nextSkill4 = false;
        yield return new WaitForSeconds(skill4CD);
        nextSkill4 = true;
    }

    private IEnumerator CountSkill5CD()
    {
        nextSkill5 = false;
        yield return new WaitForSeconds(skill5CD);
        nextSkill5 = true;
    }

    private IEnumerator CountSKill6CD()
    {
        nextSkill6 = false;
        yield return new WaitForSeconds(skill6CD);
        nextSkill6 = true;
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