using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Camera mainCamera;    // Reference to the main camera

    // Movemet speed of the player, Max (moveSpeed,moveSpeed) * 1second
    public float moveSpeed = 5f;

    public float dashCd = 4f; // Dash Colldown
    public float dashDuration = 2f;
    public float dashMultiple = 3f;


    public bool isforce = false;
    private bool isDashing = false;
    private bool nextDash = true;
    private Vector3 dashTarget;
    private float nextDashTime = 0f;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 movement;
    private float nextMovement = 0f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Get the Rigidbody2D component for physics-based movement
        rb = GetComponent<Rigidbody2D>();

        // Check if the mainCamera is assigned; if not, find the main camera
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            Camera.main.aspect = 1920f / 1080f;
        }
    }

    private IEnumerator CountDashCd()
    {

        yield return new WaitForSeconds(dashCd);
        nextDash = true;
    }

    private IEnumerator CountDashDuration()
    {


        nextDash = false;
        isDashing = true;
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
        StartCoroutine(CountDashCd());
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButton(1)) && Time.time > nextDashTime)
        {
            if (nextDash)
            {
                StartCoroutine(CountDashDuration());
            }

        }

    }

    public void TakeKnockback(Vector3 force, float time)
    {
        nextMovement = Time.time + time;
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    void FixedUpdate()
    {
        if (movement.x < 0)
            spriteRenderer.flipX = true;
        else if (movement.x > 0)
            spriteRenderer.flipX = false;


        if (Time.time > nextMovement)
            rb.velocity = movement * moveSpeed * (isDashing ? dashMultiple : 1);

        if (mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(rb.position.x, rb.position.y, mainCamera.transform.position.z);
        }
    }
}
